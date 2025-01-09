// dotnet publish ACardGame/ACardGame.csproj -c release -r win-x64 --self-contained -o "Cards for gamers"

using var game = new ACardGame.Main();
game.Run();
