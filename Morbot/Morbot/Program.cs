using System;
using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Linq;

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
            string minutes = null;
            if (DateTime.Now.TimeOfDay.Minutes.ToString().Length == 1)
            {
                minutes = "0" + DateTime.Now.TimeOfDay.Minutes.ToString();
            }
            else
            {
                minutes = DateTime.Now.TimeOfDay.Minutes.ToString();
            }
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes + "]" + v);
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

            if (!File.Exists("giphyapikey"))
            {
                File.Create("giphyapikey");
                CWrite("Creation of giphyapikey file succeeded", ConsoleColor.Green);
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("giphyapikey")[0];
                }
                catch (Exception ex)
                {
                    empty = true;
                    CWrite("Contents of Giphy API Key file are empty! This discord bot cannot work until you fill everything!", ConsoleColor.Red);
                }
                if (!empty)
                    CWrite("Giphy API Key file already exists. Contents: " + txt, ConsoleColor.Green);

            }

            if (!File.Exists("openwapikey"))
            {
                File.Create("openwapikey");
                CWrite("Creation of openwapikey file succeeded", ConsoleColor.Green);
            }
            else
            {
                string txt = "";
                bool empty = false;
                try
                {
                    txt = File.ReadAllLines("openwapikey")[0];
                }
                catch (Exception ex)
                {
                    empty = true;
                    CWrite("Contents of OpenWeather API Key file are empty! This discord bot cannot work until you fill everything!", ConsoleColor.Red);
                }
                if (!empty)
                    CWrite("OpenWeather API Key file already exists. Contents: " + txt, ConsoleColor.Green);

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
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllLines("token")[0],
                TokenType = TokenType.Bot
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
                //string serverlist = null;
                
                //foreach (string server in discord.Guilds.Values.Select(e => e.Name))
                //{

                    
                //    CWrite(server);
                //    string help = "";
                //    try { help = serverlist.Remove(server.Length, serverlist.Length - server.Length); } catch { }
                //    if (help == server) { }
                //    else
                //    {
                //        serverlist = server + serverlist;
                //    }
                //}

                //CWrite("I am on these servers: " + serverlist, ConsoleColor.Green);
            };
            await Task.Delay(-1);
        }

    }
}