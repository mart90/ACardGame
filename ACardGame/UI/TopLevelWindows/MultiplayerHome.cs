using ACardGameLibrary;
using Newtonsoft.Json;
using TextCopy;

namespace ACardGame.UI
{
    public class MultiplayerHome : TopLevelUiWindow
    {
        private readonly ServerConnection _server;

        public User AuthenticatedUser => _server.AuthenticatedUser;

        public TextArea Message { get; set; }

        public Button JoinOpenChallengeButton { get; set; }
        public Button JoinFromClipboardButton { get; set; }
        public Button ToggleCreateChallengeButton { get; set; }

        public string OpenChallengeGuid { get; set; }

        public MultiplayerGame CreatedGame { get; set; }

        public MultiplayerHome(AssetManager assetManager, ServerConnection server) : base(assetManager)
        {
            CorrespondingUiState = UiState.MultiplayerHome;
            Texture = assetManager.LoadTexture("UI/wallpaper");
            _server = server;

            Build();
        }

        public void Build()
        {
            GoDown();
            SetCursor(43, 30);

            AddChild(new Button(AssetManager, ButtonType.Long, 14, true, "Join open challenge", delegate
            {
                JoinNearestOpen();
            }));

            AddSpacing(2);

            AddChild(new Button(AssetManager, ButtonType.Long, 14, true, "Join from clipboard", delegate
            {
                JoinFromClipboard();
            }));

            AddSpacing(2);

            ToggleCreateChallengeButton = new Button(AssetManager, ButtonType.Long, 14, true, "Create challenge", delegate
            {
                ToggleCreateChallenge();
            });
            AddChild(ToggleCreateChallengeButton);

            AddSpacing(2);

            AddChild(new Button(AssetManager, ButtonType.Long, 14, true, "Back to menu", delegate
            {
                NewUiState = UiState.MainMenu;
            }));

            AddSpacing(2);

            Message = new TextArea(AssetManager, "buttonFont", 20, true, 10);
            AddChild(Message);
        }

        public void JoinNearestOpen()
        {
            var response = _server.JoinNearestOpen();

            if (response.StatusCode != StatusCode.Ok)
            {
                Message.Text = JsonConvert.DeserializeObject<string>(response.Data);
            }
        }

        public void JoinFromClipboard()
        {
            var challengeId = ClipboardService.GetText();
            var response = _server.JoinFromClipboard(challengeId);

            if (response.StatusCode != StatusCode.Ok)
            {
                Message.Text = JsonConvert.DeserializeObject<string>(response.Data);
            }
        }

        public void ToggleCreateChallenge()
        {
            if (OpenChallengeGuid != null)
            {
                OpenChallengeGuid = null;
                _server.CancelChallenge();
                ToggleCreateChallengeButton.Text = "Create challenge";
                return;
            }

            var response = _server.CreateChallenge();

            if (response.StatusCode == StatusCode.Ok)
            {
                OpenChallengeGuid = JsonConvert.DeserializeObject<string>(response.Data);
                ClipboardService.SetText(OpenChallengeGuid);
                Message.Text = "Challenge id copied to clipboard";
                ToggleCreateChallengeButton.Text = "Cancel challenge";
            }
            else
            {
                Message.Text = JsonConvert.DeserializeObject<string>(response.Data);
            }
        }

        public override void Update()
        {
            if (UpdateCounter % 15 == 0)
            {
                _server.PollEvents();
            }

            if (_server.IncomingMessage != null)
            {
                if (_server.IncomingMessage.MessageType == ServerMessageType.GameStart)
                {
                    GameStartMessage gameStartMessage = JsonConvert.DeserializeObject<GameStartMessage>(_server.IncomingMessage.Data);

                    CreatedGame = new MultiplayerGame(AssetManager, new GameStateManager(), _server)
                    {
                        Id = gameStartMessage.GameId
                    };

                    CreatedGame.GameState.ActivePlayer.Name = gameStartMessage.IPlayFirst ? AuthenticatedUser.Name : gameStartMessage.OpponentName;
                    CreatedGame.GameState.Enemy.Name = gameStartMessage.IPlayFirst ? gameStartMessage.OpponentName : AuthenticatedUser.Name;

                    Extensions.ShuffleSeed = gameStartMessage.ShuffleSeed;

                    CreatedGame.PrepareShop();
                    CreatedGame.BuildUI();
                    CreatedGame.Update();

                    NewUiState = UiState.MultiplayerGame;
                }

                _server.IncomingMessage = null;
            }
        }
    }
}
