﻿namespace ACardGameServer
{
    // dotnet publish ACardGameServer/ACardGameServer.csproj -c release -r linux-x64 --self-contained -o out
    public class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/log");
            new ServerMain().Start();
        }
    }
}
