using System;
using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Morbot
{
    class Program
    {
        //simpul introduction to this bot
        static DiscordClient discord;
        public static Game game = new Game();
        static public string prefix = "--";
        static CommandsNextModule commands;
        public static void CWrite(string v, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(v);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void Main(string[] args)
        {
            CWrite("Morbot - started!", ConsoleColor.White);


            if (!File.Exists("token"))
            {
                File.Create("token");
                CWrite("Creation of token file succeeded", ConsoleColor.Green);
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("token")[0];
                }
                catch (Exception ex)
                {
                    empty = true;
                    CWrite("Contents of Token file are empty! This discord bot cannot work until you fill everything!", ConsoleColor.Red);
                }
                if (!empty)
                    CWrite("Token file already exists. Contents: " + txt, ConsoleColor.Green);

            }


            if (!File.Exists("ytapikey"))
            {
                File.Create("ytapikey");
                CWrite("Creation of Youtube Data API Key file succeeded", ConsoleColor.Green);
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("ytapikey")[0];
                }
                catch (Exception ex)
                {
                    empty = true;
                    CWrite("Contents of Youtube Data API Key file are empty! This discord bot cannot work until you fill everything!", ConsoleColor.Red);
                }
                if (!empty)
                    
                CWrite("Youtube Data API Key file already exists. Contents: " + txt, ConsoleColor.Green);

            }

            try
            {
                CWrite("Everything seems to be working fine!", ConsoleColor.Green);
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {

                if(ex.Message == "Index was outside the bounds of the array.")
                {
                    CWrite("Discord token and YTAPIKey files set incorrectly", ConsoleColor.Red);
                    
                }
                
                CWrite("Bot got issue! (Error code: " + ex.Message + " )..", ConsoleColor.Red);
                while (true)
                {
                    Console.ReadKey();
                }
            }
            }

        

        static async Task MainAsync(string[] args)
        {
            
            //initializing discord client that is needed to have a working discord bot(really? i dont need this line but as always do it! xDDD)
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllLines("token")[0],
                TokenType = TokenType.Bot
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = prefix
            });

            discord.Ready += async e =>
            {
                game.Name = prefix+"help|Bot ready.";
                game.StreamType = GameStreamType.NoStream;
                await discord.UpdateStatusAsync(game);
            };

            
            commands.RegisterCommands<Commands>();
            await discord.ConnectAsync();
            
            await Task.Delay(-1);
        }

    }
}