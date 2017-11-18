using System;
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
using System.Drawing;

namespace Morbot
{
    public class Commands
    {
        string lastCommit = null;
        string commitDate = null;
        static readonly string embed_title = "Morbot [ver: " + Program.version + ", Made in 🇸🇰, By: Morc]";
        public static readonly string error_message = ":no_entry: An exception occurred!!!\n";
        static private DiscordActivity botActivity = new DiscordActivity();
        DiscordActivity temp = new DiscordActivity("temp", ActivityType.Playing);

        //mhm, wot??
        #region tasks for commands
        private void LastCommitFromGitHub()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

                using (var response = client.GetAsync("https://api.github.com/repos/TheMorc/Morbot/commits").Result)
                {
                    var json = response.Content.ReadAsStringAsync().Result;

                    dynamic commits = JArray.Parse(json);
                    lastCommit = commits[0].commit.message;
                    commitDate = commits[0].commit.author.date;
                }
            }
        }
        public string CalculateAge(string date)
        {
            //DateTime age = Convert.ToDateTime("01 October 2017 3:05:31 PM");
            DateTime age = Convert.ToDateTime(date);
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(age).Ticks).Year - 1;
            DateTime PastYearDate = age.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }
            int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
            int Hours = Now.Subtract(PastYearDate).Hours;
            int Minutes = Now.Subtract(PastYearDate).Minutes;
            int Seconds = Now.Subtract(PastYearDate).Seconds;
            return String.Format("**{0}** Months **{1}** Days **{2}** Hours **{3}** Minutes **{4}** Seconds",
            Months, Days, Hours, Minutes, Seconds);
        }
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
        public Image WatermarkIt(Image img)
        {
            Bitmap watermarkIMG = new Bitmap("watermark.png");
            const byte ALPHA = 255;
            Color clr;
            for (int py = 0; py < watermarkIMG.Height; py++)
            {
                for (int px = 0; px < watermarkIMG.Width; px++)
                {
                    clr = watermarkIMG.GetPixel(px, py);
                    watermarkIMG.SetPixel(px, py,
                        Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
                }
            }

            watermarkIMG.MakeTransparent(watermarkIMG.GetPixel(0, 0));

            Graphics g = Graphics.FromImage(img);
            g.DrawImage(watermarkIMG, img.Width - watermarkIMG.Width, img.Height - watermarkIMG.Height);

            Bitmap wtf = new Bitmap(img.Width, img.Height, g);
            return wtf;
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
                            if (vnc.Channel == null) { }
                            else
                            { await vnc.SendAsync(buff, 20); }
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
                if (vnc.Channel == null) { }
                else
                { await vnc.SendSpeakingAsync(false); }

            }

        }
        #endregion
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
        #region bot command
        [Command("bot")]
        public async Task Bot(CommandContext e)
        {
            LastCommitFromGitHub();
            await CreateMessage(e, color: DiscordColor.Blurple,
            desc: "**Morbot** is OpenSource bot maintained by **Morc** and **Made in Slovakia** with D#+ (DSharpPlus) API." +
            "\nAPI Version: `" + e.Client.VersionString +
            "`.\n\n**Age of Bot(since first commit on GitHub | 1 October 2017):** " + CalculateAge("01 October 2017 3:05:31 PM") +
            "\n\n**Age of last commit:** " + CalculateAge(commitDate) +
            "\n**Name of last commit:** " + lastCommit +
            "\n\n**Bot Source Code:** \nhttps://www.github.com/TheMorc/Morbot " +
            "\n\n**D#+ GitHub:** \nhttps://github.com/NaamloosDT/DSharpPlus");
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
            var yt = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Program.configuration.YoutubeDataAPIKey
            });
            var channelsListRequest = yt.Channels.List("contentDetails");
            channelsListRequest.ForUsername = "riskoautobus";
            var channelsListResponse = channelsListRequest.Execute();
            foreach (var channel in channelsListResponse.Items)
            {
                var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
                var nextPageToken = "";
                while (nextPageToken != null)
                {
                    var playlistItemsListRequest = yt.PlaylistItems.List("snippet");
                    playlistItemsListRequest.PlaylistId = uploadsListId;
                    playlistItemsListRequest.MaxResults = 50;
                    playlistItemsListRequest.PageToken = nextPageToken;
                    var playlistItemsListResponse = playlistItemsListRequest.Execute();
                    string ytlink = "https://youtu.be/" + playlistItemsListResponse.Items[0].Snippet.ResourceId.VideoId;
                    await e.Message.RespondAsync("\u200B " + ytlink);
                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
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
        #region mode command
        [Command("changemode"), Aliases("mode"), Description("This command changes mode of bot.(Playing,Streaming,Watching,Listening To)")]
        public async Task BETA(CommandContext e, [RemainingText]string mode)
        {
            bool finished = false;
            string[] streaming = { "streaming", "Streaming", "Stream", "stream" };
            string[] playing = { "playing", "Playing", "Play", "play" };
            string[] watching = { "watching", "Watching", "Watch", "watch" };
            string[] listening = { "listeningto", "Listeningto", "Listen", "listen", "listento", "Listento", "Listen to", "listen to", "listening to", "Listening to" };

            foreach (string name in streaming)
            {
                if (!finished)
                {
                    if (mode == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.Streaming;
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: "Sucessfully changed mode to `Streaming` by: " + e.Member.Mention);
                    }
                }
            }
            foreach (string name in playing)
            {
                if (!finished)
                {
                    if (mode == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.Playing;
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: "Sucessfully changed mode to `Playing` by: " + e.Member.Mention);
                    }
                }
            }
            foreach (string name in watching)
            {
                if (!finished)
                {
                    if (mode == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.Watching;
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: "Sucessfully changed mode to `Watching` by: " + e.Member.Mention);
                    }
                }
            }
            foreach (string name in listening)
            {
                if (!finished)
                {
                    if (mode == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.ListeningTo;
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: "Sucessfully changed mode to `Listening To` by: " + e.Member.Mention);
                    }
                }
            }
            if (!finished)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: "You Specified: " + mode + "\nSo it looks like you specified the wrong bot mode. :joy:");
            }
        }
        #endregion
        #region gif command
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
        #endregion
        #region picture command
        [Command("picture"), Aliases("pic", "pix", "image", "img", "photo"), Description("This command works only if something is specified for ex 'Zetor' then it sends picture of zetor tractor..")]
        public async Task IMGSearch(CommandContext e, [RemainingText]string arg1 = "")
        {
            Random rand = new Random();
            string page = "https://pixabay.com/api/?key=" + Program.configuration.PixabayAPIKey + "&q=" + arg1 + "&image_type=photo";

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
        #endregion
        #region hidden commands
        [Command("žaneta"), Aliases("zani", "žani", "zaneta", "hulvat"), Hidden, Description("sings section of hej žaneta song, text arrangement by crafty ")]
        public async Task žaneta(CommandContext e)
        {
            await connectToVoiceChannel(e);
            await CreateMessage(e, color: DiscordColor.Green, desc: "Hej Žaneta https://youtu.be/jtdJAnZNDto", imageurl: "https://i.ytimg.com/vi/jtdJAnZNDto/hqdefault.jpg?sqp=-oaymwEXCPYBEIoBSFryq4qpAwkIARUAAIhCGAE=&rs=AOn4CLB8rIsvdWdSN67GmbM48erLIuvjbQ");
            await SetSpeaking(e, true);
            await music(e, "M:/Downloaded/zaneta.mp3");

        }
        [Command("tttie"), Aliases("zeleny", "zelenyony"), Hidden, Description("TTtie")]
        public async Task tttie(CommandContext e, string args = "random")
        {
            string[] nekvalita = { "nekvalitny", "nekvalitni", "nekvalitní", "nekvalitný", "NEKVALITNÝ", "NEKVALITNÍ", "NEKVALITNY", "NEKVALITNI" };
            string[] kvalita = { "kvalitny", "kvalitni", "kvalitní", "kvalitný", "KVALITNÝ", "KVALITNÍ", "KVALITNY", "KVALITNI" };
            bool finished = false;
            Random num = new Random();
            if (args == "random")
            {
                if (num.Next(0, 2) == 1)
                {
                    await e.RespondWithFileAsync("tttie.png");
                }
                else
                {
                    await e.RespondWithFileAsync("tttie2.png");
                }
            }
            else
            {
                foreach (string name in nekvalita)
                {
                    if (!finished)
                    {
                        if (args == name)
                        {
                            await e.RespondWithFileAsync("tttie.png");
                        }
                    }
                }
                foreach (string name in kvalita)
                {
                    if (!finished)
                    {
                        if (args == name)
                        {
                            await e.RespondWithFileAsync("tttie2.png");
                        }
                    }
                }
            }
        }
        #endregion
        #region compress command
        [Command("compress"), Description("If you specify value from 0 to 100 then the bot compresses to the value. 0 awful | 100 great")]
        public async Task compress(CommandContext e, [RemainingText]string args = "5")
        {
            File.Delete("temp.jpg");
            string url;
            int quality;
            string attachementURL = null;
            try
            {
                attachementURL = e.Message.Attachments[0].Url;
            }
            catch
            {
                attachementURL = null;
            }
            if (attachementURL == null)
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
                    if (args.StartsWith("-"))
                    {

                        await CreateMessage(e, desc: "no shit sherlock!", color: DiscordColor.Red);
                        return;
                    }
                    else
                    {

                        quality = Int32.Parse(args.Remove(1, args.Length - 1));
                    }

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
            string size;
            if (info2.Length / 1024 == 0)
            {
                size = info2.Length + "B";
            }
            else
            {
                size = info2.Length / 1024 + "KB";
            }
            await CreateMessage(e, color: DiscordColor.Green, desc: "Here is your fresh compressed art. We know that it is delicious!\nStats:\nSize before compressing: " + (info1.Length / 1024) + "KB\nSize after compressing: " + size);
        }
        #endregion
        #region message command
        [Command("message")]
        public async Task message(CommandContext e, params string[] args)
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

            string words = "";
            int count = 0;
            int newlines = 0;
            foreach (string word in args)
            {
                count++;
                if (count > 7)
                {
                    count = 0;
                    newlines++;
                    words = words + "\n" + word;
                }
                else
                {
                    words = words + " " + word;
                }
            }
            var test = new MagickImage("template.png");
            using (var image = new MagickImage(new MagickColor("#36393E"), 512, 82 + (20 * newlines)))
            {
                MagickGeometry size = new MagickGeometry(512, 82 + (20 * newlines));
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.Composite(test);

                new Drawables()
                  .FontPointSize(16.5)
                  .Font("Fontaria") //secret font..
                  .FillColor(MagickColors.White)
                  .TextAlignment(TextAlignment.Left)
                  .Text(87, 59, words)

                  .FontPointSize(12)
                  .FillColor(MagickColor.FromRgb(85, 87, 92))
                  .Text(134, 35, "Today at " + string.Format("{0:hh:mm tt}", DateTime.Now))
                  .Draw(image);
                image.Write("message.png");
            }
            await e.RespondWithFileAsync("message.png");
        }
        #endregion
        #region emoji command
        [Command("emoji")]
        public async Task emoji(CommandContext e, [RemainingText]string args)
        {
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            Font font = new Font("Segoe UI Emoji", 64);
            SizeF textSize = drawing.MeasureString(args, font);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(Color.FromArgb(53, 57, 62));
            Brush textBrush = new SolidBrush(Color.White);
            drawing.DrawString(args, font, textBrush, 0, 0);

            drawing.Save();
            img.Save("emoji.png");

            textBrush.Dispose();
            drawing.Dispose();
            await e.RespondWithFileAsync("emoji.png");

        }
        #endregion
        #region screenshot command
        [Command("screenshot"), Description("screenshots site")]
        public async Task screenshot(CommandContext e, string args)
        {

            Process process = new Process();
            var startInfo = new ProcessStartInfo
            {
                FileName = "SiteShoter.exe",
                Arguments = $"/URL " + args + " /Filename \"screenshot.png\" /DisableScrollBars 1 /BrowserAutoSize 1 /MaxBrowserWidth 1280 /MaxBrowserHeight 20000",
                UseShellExecute = false
            };
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;
            process.Start();
            process.Exited += async delegate
            {
                await e.RespondWithFileAsync("screenshot.png");
            };
        }
        #endregion
        #region anime command
        [Command("top10animedeaths"), Aliases("anime"), Description("meme generator")]
        public async Task anime(CommandContext e, [RemainingText]string args)
        {
            string attachementURL = null;
            try
            {
                attachementURL = e.Message.Attachments[0].Url;
            }
            catch
            {
                attachementURL = null;
            }
            if (attachementURL == null)
            {
                HttpClient imgdown = new HttpClient();
                HttpResponseMessage imgrespons = await imgdown.GetAsync(args);
                using (FileStream fs = new FileStream("tempanime.jpg", FileMode.Create))
                {
                    await imgrespons.Content.CopyToAsync(fs);
                }

            }
            else
            {
                HttpClient cl = new HttpClient();
                HttpResponseMessage response = await cl.GetAsync(e.Message.Attachments[0].Url);
                using (FileStream fs = new FileStream("tempanime.jpg", FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }

            }
            Image anime = Bitmap.FromFile("anime.png");
            Graphics g = Graphics.FromImage(anime);
            Bitmap new_img = new Bitmap("tempanime.jpg");
            g.DrawImage(new_img, new Rectangle(5, 7, 854, 480));

            //Image Watermarked = WatermarkIt(anime);
            anime.Save("anime.jpg");
            await e.RespondWithFileAsync("anime.jpg");
            new_img.Dispose();
            g.Dispose();
            anime.Dispose();
        }
        #endregion
        #region age command
        [Command("age"), Aliases("botage", "agebot", "ageofbot")]
        public async Task age(CommandContext e)
        {
            LastCommitFromGitHub();
            await CreateMessage(e, desc: "**Age of bot(since first commit on GitHub | 1 October 2017):** " + CalculateAge("01 October 2017 3:05:31 PM") +
                "\n\n**Age of last commit:** " + CalculateAge(commitDate) +
                "\n**Name of last commit:** " + lastCommit);
        }
        #endregion
        #region user command
        [Command("user")]
        public async Task user(CommandContext e, DiscordMember user)
        {
            await e.RespondAsync(user.ToString() + user.Mention);
        }
        #endregion

        //VoiceNext Extension Commands!
        #region voice channel join command
        [Command("join"), Aliases("vchjoin", "voicechanneljoin", "voicejoin", "channeljoin", "voicechjoin"), Description("Joins a voice channel.")]
        public async Task connectToVoiceChannel(CommandContext e)
        {
            DiscordChannel chn = null;


            var vstat = e.Member?.VoiceState;
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

            if (chn == null)
                chn = vstat.Channel;
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
                    await CreateMessage(e, desc: $"File `{filename}` does not exist!");
                    return;
                }
                await CreateMessage(e, desc: $"Playing: `{filename}`");
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
                await CreateMessage(e, color: DiscordColor.Green, desc: "Letters Remaining: " + (200 - text.Remove(0, 1).Length) + "\n\nSpeaking: `" + speakdata.Remove(0, 3) + "`");
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

                    await CreateMessage(e, color: DiscordColor.Green, desc: "Letters Remaining: " + (200 - text.Remove(0, 1).Length) + "\n\nSpeaking: `" + speakdata.Remove(0, 3) + "`");
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
        #region lookatthisdude command
        [Command("lookatthisdude"), Aliases("dude", "lookatdude", "latd", "smiech"), Description("Plays the guy that was laughing.")]
        public async Task latd(CommandContext e)
        {
            await connectToVoiceChannel(e);
            await CreateMessage(e, imageurl: "http://ww2.hdnux.com/photos/51/63/45/10959205/17/920x920.jpg", color: DiscordColor.Green);
            await SetSpeaking(e, true);
            await music(e, "M:/Downloaded/LookAtThisDude.mp3");

        }
        #endregion
    }
}