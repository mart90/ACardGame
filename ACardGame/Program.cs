// dotnet publish ACardGame/ACardGame.csproj -c release -r win-x64 --self-contained -o out_game

using var game = new ACardGame.Main();
game.Run();
