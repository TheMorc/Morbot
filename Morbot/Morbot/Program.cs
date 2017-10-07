using System;
using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext.Exceptions;

namespace Morbot
{
    class Program
    {
        static DiscordClient discord;
        public static DiscordGame game = new DiscordGame();
        static public string prefix = "--";
        static CommandsNextModule commands;
        
        static void Main(string[] args)
        {
            
            if (!File.Exists("token"))
            {
                File.Create("token");
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("token")[0];
                }
                catch
                {
                    empty = true;
                }
                

            }
            if (!File.Exists("ytapikey"))
            {
                File.Create("ytapikey");
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("ytapikey")[0];
                }
                catch
                {
                    empty = true;
                }

            }

            if (!File.Exists("giphyapikey"))
            {
                File.Create("giphyapikey");
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("giphyapikey")[0];
                }
                catch
                {
                    empty = true;
                }

            }

            if (!File.Exists("openwapikey"))
            {
                File.Create("openwapikey");
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("openwapikey")[0];
                }
                catch
                {
                    empty = true;
                }

            }
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            
            }
        
        

        static async Task MainAsync(string[] args)
        {


            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllLines("token")[0],
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = prefix
            });
            commands.RegisterCommands<Commands>();
            await discord.ConnectAsync();
            discord.Ready += async ex =>
            {
                game.Name = prefix + "help|Bot ready.";
                game.StreamType = GameStreamType.NoStream;
                await discord.UpdateStatusAsync(game);
            };
            await Task.Delay(-1);
        }

    }
}