using ACardGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace ACardGame.UI
{
    public class LoginScreen : TopLevelUiWindow
    {
        public TextInput Username { get; set; }
        public TextInput Password { get; set; }

        public TextArea ErrorMessage { get; set; }

        private readonly ServerConnection _server;

        public LoginScreen(AssetManager assetManager, ServerConnection server) : base(assetManager)
        {
            CorrespondingUiState = UiState.LoginScreen;
            Texture = assetManager.LoadTexture("UI/wallpaper");
            _server = server;

            Build();

            Username.HasFocus = true;
        }

        public void Build()
        {
            SetCursor(35, 40);

            AddChild(new TextArea(AssetManager, "buttonFont", 5, true, 5)
            {
                Text = "Username:",
                TextOffset = new Vector2(2, 40),
                ForceOneLine = true
            });
            AddSpacing(1);

            Username = new TextInput(AssetManager, "buttonFont", 20, true, 10);
            AddChild(Username);

            SetCursor(35, 45);
            AddChild(new TextArea(AssetManager, "buttonFont", 5, true, 5)
            {
                Text = "Password:",
                TextOffset = new Vector2(2, 40),
                ForceOneLine = true
            });
            AddSpacing(1);

            Password = new TextInput(AssetManager, "buttonFont", 20, true, 10) { IsPassword = true };
            AddChild(Password);

            GoDown();
            SetCursor(41, 53);

            AddChild(new Button(AssetManager, ButtonType.Long, 10, true, "Login", delegate
            {
                Login();
            }));

            AddSpacing(2);

            AddChild(new Button(AssetManager, ButtonType.Long, 10, true, "Register", delegate
            {
                Register();
            }));

            AddSpacing(2);

            AddChild(new Button(AssetManager, ButtonType.Long, 10, true, "Back", delegate
            {
                NewUiState = UiState.MainMenu;
            }));

            AddSpacing(2);

            ErrorMessage = new TextArea(AssetManager, "buttonFont", 20, true, 10)
            {
                IsVisible = false
            };
            AddChild(ErrorMessage);
        }

        private void Login()
        {
            var response = _server.Login(Username.Text, Password.Text);
            
            if (response.StatusCode == StatusCode.Ok)
            {
                NewUiState = UiState.MultiplayerHome;
            }
            else
            {
                ErrorMessage.Text = JsonConvert.DeserializeObject<string>(response.Data);
                ErrorMessage.IsVisible = true;
            }
        }

        private void Register()
        {
            var response = _server.Register(Username.Text, Password.Text);
            
            if (response.StatusCode == StatusCode.Ok)
            {
                NewUiState = UiState.MultiplayerHome;
            }
            else
            {
                ErrorMessage.Text = JsonConvert.DeserializeObject<string>(response.Data);
                ErrorMessage.IsVisible = true;
            }
        }

        public override void ReceiveKeyboardInput(TextInputEventArgs args)
        {
            if (args.Key == Keys.Tab)
            {
                Username.HasFocus = false;
                Password.HasFocus = true;
            }
            else if (args.Key == Keys.Enter)
            {
                Login();
            }

            base.ReceiveKeyboardInput(args);
        }
    }
}
