using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus.EventArgs;
using System;
using DSharpPlus.CommandsNext.Exceptions;
using System.Reflection;

namespace Morbot
{
    public class Program
    {
        public static DiscordClient discord;
        static public string prefix;
        static CommandsNextExtension commands;
        static VoiceNextExtension voice;
        public static ConfigJSON configuration = new ConfigJSON();
        public static string version = "1.9.2";
        public static DiscordActivity game = new DiscordActivity();
        public static string DiscordActivityText = null;

        public class ConfigJSON
        {
            public string DiscordBotToken { get; set; }
            public string YoutubeDataAPIKey { get; set; }
            public string OpenWeatherAPIKey { get; set; }
            public string PixabayAPIKey { get; set; }
            public string GiphyAPIKey { get; set; }
            public string YandexAPIKey { get; set; }
            public string Prefix { get; set; }
        }

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                File.Create("config.json");
            }
            else
            {
                var json = "";
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = await sr.ReadToEndAsync();
                configuration = JsonConvert.DeserializeObject<ConfigJSON>(json);
            }
            if (!Directory.Exists("Songs"))
            {
                Directory.CreateDirectory("Songs");
            }
            if (!Directory.Exists("WEBSongs"))
            {
                Directory.CreateDirectory("WEBSongs");
            }
            if (!Directory.Exists("Pictures"))
            {
                Directory.CreateDirectory("Pictures");
            }
            if (!Directory.Exists("WinPics"))
            {
                Directory.CreateDirectory("WinPics");
            }

            DiscordActivityText = $"type {prefix}help|ver: {version}";

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = configuration.DiscordBotToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = false
            });

            discord.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
            discord.MessageCreated += async e =>
            {
                if (!e.Message.Author.IsBot)
                {
                    if (e.Message.Content.ToLower().StartsWith("clap"))
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":clap:"));
                    }
                    else if (e.Message.Content.ToLower().StartsWith("mhm"))
                    {
                        Random rand = new Random();
                        if (rand.Next(2) == 1)
                        {
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thinking:"));
                        }
                        else
                        {
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thonkong:"));
                        }
                    }
                    else if (e.Message.Content.ToLower().StartsWith("hmst"))
                    {
                        Random rand = new Random();
                        if (rand.Next(2) == 1)
                        {
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thinking:"));
                        }
                        else
                        {
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thonkong:"));
                        }
                    }
                    else if (e.Message.Content.ToLower().StartsWith("hmm"))
                    {
                        Random rand = new Random();
                        if (rand.Next(2) == 1)
                        {
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thinking:"));
                        }
                        else
                        {
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thonkong:"));
                        }
                    }
                    else if (e.Message.Content.ToLower().StartsWith("ok"))
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":ok_hand:"));
                    }
                    else if (e.Message.Content.ToLower().StartsWith("jsx"))
                    {
                        await e.Message.RespondAsync("**easier**");
                    }
                    else if (e.Message.Content.ToLower().StartsWith("**easier**"))
                    {
                        await e.Message.RespondAsync("JSX");
                    }
                    else if (e.Message.Content.ToLower().StartsWith("easier"))
                    {
                        await e.Message.RespondAsync("JSX");
                    }
                    else if (e.Message.Content.StartsWith(":") & e.Message.Content.EndsWith(":"))
                    {
                        await e.Message.DeleteAsync();
                        await e.Message.RespondAsync(DiscordEmoji.FromName(e.Client, e.Message.Content));
                    }

                }
            };

            prefix = configuration.Prefix;
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = configuration.Prefix != null ? new[] { configuration.Prefix } : null,
                EnableDms = true,
                EnableMentionPrefix = true
            });

            commands.CommandErrored += async e =>
            {
                if (e.Exception is CommandNotFoundException)
                {
                    await Commands.CreateMessage(e.Context, desc: $"Command doesn't exist.\nUse command `--help` to see list of commands.", color: DiscordColor.Red);
                }
                else
                {
                    await Commands.CreateMessage(e.Context, desc: $"{Commands.error_message}`{e.Exception.Message}`", color: DiscordColor.Red);
                }
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Morbot", $"{e.Context.Member.Username}#{e.Context.Member.Discriminator} on {e.Context.Guild.Name} executed command --{e.Command.Name} and the command failed\n{e.Exception.Message}", DateTime.Now);
            };

            commands.CommandExecuted += async e =>
            {
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "Morbot", $"{e.Context.Member.Username}#{e.Context.Member.Discriminator} on {e.Context.Guild.Name} succesfully executed command --{e.Command.Name}", DateTime.Now);
            };

            voice = discord.UseVoiceNext(new VoiceNextConfiguration
            {
                VoiceApplication = DSharpPlus.VoiceNext.Codec.VoiceApplication.Music
            });
            commands.RegisterCommands(Assembly.GetExecutingAssembly());
            await discord.ConnectAsync();
            discord.Ready += async ex =>
                        {
                            game.Name = DiscordActivityText;
                            game.ActivityType = ActivityType.Watching;
                            await discord.UpdateStatusAsync(game);
                        };

            discord.GuildMemberAdded += async e =>
            {
                await Commands.CreateHelloImage(e.Member, Commands.Welcome_on_server);
                await e.Guild.Channels[0].SendFileAsync("Pictures/morbot_image.png");
            };
            discord.GuildMemberRemoved += async e =>
            {
                await Commands.CreateHelloImage(e.Member, Commands.You_will_be_missed);
                await e.Guild.Channels[0].SendFileAsync("Pictures/morbot_image.png");
            };
            await Task.Delay(-1);
        }

        private static void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Timestamp.ToString("MM.dd HH:mm:ss")}] {e.Message}");
        }
    }
}