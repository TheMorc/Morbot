using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus.EventArgs;
using System.Linq;

namespace Morbot
{
    class Program
    {
        public static DiscordClient discord;
        public static DiscordGame game = new DiscordGame();
        static public string prefix = "";
        static CommandsNextExtension commands;
        static VoiceNextExtension voice;
        public static configJSON configuration = new configJSON();
        public static string version = "1.7.2";
        private Task Client_Ready(ReadyEventArgs exx)
        {
            exx.Client.Guilds[0].Channels.Where(e => e.Id == 0);


            return Task.CompletedTask;
        }
        public class configJSON
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
                configuration = JsonConvert.DeserializeObject<configJSON>(json);
            }

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = configuration.DiscordBotToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });



            prefix = configuration.Prefix;
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = prefix,
                EnableDms = true,
                EnableMentionPrefix = true,
                CaseSensitive = false
            });

            voice = discord.UseVoiceNext(new VoiceNextConfiguration
            {
                VoiceApplication = DSharpPlus.VoiceNext.Codec.VoiceApplication.Music
            });
            commands.RegisterCommands<Commands>();
            await discord.ConnectAsync();
            discord.Ready += async ex =>
            {
                game.Name = prefix + "help|Ready|V:" + version;
                game.StreamType = GameStreamType.NoStream;
                await discord.UpdateStatusAsync(game);
            };

            await Task.Delay(-1);
        }

    }
}