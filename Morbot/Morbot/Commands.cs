﻿using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using System.Linq;
using System.IO;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using ImageMagick;

namespace Morbot
{


    public class Commands
    {
        static readonly string embed_title = "Morbot [ver: " + Program.version + ", Made in 🇸🇰, By: Morc]";
        static readonly string error_message = ":no_entry: Bot encoutered an error!!! \n";

        //TASKS FOR COMMANDS ala translate, createmessage etc.
        #region tasks for commands
        public string ShortenName(string value, string addargument)
        {
            string shortened;
            if (value.Length > 200)
            {
                shortened = value.Remove(200, value.Length - 200) + addargument;
            }
            else
            {
                shortened = value;
            }
            return shortened;
        }
        public async Task<string> translate(string text)
        {
            string resultstring = "";
            string data = "";
            string page = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=" + Program.configuration.YandexAPIKey + "&text=" + text + "&lang=" + text.Remove(6, text.Length - 6);
            using (HttpClient cl = new HttpClient())
            {
                data = await cl.GetStringAsync(page);
                JSONs.RootObjectresult result = new JSONs.RootObjectresult();
                result = JsonConvert.DeserializeObject<JSONs.RootObjectresult>(data);
                foreach (string resul in result.text)
                {
                    resultstring = resultstring + "   " + resul;
                }
            }
            return resultstring;
        }
        public async Task SetSpeaking(CommandContext e, bool SetSpeaking)
        {
            var vnext = e.Client.GetVoiceNext();
            var vnc = vnext.GetConnection(e.Guild);
            await vnc.SendSpeakingAsync(SetSpeaking);
        }
        public static async Task CreateMessage(CommandContext e, string titleurl = null, string imageurl = null, string thumbnailurl = "https://github.com/NaamloosDT/DSharpPlus/blob/4858631e87392a8586a685bd0e9cb2a96f7d1ffb/logo/d%23+_smaller.png?raw=true", string url = null, string desc = "", string title = "", DiscordColor color = default(DiscordColor), bool sendToUser = false)
        {
            if (title == "")
            {
                title = embed_title;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Color = color,
                Description = desc,
                ImageUrl = imageurl,
                ThumbnailUrl = thumbnailurl,
                Url = url,
                Timestamp = DateTime.Now
            };
            if (sendToUser)
                await e.Member.SendMessageAsync("", embed: embed);
            else
                await e.RespondAsync("", embed: embed);
        }
        private async Task music(CommandContext e, string v, double speed = 1.0)
        {

            var vnext = e.Client.GetVoiceNext();
            var vnc = vnext.GetConnection(e.Guild);

            Exception exc = null;
            try
            {
                if (speed == 0)
                    speed = 1.0;
                if (speed == 0.0)
                    speed = 1.0;
                var ffmpeg_inf = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i \"{v}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var ffmpeg = Process.Start(ffmpeg_inf);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                using (var ms = new MemoryStream())
                {
                    await ffout.CopyToAsync(ms);
                    ms.Position = 0;

                    var buff = new byte[3840];
                    var br = 0;
                    while ((br = ms.Read(buff, 0, buff.Length)) > 0)
                    {
                        if (br < buff.Length)
                            for (var i = br; i < buff.Length; i++)
                                buff[i] = 0;

                        try
                        {
                            await vnc.SendAsync(buff, 20);
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { exc = ex; }
            finally
            {
                await vnc.SendSpeakingAsync(false);
            }

        }
        #endregion

        //admin only commands
        #region servers command
        [Command("servers"), RequirePermissions(DSharpPlus.Permissions.ManageGuild), Hidden, Description("Should be hidden! But this command shows list of servers that it is on...")]
        public async Task Servers(CommandContext ex)
        {
            string serverlist = null;
            foreach (string server in ex.Client.Guilds.Values.Select(e => e.Name))
            {
                string help = "";
                try { help = serverlist.Remove(server.Length, serverlist.Length - server.Length); } catch { }
                if (help == server) { }
                else
                {
                    serverlist = server + "\n" + serverlist;
                }
            }
            await CreateMessage(ex, desc: "Servers: " + serverlist, sendToUser: true, color: DiscordColor.Cyan);
        }
        #endregion

        //Commands
        #region whoami command
        [Command("whoami"), Description("Bot responds with info about bot! It is that simple...")]
        public async Task Whoami(CommandContext e)
        {
            await CreateMessage(e, desc: "I am Morbot, and Morc creates this bot for Discord server but also as example.\nLink to sourcecode: https://www.github.com/TheMorc/Morbot \nMade with DSharpPlus API(ver: " + e.Client.VersionString + ")", color: DiscordColor.Cyan);
        }
        #endregion
        #region love command
        [Command("love"), Description("Who needs command that generates random number and sends it?? ANYONE! but this is great example for array string command!!")]
        public async Task Love(CommandContext e, params string[] name)
        {
            Random rand = new Random();
            string loveemoji = "";
            DiscordColor laavcolor = DiscordColor.None;
            int num = rand.Next(0, 100);
            if (num < 15)
            {
                loveemoji = ":broken_heart:";
                laavcolor = DiscordColor.Brown;
            }
            else if (num < 30)
            {
                loveemoji = ":blue_heart:";
                laavcolor = DiscordColor.Blue;
            }
            else if (num < 50)
            {
                loveemoji = ":green_heart:";
                laavcolor = DiscordColor.Green;
            }
            else if (num < 70)
            {
                loveemoji = ":yellow_heart:";
                laavcolor = DiscordColor.Yellow;
            }
            else if (num < 100)
            {
                loveemoji = ":heart:";
                laavcolor = DiscordColor.Red;
            }
            string laav = null;
            bool passed = false;
            string[] names = { "Andrej", "Babiš", "Monika", "Adam" };
            foreach (string fename in names)
            {
                if (!passed)
                {
                    if (name[0] == fename)
                    {
                        loveemoji = ":heart:";
                        laavcolor = DiscordColor.Red;
                        laav = name[0] + " + " + name[1] + " = " + "100% " + loveemoji;
                        passed = true;
                    }
                    else
                    {
                        laav = name[0] + " + " + name[1] + " = " + num + "%   " + loveemoji;
                        passed = true;
                    }
                }

            }
            await CreateMessage(e, desc: laav, color: laavcolor);
        }
        #endregion
        #region ping command
        [Command("ping"), Description("What simpler than this! This sends ping in miliseconds!")]
        public async Task Ping(CommandContext e)
        {
            await CreateMessage(e, desc: "Ping: " + e.Client.Ping + "ms", color: DiscordColor.Green);
        }
        #endregion
        #region latestvideo command
        [Command("latestvideo"), Aliases("latestmorcvideo", "morcvideo", "lastvideo", "lastvideobymorc"), Description("This command pulls link of last video posted by Morc")]
        public async Task Latestvideo(CommandContext e)
        {
            try
            {
                var yt = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = Program.configuration.YoutubeDataAPIKey
                });
                var channelsListRequest = yt.Channels.List("contentDetails");
                channelsListRequest.ForUsername = "riskoautobus";
                var channelsListResponse = channelsListRequest.Execute();
                foreach (var channel in channelsListResponse.Items)
                {
                    // of videos uploaded to the authenticated user's channel.
                    var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
                    var nextPageToken = "";
                    while (nextPageToken != null)
                    {
                        var playlistItemsListRequest = yt.PlaylistItems.List("snippet");
                        playlistItemsListRequest.PlaylistId = uploadsListId;
                        playlistItemsListRequest.MaxResults = 50;
                        playlistItemsListRequest.PageToken = nextPageToken;

                        // Retrieve the list of videos uploaded to the authenticated user's channel.
                        var playlistItemsListResponse = playlistItemsListRequest.Execute();
                        string ytlink = "https://youtu.be/" + playlistItemsListResponse.Items[0].Snippet.ResourceId.VideoId;
                        await e.Message.RespondAsync("\u200B " + ytlink);
                        nextPageToken = playlistItemsListResponse.NextPageToken;
                    }
                }
            }
            catch (Exception exy)
            {
                await CreateMessage(e, desc: error_message + exy, color: DiscordColor.Red);
            }

        }
        #endregion
        #region weather command


        [Command("weather"), Description("Bot responds with actual temperature in °C.Weather gets pulled from OpenWeather and city is Topoľčany(small town near village Biskupová where Morc lives).")]
        public async Task CWeather(CommandContext e, [RemainingText]string town = "Topolcany")
        {
            string data = "";
            string weathertype = null;
            double temp = 0;
            try
            {
                string page = "http://api.openweathermap.org/data/2.5/weather?q=" + System.Web.HttpUtility.UrlEncode(town) + "&mode=json&APPID=" + Program.configuration.OpenWeatherAPIKey;
                using (HttpClient cl = new HttpClient())
                {
                    data = await cl.GetStringAsync(page);
                    JSONs.RootObjectW2 oRootObject = new JSONs.RootObjectW2();
                    try
                    {

                        oRootObject = JsonConvert.DeserializeObject<JSONs.RootObjectW2>(data);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                    }
                    weathertype = null;
                    DiscordColor wcolor = DiscordColor.None;
                    temp = oRootObject.main.temp - 273.15;
                    if (oRootObject.weather[0].description == "clear sky")
                    {
                        weathertype = ":sunny:" + " - Sunny";
                        wcolor = DiscordColor.Yellow;
                    }
                    if (oRootObject.weather[0].description == "broken clouds")
                    {
                        weathertype = ":cloud:" + " - Clouds";
                        wcolor = DiscordColor.Gray;
                    }
                    if (oRootObject.weather[0].description == "few clouds")
                    {
                        weathertype = ":cloud:" + " - Clouds";
                        wcolor = DiscordColor.Gray;
                    }
                    if (oRootObject.weather[0].description == "light rain")
                    {
                        weathertype = ":cloud_rain:" + " - Rain";
                        wcolor = DiscordColor.Cyan;
                    }

                    if (oRootObject.weather[0].description == "mist")
                    {
                        weathertype = ":fog:" + " - Fog/Mist";
                        wcolor = DiscordColor.Cyan;
                    }
                    await CreateMessage(e, desc: oRootObject.name + " - " + oRootObject.sys.country + ", specified by " + e.User.Username + "\n" + temp + "°C \n" + weathertype, color: wcolor);
                    //await CreateMessage(e, desc: "Town near Morc - Topoľčany:\n" + temp + "°C \n" + weathertype, color: wcolor);
                }
            }
            catch (Exception ex)
            {
                await CreateMessage(e, desc: error_message + ex, color: DiscordColor.Red);
            }
        }
        #endregion
        #region randomnorrisjoke command

        [Command("randomnorrisjoke"), Aliases("norris", "norrisjoke", "chucknorris", "chuck", "chuckjoke", "randomchuckjoke"), Description("Chuck Norris was born earlier than he died! Command pulls random joke from ChuckNorris API..")]
        public async Task ChuckNorris(CommandContext e, string language = "")
        {
            string data = "";
            string url = "";
            string page = "https://api.chucknorris.io/jokes/random";
            using (HttpClient cl = new HttpClient())
            {
                data = await cl.GetStringAsync(page);
                JSONs.RootObjectnorris chuck = new JSONs.RootObjectnorris();
                chuck = JsonConvert.DeserializeObject<JSONs.RootObjectnorris>(data);
                url = chuck.url;
                if (language == "")
                {
                    await CreateMessage(e, title: "Chuck Norris joke:", desc: chuck.value, thumbnailurl: chuck.icon_url, url: "https://api.chucknorris.io", color: DiscordColor.Green);
                }
                else
                {

                    await CreateMessage(e, title: "Chuck Norris joke in English:", desc: chuck.value, thumbnailurl: chuck.icon_url, url: "https://api.chucknorris.io", color: DiscordColor.Green);
                    string translation = await translate(language + " " + chuck.value);
                    await CreateMessage(e, title: "Chuck Norris joke in " + translation.Remove(9, translation.Length - 9) + ":", desc: translation.Remove(0, 9), thumbnailurl: chuck.icon_url, url: "https://api.chucknorris.io", color: DiscordColor.Green);
                }
            }
        }

        #endregion
        #region translate command

        [Command("translate"), Description("eee what else? one! what else? two what else?? ..... translate command!!")]
        public async Task translate(CommandContext e, params string[] args)
        {
            string data = "";
            if (args[0] == "languages")
            {
                string page = "https://translate.yandex.net/api/v1.5/tr.json/getLangs?key=" + Program.configuration.YandexAPIKey;
                using (HttpClient cl = new HttpClient())
                {
                    data = await cl.GetStringAsync(page);
                    JSONs.RootObjectlanguages languages = new JSONs.RootObjectlanguages();
                    languages = JsonConvert.DeserializeObject<JSONs.RootObjectlanguages>(data);
                    string langstring = "";
                    foreach (string lang in languages.dirs)
                    {
                        langstring = langstring + "   " + lang;
                    }
                    await CreateMessage(e, desc: langstring, color: DiscordColor.Green);
                }
            }
            else
            {
                string text = "";
                foreach (string arg in args)
                {
                    text = text + " " + arg;
                }
                string result = await translate(text);
                await CreateMessage(e, desc: result.Remove(0, 9));

            }
        }

        #endregion
        #region time command
        [Command("time"), Description("WHO WANTS THIS COMMAND???")]
        public async Task Time(CommandContext e)
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
            await CreateMessage(e, desc: "Morc's time zone is UTC+01:00 so the time is: " + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes, color: DiscordColor.Orange);
        }
        #endregion
        #region randomwindows command
        string[] versions = { "w95.png", "w98.png", "wme.png", "w2k.png", "w7.png", "wvista.png", "wxp.png", "w8.png", "w10.png" };
        [Command("randomwindows"), Aliases("RandomWindows", "RandWind", "randwind", "ranwin", "randomwin", "RandomWin", "rwin", "randomw", "randw"), Description("Command sends random picture(Windows 95/98/ME/2000/XP/Vista/7/8/10).")]
        public async Task RandomWindows(CommandContext e)
        {
            Random rnd = new Random();
            string ver = versions[rnd.Next(0, versions.Length)];
            await e.Message.RespondWithFileAsync(ver);
        }
        #endregion
        #region cat command
        [Command("meow"), Aliases("cat", "kitty", "catpicture", "meov", "mjau"), Description("Command cat and dog does the same thing. Sends cute picture/gif of cat or dog!")]
        public async Task Meow(CommandContext e)
        {
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://random.cat/meow");
                var pData = JObject.Parse(data);
                url = pData["file"].ToString();
                await CreateMessage(e, thumbnailurl: "http://random.cat/random.cat-logo.png", imageurl: url, color: DiscordColor.Green);
            }

        }
        #endregion
        #region dog command
        [Command("woof"), Aliases("dog", "puppy", "dogpicture", "hau", "haw"), Description("Command cat and dog does the same thing. Sends cute picture/gif of cat or dog!")]
        public async Task Woof(CommandContext e)
        {
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://random.dog/woof.json");
                var pData = JObject.Parse(data);
                url = pData["url"].ToString();
                await CreateMessage(e, imageurl: url, color: DiscordColor.Green);
            }
        }
        #endregion
        #region status command
        [Group("changestatus", CanInvokeWithoutSubcommand = true), Aliases("status"), Description("This command changes status of bot.(the Playing below name in users)")]
        public class Status
        {
            public async Task ExecuteGroupAsync(CommandContext e, string name = "")
            {
                if (name == "BETA")
                {
                    await BETA(e);
                    await CreateMessage(e, desc: "Status set to " + name, color: DiscordColor.Green);
                }
                else if (name == "WIP")
                {
                    await WIP(e);
                    await CreateMessage(e, desc: "Status set to " + name, color: DiscordColor.Green);
                }
                else if (name == "FIX")
                {
                    await FIX(e);
                    await CreateMessage(e, desc: "Status set to " + name, color: DiscordColor.Green);
                }
                else if (name == "READY")
                {
                    await READY(e);
                    await CreateMessage(e, desc: "Status set to " + name, color: DiscordColor.Green);
                }
                else if (name == "")
                {
                    await CreateMessage(e, desc: "Select status from one of these: BETA WIP FIX READY !", color: DiscordColor.Red);
                }
                else
                {
                    await CreateMessage(e, desc: name + " isn't a status. Select status from one of these: BETA WIP FIX READY !", color: DiscordColor.Red);
                }
            }
            [Command("null1")]
            public async Task BETA(CommandContext e)
            {
                string gamename = Program.prefix + "help|BETA Mode|V:" + Program.version;
                DiscordGame game = new DiscordGame()
                {
                    StreamType = GameStreamType.NoStream,
                    Name = gamename
                };
                await e.Client.UpdateStatusAsync(game);
            }
            [Command("null2")]
            public async Task WIP(CommandContext e)
            {
                string gamename = Program.prefix + "help|WIP Mode|V:" + Program.version;
                DiscordGame game = new DiscordGame()
                {
                    StreamType = GameStreamType.NoStream,
                    Name = gamename
                };
                await e.Client.UpdateStatusAsync(game);
            }
            [Command("null3")]
            public async Task FIX(CommandContext e)
            {
                string gamename = Program.prefix + "help|FIX Mode|V:" + Program.version;
                DiscordGame game = new DiscordGame()
                {
                    StreamType = GameStreamType.NoStream,
                    Name = gamename
                };
                await e.Client.UpdateStatusAsync(game);
            }
            [Command("null4")]
            public async Task READY(CommandContext e)
            {
                string gamename = Program.prefix + "help|Ready|V:" + Program.version;
                DiscordGame game = new DiscordGame()
                {
                    StreamType = GameStreamType.NoStream,
                    Name = gamename
                };
                await e.Client.UpdateStatusAsync(game);
            }
        }
        #endregion
        #region gifv2 command

        [Command("gif"), Description("If anything isnt specified with command, then it responds with random dog or cat(dog or cat randomized by bot) gif(by giphy).")]
        public async Task GIFSearch(CommandContext e, [RemainingText]string arg1 = "")
        {
            string gifby = "";
            string gifurl = "";
            Random Rand = new Random();
            string[] GIFtype = { "cat", "dog" };
            string[] Emoji = { ":cat:", ":dog:", ":joy_cat:", ":poop:" };
            string page = null;
            if (arg1 == "")
            {
                arg1 = GIFtype[Rand.Next(0, GIFtype.Length)];
                page = "http://api.giphy.com/v1/gifs/random?q=cat&tag=" + arg1 + "&api_key=" + Program.configuration.GiphyAPIKey;

            }
            else
            {
                page = "http://api.giphy.com/v1/gifs/random?q=cat&tag=" + arg1 + "&api_key=" + Program.configuration.GiphyAPIKey;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(page))
                using (HttpContent content = response.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    JSONs.RootObjectG oRootObject = new JSONs.RootObjectG();
                    oRootObject = JsonConvert.DeserializeObject<JSONs.RootObjectG>(data);
                    gifurl = oRootObject.data.image_url;
                    if (oRootObject.data.username == "")
                    {
                        gifby = "";
                    }
                    else
                    {
                        gifby = Emoji[Rand.Next(0, Emoji.Length)] + " By: " + oRootObject.data.username;
                    }
                    await CreateMessage(e, thumbnailurl: "https://www.inboxsdk.com/images/logos/giphy.png", desc: gifby, imageurl: gifurl, color: DiscordColor.Green);
                }
            }
            catch (Exception ex)
            {

                await CreateMessage(e, desc: error_message + ex, color: DiscordColor.Red);
            }

        }



        #endregion
        #region picture command

        [Command("picture"), Aliases("pic", "pix", "image", "img", "photo"), Description("This command works only if something is specified for ex 'Zetor' then it sends picture of zetor tractor.."), Hidden]
        public async Task IMGSearch(CommandContext e, [RemainingText]string arg1 = "")
        {
            Random rand = new Random();
            string page = "https://pixabay.com/api/?key=" + Program.configuration.PixabayAPIKey + "&q=" + arg1 + "&image_type=photo";

            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(page))
                using (HttpContent content = response.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    JSONs.GIFRootObject oRootObject = new JSONs.GIFRootObject();
                    oRootObject = JsonConvert.DeserializeObject<JSONs.GIFRootObject>(data);
                    int num = rand.Next(0, oRootObject.hits.Capacity);
                    await CreateMessage(e, desc: "Photo by: " + oRootObject.hits[num].user + "\nViews: " + oRootObject.hits[num].views, imageurl: oRootObject.hits[num].webformatURL, thumbnailurl: "https://pixabay.com/static/img/logo_square.png");
                }
            }
            catch (Exception ex)
            {

                await CreateMessage(e, desc: error_message + ex, color: DiscordColor.Red);
            }



        }



        #endregion
        #region voice channel join command
        [Command("join"), Aliases("vchjoin", "voicechanneljoin", "voicejoin", "channeljoin", "voicechjoin"), Description("Joins a voice channel.")]
        public async Task connectToVoiceChannel(CommandContext e)
        {
            DiscordChannel chn = null;
            var vstat = e.Member?.VoiceState;
            if (chn == null)
                chn = vstat.Channel;
            if (vstat?.Channel == null && chn == null)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: "You are not in any Voice Channel!");
                return;
            }

            var vnext = e.Client.GetVoiceNext();
            if (vnext == null)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: error_message + "VoiceNext is not enabled or configured properly.");
                return;
            }

            var vnc = vnext.GetConnection(e.Guild);
            if (vnc == null)
            {
                await CreateMessage(e, color: DiscordColor.Yellow, desc: "Not connected in this guild. Connecting to user's voice channel :)");
                vnc = await vnext.ConnectAsync(chn);
                await CreateMessage(e, color: DiscordColor.Green, desc: $"Connected to `{chn.Name}`");
            }

            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();
        }
        #endregion
        #region voice channel leave command
        [Command("leave"), Aliases("vchleave", "voicechannelleave", "voiceleave", "channelleave", "voicechleave"), Description("Leaves a voice channel.")]
        public async Task Leave(CommandContext e)
        {
            var vnext = e.Client.GetVoiceNext();
            if (vnext == null)
            {
                await CreateMessage(e, desc: error_message, color: DiscordColor.Red);
                return;
            }

            var vnc = vnext.GetConnection(e.Guild);
            if (vnc == null)
            {
                await CreateMessage(e, desc: "Not connected on this server", color: DiscordColor.Green);
                return;
            }

            vnc.Disconnect();
            await CreateMessage(e, desc: "Disconnected", color: DiscordColor.Green);
        }
        #endregion
        #region voice channel play command



        [Command("play"), Aliases("vchplay", "voicechannelplay", "voiceplay", "channelplay", "voicechplay"), Description("Plays an audio file.")]
        public async Task Play(CommandContext e, [RemainingText, Description("Full path to the file to play.")] string filename, DiscordChannel chn = null)
        {
            await connectToVoiceChannel(e);
            if (filename.Contains("youtu"))
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "youtube",
                            Arguments = filename + " --dump-json",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = false,

                        }
                    };
                    string data = null;
                    process.Start();
                    while (!process.StandardOutput.EndOfStream)
                    {
                        data = await process.StandardOutput.ReadToEndAsync();
                    }
                    bool ready = true;
                    if (ready)
                    {
                        try
                        {
                            JSONs.RootObjectvideo2 oRootObject = new JSONs.RootObjectvideo2();
                            oRootObject = JsonConvert.DeserializeObject<JSONs.RootObjectvideo2>(data);

                            string videoname = ShortenName("M:/" + oRootObject.id + "_" + System.Web.HttpUtility.UrlEncode(oRootObject.fulltitle) + ".mp3", ".mp3");
                            if (File.Exists(videoname))
                            {
                                await CreateMessage(e, desc: "Playing: `" + oRootObject.fulltitle + "`", imageurl: oRootObject.thumbnail);
                                await SetSpeaking(e, true);
                                await music(e, videoname);
                            }
                            else
                            {
                                HttpClient cl = new HttpClient();
                                HttpResponseMessage response = await cl.GetAsync(oRootObject.formats[1].url);
                                using (FileStream fs = new FileStream(videoname, FileMode.Create))
                                {

                                    await CreateMessage(e, desc: "Playing: `" + oRootObject.fulltitle + "`", imageurl: oRootObject.thumbnail);
                                    await response.Content.CopyToAsync(fs);
                                    await SetSpeaking(e, true);
                                    await music(e, videoname);
                                    ready = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex);
                        }
                        return;
                    }

                    if (ready)
                    {
                        try
                        {
                            JSONs.RootObjectvideo1 oRootObject = new JSONs.RootObjectvideo1();
                            oRootObject = JsonConvert.DeserializeObject<JSONs.RootObjectvideo1>(data);

                            string videoname = ShortenName("M:/" + oRootObject.id + "_" + oRootObject.fulltitle + ".mp3", ".mp3");

                            if (File.Exists(videoname))
                            {
                                await CreateMessage(e, desc: "Playing: `" + oRootObject.fulltitle + "`");
                                await SetSpeaking(e, true);
                                await music(e, videoname);
                            }
                            else
                            {
                                HttpClient cl = new HttpClient();
                                HttpResponseMessage response = await cl.GetAsync(oRootObject.formats[1].url);
                                using (FileStream fs = new FileStream(videoname, FileMode.Create))
                                {

                                    await CreateMessage(e, desc: "Playing: `" + oRootObject.fulltitle + "`");
                                    await response.Content.CopyToAsync(fs);
                                    await SetSpeaking(e, true);
                                    await music(e, videoname);
                                    ready = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            await CreateMessage(e, desc: ex.ToString());
                        }
                        return;
                    }
                    else
                    {
                        return;
                    }


                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
            else if (filename.StartsWith("http"))
            {
                string site = "H:/" + filename + ".mp3";
                if (File.Exists(site))
                {
                    await CreateMessage(e, desc: $"Playing: `{site}`");
                    await SetSpeaking(e, true);
                    await music(e, site);
                }
                else
                {

                    HttpClient cl = new HttpClient();
                    HttpResponseMessage response = await cl.GetAsync(filename);
                    using (FileStream fs = new FileStream(site, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fs);
                        await CreateMessage(e, desc: $"Playing: `{filename}`");
                        await SetSpeaking(e, true);
                        await music(e, site);
                    }
                }
            }
            else
            {
                if (!File.Exists(filename))
                {
                    await e.RespondAsync($"File `{filename}` does not exist.");
                    return;
                }
                await e.Message.RespondAsync($"Playing `{filename}`");
                await SetSpeaking(e, true);
                await music(e, filename);

            }
        }
        #endregion
        #region speak command
        [Command("speak"), Aliases("vchspeak", "voicechannelspeak", "voicespeak", "channelspeak", "voicechspeak"), Description("speaks for you in voice channel :)")]
        public async Task speak(CommandContext e, params string[] args)
        {
            await connectToVoiceChannel(e);
            string text = "";
            foreach (string arg in args)
            {
                text = text + " " + arg;
            }
            if (text.Remove(0, 1).Length > 200)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: "This request cannot be processed because the length of text is over 200!\nLength: " + text.Remove(0, 1).Length);
                return;
            }

            string speakdata = text.Remove(0, 1);
            string speaktext = speakdata.Remove(0, 3);

            string page = "https://translate.google.com/translate_tts?ie=UTF-8&q=" + System.Web.HttpUtility.UrlEncode(speaktext) + "&tl=" + speakdata.Remove(2, speakdata.Length - 2) + "&client=tw-ob";
            string filename = ShortenName("M:/Translator/" + speakdata.Remove(2, speakdata.Length - 2) + "_" + System.Web.HttpUtility.UrlEncode(speaktext) + ".mp3", ".mp3");

            if (File.Exists(filename))
            {
                await CreateMessage(e, color: DiscordColor.Green, desc: "Speaking: " + speakdata.Remove(0, 3));
                await SetSpeaking(e, true);
                await music(e, filename);
            }
            else
            {
                HttpClient cl = new HttpClient();
                HttpResponseMessage response = await cl.GetAsync(page);
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);

                    await CreateMessage(e, color: DiscordColor.Green, desc: "Speaking: " + speakdata.Remove(0, 3));
                    await SetSpeaking(e, true);
                    await music(e, filename);
                }
            }
        }

        #endregion
        #region music command
        [Command("music"), Aliases("musiclist", "vchmusiclist", "voicechannelmusiclist", "voicemusiclist", "channelmusiclist", "voicechmusiclist"), Description("sends list of music on my pc")]
        public async Task music(CommandContext e, params string[] args)
        {
            string rootfilelist = null;
            string downfilelist = null;
            foreach (string file in Directory.GetFiles("M:/"))
            {
                rootfilelist = rootfilelist + "\n" + file;
            }
            foreach (string file in Directory.GetFiles("M:/Downloaded"))
            {
                downfilelist = downfilelist + "\n" + file;
            }


            await CreateMessage(e, desc: e.User.Mention + " sent list to DM");
            await CreateMessage(e, desc: rootfilelist, sendToUser: true);
            await CreateMessage(e, desc: downfilelist, sendToUser: true);
        }

        #endregion
        #region hidden commands
        [Command("žaneta"), Hidden, Description("sings section of hej žaneta song, text arrangement by crafty ")]
        public async Task žaneta(CommandContext e)
        {
            await connectToVoiceChannel(e);
            await CreateMessage(e, color: DiscordColor.Green, desc: "Hej Žaneta https://youtu.be/jtdJAnZNDto", imageurl: "https://i.ytimg.com/vi/jtdJAnZNDto/hqdefault.jpg?sqp=-oaymwEXCPYBEIoBSFryq4qpAwkIARUAAIhCGAE=&rs=AOn4CLB8rIsvdWdSN67GmbM48erLIuvjbQ");
            await SetSpeaking(e, true);
            await music(e, "M:/Downloaded/zaneta.mp3");

        }

        [Command("tttie"), Hidden, Description("TTtie")]
        public async Task tttie(CommandContext e)
        {
            await e.RespondWithFileAsync("tttie.png");
        }
        #endregion

        #region compress command
        [Command("compress"), Description("If you specify value from 0 to 100 then the bot compresses to the value. 0 awful | 100 great")]
        public async Task compress(CommandContext e, [RemainingText]string args = "5")
        {
            File.Delete("temp.jpg");
            string url;
            int quality;
            if (args.Length > 1)
            {
                if (args.StartsWith("h"))
                {
                    quality = 5;
                    url = args;
                }
                else if (args.StartsWith("w"))
                {
                    quality = 5;

                    url = args;
                }
                else
                {

                    url = args.Remove(0, 2);
                    quality = Int32.Parse(args.Remove(1, args.Length - 1));
                }
                HttpClient imgdown = new HttpClient();
                HttpResponseMessage imgrespons = await imgdown.GetAsync(url);
                using (FileStream fs = new FileStream("temp.jpg", FileMode.Create))
                {
                    await imgrespons.Content.CopyToAsync(fs);
                }
            }
            else
            {
                quality = Int32.Parse(args);
                HttpClient cl = new HttpClient();
                HttpResponseMessage response = await cl.GetAsync(e.Message.Attachments[0].Url);
                using (FileStream fs = new FileStream("temp.jpg", FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
            File.Delete("compressed.jpg");

            using (var img = new MagickImage("temp.jpg"))
            {
                img.Strip();
                img.Quality = quality;
                img.Write("compressed.jpg");
            }
            await e.RespondWithFileAsync("compressed.jpg");
            FileInfo info1 = new FileInfo("temp.jpg");
            FileInfo info2 = new FileInfo("compressed.jpg");
            await CreateMessage(e, color: DiscordColor.Green, desc: "Here is your fresh compressed art. We know that it is delicious!\nStats:\nSize before compressing: " + (info1.Length / 1024) + "KB\nSize after compressing: " + (info2.Length / 1024 + "KB"));
        }
        #endregion

    }
}
