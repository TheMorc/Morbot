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
using System.Drawing;
using static DSharpPlus.Entities.DiscordEmbedBuilder;
using System.Globalization;
using DSharpPlus;
using System.Net;

namespace Morbot
{
    public class Commands : BaseCommandModule
    {
        string linkname = null;
        string link = null;
        string lastCommit = null;
        string commitDate = null;
        static readonly string embed_title = "Morbot [ver: " + Program.version + ", Made in 🇸🇰, By: Morc]";
        public static readonly string error_message = "**:no_entry: |  An exception occurred!!!  | :no_entry:**\n";
        public static readonly string processing_message = "**Morbot is now processing your request. Please Wait!!**\n";
        public static readonly string Welcome_on_server = "Welcome on our server!";
        public static readonly string You_will_be_missed = "You will be missed. :(";
        public static DSharpPlus.Entities.Optional<DiscordEmbed> embed = new DSharpPlus.Entities.Optional<DiscordEmbed>(embedBuilder);
        static private DiscordActivity botActivity = new DiscordActivity();
        DiscordActivity temp = new DiscordActivity("temp", ActivityType.Playing);
        static DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
        static DiscordMessage msg = null;
        public static System.Reflection.Assembly assembl = System.Reflection.Assembly.GetExecutingAssembly();
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
        public string Linkify(string v)
        {
            bool https = false;
            bool http = false;
            if (v.Contains("https://"))
            {
                https = true;
            }
            if (v.Contains("http://"))
            {
                http = true;
            }

            if (http && !https)
            {
                return v;
            }
            else if (!http && https)
            {
                return v;
            }
            else if (!http && !https)
            {
                return "http://" + v;
            }
            return null;

        }
        public string EmojifyBool(bool bol)
        {
            if (bol)
            {
                return "✅";
            }
            else
            {
                return "❌";
            }
        }
        public string CalculateAge(string date, bool GMT)
        {
            DateTime Now = DateTime.UtcNow;
            DateTime age = Convert.ToDateTime(date);
            if (!GMT)
            {
                Now = DateTime.Now;
            }
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
        public async Task<string> Translate(string text)
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
        public static async Task CreateMessage(CommandContext e, string titleurl = null, string imageurl = null, string thumbnailurl = null, string url = null, string desc = "", string title = "", EmbedAuthor author = null, EmbedFooter footer = default(EmbedFooter), DiscordColor color = default(DiscordColor), bool sendToUser = false)
        {
            var AUTHR = new EmbedAuthor
            {
                IconUrl = "https://github.com/TheMorc/imgs/blob/master/Morbot_evenSmaller.png?raw=true",
                Name = embed_title,
                Url = "https://github.com/TheMorc/Morbot/"
            };
            var FOOTR = new EmbedFooter
            {
                IconUrl = e.Member.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 128),
                Text = "Executed by: " + e.Member.DisplayName
            };
            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":bot:"));
            embedBuilder = new DiscordEmbedBuilder
            {
                Title = title,
                Color = color,
                Description = desc,
                ImageUrl = imageurl,
                ThumbnailUrl = thumbnailurl,
                Url = url,
                Author = AUTHR,
                Footer = FOOTR,
                Timestamp = DateTime.Now
            };
            if (sendToUser)
                await e.Member.SendMessageAsync("", embed: embedBuilder);
            else
                msg = await e.RespondAsync("", embed: embedBuilder);
        }
        public static async Task EditMessage(CommandContext e, string titleurl = null, string imageurl = null, string thumbnailurl = null, string url = null, string desc = "", string title = "", EmbedAuthor author = null, EmbedFooter footer = default(EmbedFooter), DiscordColor color = default(DiscordColor))
        {
            var AUTHR = new EmbedAuthor
            {
                IconUrl = "https://github.com/TheMorc/imgs/blob/master/Morbot_evenSmaller.png?raw=true",
                Name = embed_title,
                Url = "https://github.com/TheMorc/Morbot/"
            };
            var FOOTR = new EmbedFooter
            {
                IconUrl = e.Member.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 128),
                Text = "Executed by: " + e.Member.DisplayName
            };
            embedBuilder = new DiscordEmbedBuilder
            {
                Title = title,
                Color = color,
                Description = desc,
                ImageUrl = imageurl,
                ThumbnailUrl = thumbnailurl,
                Url = url,
                Author = AUTHR,
                Footer = FOOTR,
                Timestamp = DateTime.Now
            };
            embed = new DSharpPlus.Entities.Optional<DiscordEmbed>(embedBuilder);
            await msg.ModifyAsync(embed: embed);
        }
        public static async Task EditMessageSlim(CommandContext e)
        {
            embed = new DSharpPlus.Entities.Optional<DiscordEmbed>(embedBuilder);
            await msg.ModifyAsync(embed: embed);
        }
        public static async Task DeleteMessage(CommandContext e)
        {
            await msg.DeleteAsync("Deleted by command in bots code");
        }
        private async Task Music(CommandContext e, string v, double speed = 1.0)
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
        public static async Task CreateHelloImage(DiscordMember member, string text)
        {
            HttpClient cl = new HttpClient();
            HttpResponseMessage response = await cl.GetAsync(member.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256));
            using (FileStream fs = new FileStream("Pictures/avatar.png", FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }

            using (MagickImage image = new MagickImage(@"Pictures/avatar.png"))
            {
                image.Alpha(AlphaOption.Set);
                using (IMagickImage clone = image.Clone())
                {
                    clone.Distort(DistortMethod.DePolar, 0);
                    clone.VirtualPixelMethod = VirtualPixelMethod.HorizontalTile;
                    clone.BackgroundColor = MagickColors.None;
                    clone.Distort(DistortMethod.Polar, 0);
                    image.Composite(clone, CompositeOperator.DstIn);
                    image.Trim();
                    image.RePage();
                    image.Resize(256, 256);
                    using (MagickImage hello = new MagickImage("Pictures/hello_template.png"))
                    {
                        new Drawables()
                          .Composite(127, 4, image)

                          .FillColor(MagickColors.White)
                          .Font("FontariaBOLD")
                          .TextAlignment(TextAlignment.Left)
                          .FontPointSize(28)
                          .Text(10, 368, text)

                          .FontPointSize(25)
                          .TextAlignment(TextAlignment.Center)
                          .Text(256, 288, member.DisplayName)

                          .Font("Fontaria")
                          .FontPointSize(20)
                          .FillColor(MagickColor.FromRgb(196, 206, 239))
                          .Text(256, 312, member.Username + "#" + member.Discriminator)

                          .Draw(hello);
                        hello.Write("Pictures/morbot_image.png");
                    }
                }
            }
        }
        #endregion
        #region servers command
        [Command("servers"), RequireOwner, Hidden, Description("Should be hidden! But this command shows list of servers that it is on...")]
        public async Task Servers(CommandContext ex)
        {
            string serverlist = null;
            foreach (DiscordGuild server in ex.Client.Guilds.Values.Select(e => e))
            {

                serverlist = serverlist + "**" + server.Name + "** | " + server.Id + " | " + server.Owner + "\n\n";
            }
            await CreateMessage(ex, desc: "**Servers:**\n" + serverlist, color: DiscordColor.Cyan);
        }
        #endregion


        //test purpose command
        #region test command
        [Command("test")]
        public async Task Test(CommandContext ex, DiscordEmoji emo)
        {
            //if (member == null)
            //{
            //    member = ex.Member;
            //}
            //await CreateHelloImage(member, You_will_be_missed);
            //await ex.RespondWithFileAsync("morbot_image.png");
            await ex.RespondAsync(emo.Url);
        }
        #endregion

        //Commands
        #region bot command
        [Command("bot")]
        public async Task Bot(CommandContext e)
        {
            await e.TriggerTypingAsync();
            LastCommitFromGitHub();
            //await CreateMessage(e, color: DiscordColor.Blurple,
            //desc: "**Morbot** is OpenSource bot maintained by **Morc** and **Made in Slovakia** with D#+ (DSharpPlus) API." +
            //"\nAPI Version: `" + e.Client.VersionString +
            //"`.\n\n**Age of Bot(since first commit on GitHub | 1 October 2017):** " + CalculateAge("01 October 2017 3:05:31 PM") +
            //"\n\n**Age of last commit:** " + CalculateAge(commitDate) +
            //"\n**Name of last commit:** " + lastCommit +
            //"\n\n**Bot Source Code:** \nhttps://www.github.com/TheMorc/Morbot " +
            //"\n\n**D#+ GitHub:** \nhttps://github.com/NaamloosDT/DSharpPlus");
            await CreateMessage(e, color: DiscordColor.Green, desc: "**Morbot** is OpenSource bot maintained by **Morc** and **Made in Slovakia** with **D#+ (DSharpPlus) library.**");
            embedBuilder.AddField("**Morbot Version:**", Program.version, true);
            embedBuilder.AddField("**D#+ Library Version:**", e.Client.VersionString, true);
            embedBuilder.AddField("**Age of Morbot(since first commit on GitHub | 1 October 2017):**", CalculateAge("01 October 2017 3:05:31 PM", false), true);
            embedBuilder.AddField("**Age of last commit:**", CalculateAge(commitDate, true), true);
            embedBuilder.AddField("**Name of last commit:**", lastCommit, false);
            embedBuilder.AddField("**Morbot Source Code on Github:**", "https://www.github.com/TheMorc/Morbot", true);
            embedBuilder.AddField("**D#+ on GitHub:**", "https://github.com/NaamloosDT/DSharpPlus", true);
            await EditMessageSlim(e);
        }
        #endregion
        #region love command
        [Command("love"), Description("Who needs command that generates random number and sends it?? ANYONE! but this is great example for array string command!!")]
        public async Task Love(CommandContext e, params string[] name)
        {
            await e.TriggerTypingAsync();
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
                        laav = $"**{name[0]}** + **{name[1]}** = **100%** {loveemoji}";
                        passed = true;
                    }
                    else
                    {
                        laav = $"**{name[0]}** + **{name[1]} ** = **{num}%** {loveemoji}";
                        passed = true;
                    }
                }

            }
            //await CreateMessage(e, desc: laav, color: laavcolor);
            await CreateMessage(e, desc: $"**{name[0]}**", color: DiscordColor.Blurple);
            await Task.Delay(150);
            await EditMessage(e, desc: $"**{name[0]}** + ", color: DiscordColor.Blurple);
            await Task.Delay(150);
            await EditMessage(e, desc: $"**{name[0]}** + **{name[1]}**", color: DiscordColor.Blurple);
            await Task.Delay(150);
            await EditMessage(e, desc: $"**{name[0]}** + **{name[1]}** = ", color: DiscordColor.Blurple);
            await Task.Delay(2500);
            await EditMessage(e, desc: laav, color: laavcolor);
        }
        #endregion
        #region ping command
        [Command("ping"), Description("What simpler than this! This sends ping in miliseconds!")]
        public async Task Ping(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: $"**Ping:** {e.Client.Ping}ms", color: DiscordColor.Green);
        }
        #endregion
        #region latestvideo command
        [Command("latestvideo"), Aliases("latestmorcvideo", "morcvideo", "lastvideo", "lastvideobymorc"), Description("This command pulls link of last video posted by Morc")]
        public async Task Latestvideo(CommandContext e)
        {
            await e.TriggerTypingAsync();
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
                    await e.Message.RespondAsync(ytlink);
                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
            }
        }
        #endregion
        #region weather command
        [Command("weather"), Description("Bot responds with actual temperature in °C.Weather gets pulled from OpenWeather and city is Topoľčany(small town near village Biskupová where Morc lives).")]
        public async Task CWeather(CommandContext e, [RemainingText]string town = "Topolcany")
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            string data = "";
            string weathertype = null;
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
                double celsius = Math.Round(oRootObject.main.temp - 273.15, 1);
                double fahrenheit = Math.Round((oRootObject.main.temp - 273.15) * 1.8 + 32, 1);
                if (oRootObject.weather[0].description == "clear sky")
                {
                    weathertype = ":sunny: - **Sunny**";
                    wcolor = DiscordColor.Yellow;
                }
                if (oRootObject.weather[0].description == "broken clouds")
                {
                    weathertype = ":cloud: - **Clouds**";
                    wcolor = DiscordColor.Gray;
                }
                if (oRootObject.weather[0].description == "few clouds")
                {
                    weathertype = ":cloud: - **Clouds**";
                    wcolor = DiscordColor.Gray;
                }
                if (oRootObject.weather[0].description == "overcast clouds")
                {
                    weathertype = ":cloud: - **Clouds**";
                    wcolor = DiscordColor.Gray;
                }
                if (oRootObject.weather[0].description == "light rain")
                {
                    weathertype = ":cloud_rain: - **Rain**";
                    wcolor = DiscordColor.Cyan;
                }

                if (oRootObject.weather[0].description == "mist")
                {
                    weathertype = ":fog: - **Fog/Mist**";
                    wcolor = DiscordColor.Cyan;
                }
                await EditMessage(e, desc: $"**{oRootObject.name}** - {oRootObject.sys.country}\n\n**Temperature in °C:**\n**Temperature in °F:**\n", color: wcolor);
                await Task.Delay(500);
                await EditMessage(e, desc: $"**{oRootObject.name}** - {oRootObject.sys.country}\n\n**Temperature in °C:** {celsius}°C \n**Temperature in °F:** {fahrenheit}°F\n{weathertype}", color: wcolor);
            }

        }
        #endregion
        #region randomnorrisjoke command
        [Command("randomnorrisjoke"), Aliases("norris", "norrisjoke", "chucknorris", "chuck", "chuckjoke", "randomchuckjoke"), Description("Chuck Norris was born earlier than he died! Command pulls random joke from ChuckNorris API..")]
        public async Task ChuckNorris(CommandContext e, string language = "")
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
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
                    await EditMessage(e, title: "Chuck Norris joke:", desc: chuck.value, thumbnailurl: chuck.icon_url, url: "https://api.chucknorris.io", color: DiscordColor.Green);
                }
                else
                {

                    await EditMessage(e, title: "Chuck Norris joke in English:", desc: chuck.value, thumbnailurl: chuck.icon_url, url: "https://api.chucknorris.io", color: DiscordColor.Green);
                    string translation = await Translate(language + " " + chuck.value);
                    await CreateMessage(e, title: $"Chuck Norris joke in {translation.Remove(9, translation.Length - 9)}:", desc: translation.Remove(0, 9), thumbnailurl: chuck.icon_url, url: "https://api.chucknorris.io", color: DiscordColor.Green);
                }
            }
        }
        #endregion
        #region translate command
        [Command("translate"), Description("eee what else? one! what else? two what else?? ..... translate command!!")]
        public async Task Translate(CommandContext e, params string[] args)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
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
                    await EditMessage(e, desc: langstring, color: DiscordColor.Green);
                }
            }
            else
            {
                string text = "";
                foreach (string arg in args)
                {
                    text = text + " " + arg;
                }
                string result = await Translate(text);
                await EditMessage(e, desc: result.Remove(0, 9));

            }
        }
        #endregion
        #region time command
        [Command("time"), Description("WHO WANTS THIS COMMAND???")]
        public async Task Time(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            string minutes = null;
            if (DateTime.Now.TimeOfDay.Minutes.ToString().Length == 1)
            {
                minutes = "0" + DateTime.Now.TimeOfDay.Minutes.ToString();
            }
            else
            {
                minutes = DateTime.Now.TimeOfDay.Minutes.ToString();
            }
            await EditMessage(e, desc: $"Morc's time zone is UTC+01:00 so the time is: {DateTime.Now.TimeOfDay.Hours.ToString()}:{minutes}", color: DiscordColor.Orange);
        }
        #endregion
        #region randomwindows command
        string[] versions = { "w95.png", "w98.png", "wme.png", "w2k.png", "w7.png", "wvista.png", "wxp.png", "w8.png", "w10.png" };
        [Command("randomwindows"), Aliases("randwind", "ranwin", "randomwin", "rwin", "randomw", "randw"), Description("Command sends random picture(Windows 95/98/ME/2000/XP/Vista/7/8/10).")]
        public async Task RandomWindows(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            Random rnd = new Random();
            string ver = $"WinPics/{versions[rnd.Next(0, versions.Length)]}";
            await DeleteMessage(e);
            await e.Message.RespondWithFileAsync(ver);
        }
        #endregion
        #region cat command
        [Command("meow"), Aliases("cat", "kitty", "catpicture", "meov", "mjau"), Description("Command cat and dog does the same thing. Sends cute picture/gif of cat or dog!")]
        public async Task Meow(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://aws.random.cat/meow");
                var pData = JObject.Parse(data);
                url = pData["file"].ToString();
                await EditMessage(e, thumbnailurl: "https://purr.objects-us-west-1.dream.io/static/img/random.cat-logo.png", imageurl: url, color: DiscordColor.Green);
            }
        }
        #endregion
        #region dog command
        [Command("woof"), Aliases("dog", "puppy", "dogpicture", "hau", "haw"), Description("Command cat and dog does the same thing. Sends cute picture/gif of cat or dog!")]
        public async Task Woof(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://random.dog/woof.json");
                var pData = JObject.Parse(data);
                url = pData["url"].ToString();
                await EditMessage(e, imageurl: url, color: DiscordColor.Green);
            }
        }
        #endregion
        #region mode command
        [Command("changemode"), Aliases("mode"), Description("This command changes mode of bot.(Playing,Streaming,Watching,Listening To)")]
        public async Task Mode(CommandContext e, string[] mode)
        {
            await e.TriggerTypingAsync();
            bool finished = false;
            string[] streaming = { "streaming", "Streaming", "Stream", "stream" };
            string[] playing = { "playing", "Playing", "Play", "play" };
            string[] watching = { "watching", "Watching", "Watch", "watch" };
            string[] listening = { "listeningto", "Listeningto", "Listen", "listen", "listento", "Listento", "Listen to", "listen to", "listening to", "Listening to" };

            foreach (string name in streaming)
            {
                if (!finished)
                {
                    if (mode[0] == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.Streaming;
                        botActivity.StreamUrl = mode[1].Remove(0, name.Length);
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: $"Sucessfully changed mode to `Streaming` by: {e.Member.Mention}");
                    }
                }
            }
            foreach (string name in playing)
            {
                if (!finished)
                {
                    if (mode[0] == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.Playing;
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: $"Sucessfully changed mode to `Playing` by: {e.Member.Mention}");
                    }
                }
            }
            foreach (string name in watching)
            {
                if (!finished)
                {
                    if (mode[0] == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.Watching;
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: $"Sucessfully changed mode to `Watching` by: {e.Member.Mention}");
                    }
                }
            }
            foreach (string name in listening)
            {
                if (!finished)
                {
                    if (mode[0] == name)
                    {
                        finished = true;
                        await e.Client.UpdateStatusAsync(temp);
                        botActivity.Name = Program.DiscordActivityText;
                        botActivity.ActivityType = ActivityType.ListeningTo;
                        await e.Client.UpdateStatusAsync(botActivity);
                        await CreateMessage(e, color: DiscordColor.Green, desc: $"Sucessfully changed mode to `Listening To` by: {e.Member.Mention}");
                    }
                }
            }
            if (!finished)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: $"You Specified: {mode}\nBut it looks like you specified the wrong bot mode. :joy:");
            }
        }
        #endregion
        #region gif command
        [Command("gif"), Description("If anything isnt specified with command, then it responds with random dog or cat(dog or cat randomized by bot) gif(by giphy).")]
        public async Task GIFSearch(CommandContext e, [RemainingText]string arg1 = "")
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
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
                    gifby = $"{Emoji[Rand.Next(0, Emoji.Length)]} By: {oRootObject.data.username}";
                }
                await EditMessage(e, thumbnailurl: "https://www.inboxsdk.com/images/logos/giphy.png", desc: gifby, imageurl: gifurl, color: DiscordColor.Green);
            }


        }
        #endregion
        #region picture command
        [Command("picture"), Aliases("pic", "pix", "image", "img", "photo"), Description("This command works only if something is specified for ex 'Zetor' then it sends picture of zetor tractor..")]
        public async Task IMGSearch(CommandContext e, [RemainingText]string arg1 = "")
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            Random rand = new Random();
            string page = $"https://pixabay.com/api/?key=" + Program.configuration.PixabayAPIKey + "&q=" + arg1 + "&image_type=photo";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                string data = await content.ReadAsStringAsync();
                JSONs.GIFRootObject oRootObject = new JSONs.GIFRootObject();
                oRootObject = JsonConvert.DeserializeObject<JSONs.GIFRootObject>(data);
                int num = rand.Next(0, oRootObject.hits.Capacity);
                await EditMessage(e, desc: $"Photo by: {oRootObject.hits[num].user}\nViews: {oRootObject.hits[num].views}", imageurl: oRootObject.hits[num].webformatURL, thumbnailurl: "https://pixabay.com/static/img/logo_square.png");
            }
        }
        #endregion
        #region hidden commands
        [Command("žaneta"), Aliases("zani", "žani", "hulvat"), Hidden, Description("sings section of hej žaneta song, text arrangement by crafty ")]
        public async Task Žaneta(CommandContext e)
        {
            await ConnectToVoiceChannel(e);
            await CreateMessage(e, color: DiscordColor.Green, desc: "Hej Žaneta https://youtu.be/jtdJAnZNDto", imageurl: "https://i.ytimg.com/vi/jtdJAnZNDto/hqdefault.jpg?sqp=-oaymwEXCPYBEIoBSFryq4qpAwkIARUAAIhCGAE=&rs=AOn4CLB8rIsvdWdSN67GmbM48erLIuvjbQ");
            await SetSpeaking(e, true);
            await Music(e, "Songs/zaneta.mp3");

        }
        [Command("tttie"), Aliases("zeleny", "zelenyony"), Hidden, Description("TTtie")]
        public async Task TTtie(CommandContext e, string args = "random")
        {
            await e.TriggerTypingAsync();
            string[] nekvalita = { "nekvalitny", "nekvalitni", "nekvalitní", "nekvalitný" };
            string[] kvalita = { "kvalitny", "kvalitni", "kvalitní", "kvalitný" };
            bool finished = false;
            Random num = new Random();
            if (args == "random")
            {
                if (num.Next(0, 2) == 1)
                {
                    await e.RespondWithFileAsync("Pictures/tttie.png");
                }
                else
                {
                    await e.RespondWithFileAsync("Pictures/tttie2.png");
                }
            }
            else
            {
                foreach (string name in nekvalita)
                {
                    if (!finished)
                    {
                        if (args.ToLower() == name)
                        {
                            await e.RespondWithFileAsync("Pictures/tttie.png");
                        }
                    }
                }
                foreach (string name in kvalita)
                {
                    if (!finished)
                    {
                        if (args.ToLower() == name)
                        {
                            await e.RespondWithFileAsync("Pictures/tttie2.png");
                        }
                    }
                }
            }
        }
        #endregion
        #region compress command
        [Command("compress"), Description("If you specify value from 0 to 100 then the bot compresses to the value. 0 awful | 100 great")]
        public async Task Compress(CommandContext e, [RemainingText]string args = "5")
        {
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            await e.TriggerTypingAsync();
            File.Delete("Pictures/temp.jpg");
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

                        await EditMessage(e, desc: "no shit sherlock!", color: DiscordColor.Red);
                        return;
                    }
                    else
                    {

                        quality = Int32.Parse(args.Remove(1, args.Length - 1));
                    }

                }
                HttpClient imgdown = new HttpClient();
                HttpResponseMessage imgrespons = await imgdown.GetAsync(url);
                using (FileStream fs = new FileStream("Pictures/temp.jpg", FileMode.Create))
                {
                    await imgrespons.Content.CopyToAsync(fs);
                }

            }
            else
            {
                quality = Int32.Parse(args);
                HttpClient cl = new HttpClient();
                HttpResponseMessage response = await cl.GetAsync(e.Message.Attachments[0].Url);
                using (FileStream fs = new FileStream("Pictures/temp.jpg", FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }

            }
            File.Delete("Pictures/compressed.jpg");

            using (var img = new MagickImage("Pictures/temp.jpg"))
            {
                img.Strip();
                img.Quality = quality;
                img.Write("Pictures/compressed.jpg");
            }
            await e.RespondWithFileAsync("Pictures/compressed.jpg");
            FileInfo info1 = new FileInfo("Pictures/temp.jpg");
            FileInfo info2 = new FileInfo("Pictures/compressed.jpg");
            string size;
            if (info2.Length / 1024 == 0)
            {
                size = info2.Length + "B";
            }
            else
            {
                size = info2.Length / 1024 + "KB";
            }
            await EditMessage(e, color: DiscordColor.Green, desc: $"Here is your fresh compressed art. We know that it is delicious!\nStats:\nSize before compressing: {(info1.Length / 1024)}KB\nSize after compressing: {size}");
        }
        #endregion
        #region message command
        [Command("message")]
        public async Task Message(CommandContext e, params string[] args)
        {
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            await e.TriggerTypingAsync();
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
            var test = new MagickImage("Pictures/template.png");
            using (var image = new MagickImage(new MagickColor("#36393E"), 512, 82 + (20 * newlines)))
            {
                MagickGeometry size = new MagickGeometry(512, 82 + (20 * newlines))
                {
                    IgnoreAspectRatio = true
                };
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
                  .Text(134, 35, $"Today at {string.Format("{0:hh:mm tt}", DateTime.Now)}")
                  .Draw(image);
                image.Write("Pictures/message.png");
            }
            await DeleteMessage(e);
            await e.RespondWithFileAsync("Pictures/message.png");
        }
        #endregion
        #region emoji command
        [Command("emoji")]
        public async Task Emoji(CommandContext e, [RemainingText]string args)
        {
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
            await e.TriggerTypingAsync();
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            Font font = new Font("Segoe UI Emoji", 128);
            SizeF textSize = drawing.MeasureString(args, font);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(Color.FromArgb(53, 57, 62));
            Brush textBrush = new SolidBrush(Color.White);
            drawing.DrawString(args, font, textBrush, 0, 0);

            drawing.Save();
            img.Save("Pictures/emoji.png");

            textBrush.Dispose();
            drawing.Dispose();
            await DeleteMessage(e);
            await e.RespondWithFileAsync("Pictures/emoji.png");

        }
        #endregion
        #region anime command
        [Command("top10animedeaths"), Aliases("anime"), Description("meme generator")]
        public async Task Anime(CommandContext e, [RemainingText]string args)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
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
                using (FileStream fs = new FileStream("Pictures/tempanime.jpg", FileMode.Create))
                {
                    await imgrespons.Content.CopyToAsync(fs);
                }

            }
            else
            {
                HttpClient cl = new HttpClient();
                HttpResponseMessage response = await cl.GetAsync(e.Message.Attachments[0].Url);
                using (FileStream fs = new FileStream("Pictures/tempanime.jpg", FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }

            }
            Image anime = Bitmap.FromFile("Pictures/anime.png");
            Graphics g = Graphics.FromImage(anime);
            Bitmap new_img = new Bitmap("Pictures/tempanime.jpg");
            g.DrawImage(new_img, new Rectangle(5, 7, 854, 480));

            //Image Watermarked = WatermarkIt(anime);
            anime.Save("Pictures/anime.jpg");
            await DeleteMessage(e);
            await e.RespondWithFileAsync("Pictures/anime.jpg");
            new_img.Dispose();
            g.Dispose();
            anime.Dispose();
        }
        #endregion
        #region age command
        [Command("age"), Aliases("botage", "agebot", "ageofbot")]
        public async Task Age(CommandContext e)
        {
            await e.TriggerTypingAsync();
            LastCommitFromGitHub();
            await CreateMessage(e, color: DiscordColor.Green, desc: "Age of **Morbot**.**");
            embedBuilder.AddField("**Age of Morbot(since first commit on GitHub | 1 October 2017):**", CalculateAge("01 October 2017 3:05:31 PM", false), true);
            embedBuilder.AddField("**Age of last commit:**", CalculateAge(commitDate, true), true);
            embedBuilder.AddField("**Name of last commit:**", lastCommit, false);
            await EditMessageSlim(e);
        }
        #endregion
        #region user command
        [Command("user")]
        public async Task User(CommandContext e, DiscordMember member = null)
        {
            string roles = null;
            await e.TriggerTypingAsync();
            if (member == null)
            {
                member = e.Member;
            }
            foreach (DiscordRole role in member.Roles)
            {
                roles = roles + "`" + role.Name + "` **&** ";
            }
            await CreateMessage(e, imageurl: member.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 128), color: DiscordColor.Green, desc: "**Info about user:** " + member.Mention);
            embedBuilder.AddField("**Bot:**", EmojifyBool(member.IsBot), true);
            embedBuilder.AddField("**Owner:**", EmojifyBool(member.IsOwner), true);
            embedBuilder.AddField("**Muted:**", EmojifyBool(member.IsMuted), true);
            try
            {
                embedBuilder.AddField("**Voice - Channel:**", "**Connected to:** `" + member.VoiceState.Channel.Name + "`", true);
            }
            catch { }
            try
            {
                embedBuilder.AddField("**Voice - Server Muted:**", EmojifyBool(member.VoiceState.IsServerMuted), true);
            }
            catch { }
            try
            {
                embedBuilder.AddField("**Voice - Self Muted:**", EmojifyBool(member.VoiceState.IsSelfMuted), true);
            }
            catch { }
            embedBuilder.AddField("**Name on " + e.Guild.Name + ":**", member.DisplayName, false);
            embedBuilder.AddField("**Full username:**", member.Username + "#" + member.Discriminator, true);
            embedBuilder.AddField("**Roles:**", roles.Remove(roles.Length - 6, 6), true);
            embedBuilder.AddField("**Joined Discord:**", member.CreationTimestamp.DateTime.ToString(CultureInfo.CreateSpecificCulture("cs")), false);
            embedBuilder.AddField("**Joined " + e.Guild.Name + ":**", member.JoinedAt.DateTime.ToString(CultureInfo.CreateSpecificCulture("cs")), false);
            await EditMessageSlim(e);
        }
        #endregion
        #region guild command
        [Command("guild"), Aliases("server")]
        public async Task Guild(CommandContext e)
        {
            DiscordGuild guild = e.Guild;
            string emojis = null;
            string channels = null;
            string members = null;
            string roles = null;
            foreach (DiscordEmoji emoji in guild.Emojis)
            {
                emojis = emojis + DiscordEmoji.FromName(e.Client, ":" + emoji.Name + ":") + " **|** ";
            }
            foreach (DiscordChannel channel in guild.Channels)
            {
                channels = channels + channel.Mention + " **|** ";
            }
            foreach (DiscordMember member in guild.Members)
            {
                members = members + member.Mention + " **|** ";
            }
            foreach (DiscordRole role in guild.Roles)
            {
                roles = roles + "`" + role.Name + "` **|** ";
            }
            await CreateMessage(e, imageurl: guild.IconUrl, color: DiscordColor.Green, desc: "**Info about server/guild:** " + guild.Name);
            embedBuilder.AddField("**Owner:**", guild.Owner.Mention, true);
            embedBuilder.AddField("**Verification Level:**", guild.VerificationLevel.ToString(), true);
            embedBuilder.AddField("**Large:**", EmojifyBool(guild.IsLarge), true);
            embedBuilder.AddField("**Member Count:**", guild.MemberCount.ToString(), true);
            embedBuilder.AddField("**Channel Count:**", guild.Channels.Count.ToString(), true);
            embedBuilder.AddField("**Cust.Emoji Count:**", guild.Emojis.Count.ToString(), true);
            embedBuilder.AddField("**Role Count:**", guild.Roles.Count.ToString(), true);
            embedBuilder.AddField("**Members:**", members.Remove(members.Length - 6, 6), false);
            embedBuilder.AddField("**Channels:**", channels.Remove(channels.Length - 6, 6), false);
            embedBuilder.AddField("**Custom Emojis:**", emojis.Remove(emojis.Length - 6, 6), false);
            embedBuilder.AddField("**Roles:**", roles.Remove(roles.Length - 6, 6), false);
            await EditMessageSlim(e);
        }
        #endregion
        #region bye command
        [Command("bye"), RequireOwner]
        public async Task Bye(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, desc: "Bye Bye, see you later!", color: DiscordColor.Blurple);
            await e.Client.DisconnectAsync();
            Environment.Exit(0);
        }
        #endregion
        #region link commands
        [Command("link"), Aliases("links")]
        public async Task Linkcmd(CommandContext e, string selected_Link = null)
        {
            string links = null;
            if (selected_Link == null)
            {
                await e.TriggerTypingAsync();
                if (!Directory.Exists("links"))
                {
                    Directory.CreateDirectory("links");
                }
                foreach (string file in Directory.EnumerateFiles("links"))
                {
                    links = $"{links} `{file.Remove(0, 6)}` **|**";
                }

                await CreateMessage(e, color: DiscordColor.Green, desc: $"**Links:** {links.Remove(links.Length - 5, 5)}");
            }
            else
            {
                foreach (string file in Directory.EnumerateFiles("links"))
                {
                    if (selected_Link == file.Remove(0, 6))
                    {

                        await CreateMessage(e, color: DiscordColor.Green, desc: $"**Link Name:** `{file.Remove(0, 6)}`\n**Link:** {Linkify(File.ReadAllText(file))}");
                    }
                }
            }
        }


        [Command("setlink"), Aliases("linkset")]
        public async Task Setlink(CommandContext e, string alink = null)
        {
            await e.TriggerTypingAsync();
            if (alink == null)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: "**Link cannot be blank!!!**");
            }
            else
            {
                link = alink;
                await CreateMessage(e, color: DiscordColor.Green, desc: $"**Set link to:** `{link}`");
            }
        }
        [Command("setname"), Aliases("linkname", "setlinkname")]
        public async Task Setnamelink(CommandContext e, string name = null)
        {
            await e.TriggerTypingAsync();
            await e.TriggerTypingAsync();
            if (name == null)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: "**Link name cannot be blank!!!**");
            }
            else
            {
                linkname = name;
                await CreateMessage(e, color: DiscordColor.Green, desc: $"**Set link name to:** `{name}`");
            }
        }
        [Command("savelink"), Aliases("linksave")]
        public async Task Savelink(CommandContext e)
        {
            if (Directory.Exists("links"))
            {
                if (linkname == null)
                {
                    await CreateMessage(e, color: DiscordColor.Red, desc: "**Cannot create link because link name is blank!!!**");
                }
                else if (link == null)
                {
                    await CreateMessage(e, color: DiscordColor.Red, desc: "**Cannot create link because link is blank!!!**");
                }
                else
                {
                    await e.TriggerTypingAsync();
                    using (var StrmWrt = new StreamWriter("links/" + linkname, false))
                    {
                        StrmWrt.WriteLine(link);
                        StrmWrt.Close();
                        await CreateMessage(e, color: DiscordColor.Green, desc: $"**Saved link:** `{linkname}`");
                    }
                }
            }
            else
            {
                Directory.CreateDirectory("links");
            }
        }
        #endregion
        #region fullwidth command
        private System.Collections.Generic.Dictionary<char, char> letters = new System.Collections.Generic.Dictionary<char, char>()
        {
                {'\u0041', '\uff21'},
                {'\u0042', '\uff22'},
                {'\u0043', '\uff23'},
                {'\u0044', '\uff24'},
                {'\u0045', '\uff25'},
                {'\u0046', '\uff26'},
                {'\u0047', '\uff27'},
                {'\u0048', '\uff28'},
                {'\u0049', '\uff29'},
                {'\u004a', '\uff2a'},
                {'\u004b', '\uff2b'},
                {'\u004c', '\uff2c'},
                {'\u004d', '\uff2d'},
                {'\u004e', '\uff2e'},
                {'\u004f', '\uff2f'},
                {'\u0050', '\uff30'},
                {'\u0051', '\uff31'},
                {'\u0052', '\uff32'},
                {'\u0053', '\uff33'},
                {'\u0054', '\uff34'},
                {'\u0055', '\uff35'},
                {'\u0056', '\uff36'},
                {'\u0057', '\uff37'},
                {'\u0058', '\uff38'},
                {'\u0059', '\uff39'},
                {'\u005a', '\uff3a'},
                {'\u0061', '\uff41'},
                {'\u0062', '\uff42'},
                {'\u0063', '\uff43'},
                {'\u0064', '\uff44'},
                {'\u0065', '\uff45'},
                {'\u0066', '\uff46'},
                {'\u0067', '\uff47'},
                {'\u0068', '\uff48'},
                {'\u0069', '\uff49'},
                {'\u006a', '\uff4a'},
                {'\u006b', '\uff4b'},
                {'\u006c', '\uff4c'},
                {'\u006d', '\uff4d'},
                {'\u006e', '\uff4e'},
                {'\u006f', '\uff4f'},
                {'\u0070', '\uff50'},
                {'\u0071', '\uff51'},
                {'\u0072', '\uff52'},
                {'\u0073', '\uff53'},
                {'\u0074', '\uff54'},
                {'\u0075', '\uff55'},
                {'\u0076', '\uff56'},
                {'\u0077', '\uff57'},
                {'\u0078', '\uff58'},
                {'\u0079', '\uff59'},
                {'\u007a', '\uff5a'}
            };
        [Command("fullwidth"), Aliases("fullw", "fwidth")]
        public async Task FWidth(CommandContext e, [RemainingText]string text)
        {
            await e.Message.DeleteAsync();
            string output = "";
            for (var i = 0; i < text.Length; i++)
            {
                char normal = text[i];
                try
                {
                    char latin = letters[normal];
                    output += latin;
                }
                catch
                {
                    output += normal;
                }
            }
            await e.RespondAsync(output);
        }
        #endregion
        #region pc command
        [Command("pc")]
        public async Task PC(CommandContext e, DiscordMember member = null)
        {
            await e.TriggerTypingAsync();
            await CreateMessage(e, color: DiscordColor.Green, desc: "**Morc's PC:**");
            embedBuilder.AddField("**Specifications of:**", "Morc's PC", true);
            embedBuilder.AddField("**Motherboard:**", "Gigabyte Z97-HD3", true);
            embedBuilder.AddField("**CPU:**", "Intel Core i5-4460", true);
            embedBuilder.AddField("**GPU:**", "Asus Turbo GTX 1060(previous was Gigabyte GTX 750)", true);
            embedBuilder.AddField("**RAM:**", "2x8GB Corsair VengeanceLP 1600MHz", true);
            embedBuilder.AddField("**SDD:**", "Kingston SSDNow 120GB", true);
            embedBuilder.AddField("**HDD:**", "old WD 3.5\" 320GB", true);
            embedBuilder.AddField("**FullHD Monitor:**", "LG IPS237", true);
            embedBuilder.AddField("**1280x1024 Monitor:**", "BenQ FP7|G+", true);
            embedBuilder.AddField("**Webcam:**", "Logitech C270", true);
            embedBuilder.AddField("**Audio Mixer:**", "Unomat VP1000(ancient VCR Audio Video Mixer)", true);
            embedBuilder.AddField("**External HDD:**", "1TB WD 3.5\" used as storage + as stand for 1024p Monitor :grin:", true);
            await EditMessageSlim(e);
        }
        #endregion */
        #region decode command
        [Command("decode")]
        public async Task Decode(CommandContext e, [RemainingText]string text)
        {
            await CreateMessage(e, desc: System.Web.HttpUtility.UrlDecode(text));
        }
        #endregion */
        #region encode command
        [Command("encode")]
        public async Task Encode(CommandContext e, [RemainingText]string text)
        {
            await CreateMessage(e, desc: System.Web.HttpUtility.UrlEncode(text));
        }
        #endregion
        #region emoji commands
        [Command("createemoji"), RequireOwner]
        public async Task NitroEmoji(CommandContext e, params string[] text)
        {
            byte[] image = (new WebClient()).DownloadData(text[1]);
            MemoryStream memstrm = new MemoryStream(image);
            await e.Guild.CreateEmojiAsync(text[0], memstrm);
            await CreateMessage(e, desc: $"Emoji :{text[0]}: was created successfully!", color: DiscordColor.Green);
        }
        [Command("sendemoji"), Aliases("sne", "se")]
        public async Task SendEmoji(CommandContext e, string text)
        {
            await e.Message.DeleteAsync();
            await e.RespondAsync(DiscordEmoji.FromName(e.Client, text));
        }
        #endregion

        //VoiceNext Extension Commands!
        #region voice channel join command
        [Command("join"), Aliases("vchjoin", "voicechanneljoin", "voicejoin", "channeljoin", "voicechjoin"), Description("Joins a voice channel.")]
        public async Task ConnectToVoiceChannel(CommandContext e)
        {
            await e.TriggerTypingAsync();
            DiscordChannel chn = null;

            var vstat = e.Member?.VoiceState;
            if (vstat?.Channel == null && chn == null)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: "**You are not in any Voice Channel!**");
                return;
            }

            var vnext = e.Client.GetVoiceNext();
            if (vnext == null)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: error_message + "**VoiceNext is not enabled or configured properly.**");
                return;
            }

            if (chn == null)
                chn = vstat.Channel;
            var vnc = vnext.GetConnection(e.Guild);
            if (vnc == null)
            {
                await CreateMessage(e, color: DiscordColor.Yellow, desc: "**Not connected in this guild. Connecting to user's voice channel** :)");
                vnc = await vnext.ConnectAsync(chn);
                await EditMessage(e, color: DiscordColor.Green, desc: $"**Connected to** `{chn.Name}`");
                //await Music(e, "M:/Downloaded/Morbot_VoiceIntro.mp3");
            }
            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();

        }
        #endregion
        #region voice channel leave command
        [Command("leave"), Aliases("vchleave", "voicechannelleave", "voiceleave", "channelleave", "voicechleave"), Description("Leaves a voice channel.")]
        public async Task Leave(CommandContext e)
        {
            await e.TriggerTypingAsync();
            var vnext = e.Client.GetVoiceNext();
            if (vnext == null)
            {
                await CreateMessage(e, desc: error_message, color: DiscordColor.Red);
                return;
            }

            var vnc = vnext.GetConnection(e.Guild);
            if (vnc == null)
            {
                await CreateMessage(e, desc: "**Not connected on this server**", color: DiscordColor.Green);
                return;
            }
            vnc.Disconnect();
            await CreateMessage(e, desc: "**Disconnected!**", color: DiscordColor.Green);
        }
        #endregion
        #region voice channel play command
        [Command("play"), Aliases("vchplay", "voicechannelplay", "voiceplay", "channelplay", "voicechplay"), Description("Plays an audio file.")]
        public async Task Play(CommandContext e, [RemainingText, Description("Full path to the file to play.")] string filename)
        {
            await ConnectToVoiceChannel(e);
            if (filename.Contains("youtu"))
            {

                await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "youtube-dl",
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

                            string videoname = ShortenName($"Songs/{oRootObject.id}_{System.Web.HttpUtility.UrlEncode(oRootObject.fulltitle)}.mp3", ".mp3");
                            if (File.Exists(videoname))
                            {
                                await EditMessage(e, desc: $"**Playing:** `{oRootObject.fulltitle}`", imageurl: oRootObject.thumbnail, thumbnailurl: "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b8/YouTube_Logo_2017.svg/2000px-YouTube_Logo_2017.svg.png");
                                await SetSpeaking(e, true);
                                await Music(e, videoname);
                            }
                            else
                            {
                                HttpClient cl = new HttpClient();
                                HttpResponseMessage response = await cl.GetAsync(oRootObject.formats[1].url);
                                using (FileStream fs = new FileStream(videoname, FileMode.Create))
                                {

                                    await EditMessage(e, desc: $"**Playing:** `{oRootObject.fulltitle}`", imageurl: oRootObject.thumbnail, thumbnailurl: "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b8/YouTube_Logo_2017.svg/2000px-YouTube_Logo_2017.svg.png");
                                    await response.Content.CopyToAsync(fs);
                                    await SetSpeaking(e, true);
                                    await Music(e, videoname);
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

                            string videoname = ShortenName($"Songs/{oRootObject.id}_{System.Web.HttpUtility.UrlEncode(oRootObject.fulltitle)}.mp3", ".mp3");

                            if (File.Exists(videoname))
                            {
                                await EditMessage(e, desc: $"**Playing:** `{oRootObject.fulltitle}`", thumbnailurl: "https://upload.wikimedia.org/wikipedia/commons/e/ea/Mp3.svg");
                                await SetSpeaking(e, true);
                                await Music(e, videoname);
                            }
                            else
                            {
                                await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
                                HttpClient cl = new HttpClient();
                                HttpResponseMessage response = await cl.GetAsync(oRootObject.formats[1].url);
                                using (FileStream fs = new FileStream(videoname, FileMode.Create))
                                {

                                    await EditMessage(e, desc: $"**Playing:** `{oRootObject.fulltitle}`", thumbnailurl: "https://upload.wikimedia.org/wikipedia/commons/e/ea/Mp3.svg");
                                    await response.Content.CopyToAsync(fs);
                                    await SetSpeaking(e, true);
                                    await Music(e, videoname);
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

                await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
                string site = $"WEBSongs/{filename}.mp3";
                if (File.Exists(site))
                {
                    await EditMessage(e, desc: $"**Playing:** `{site}`");
                    await SetSpeaking(e, true);
                    await Music(e, site);
                }
                else
                {

                    HttpClient cl = new HttpClient();
                    HttpResponseMessage response = await cl.GetAsync(filename);
                    using (FileStream fs = new FileStream(site, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fs);
                        await CreateMessage(e, desc: $"**Playing:** `{filename}`");
                        await SetSpeaking(e, true);
                        await Music(e, site);
                    }
                }
            }
            else
            {
                await CreateMessage(e, desc: processing_message, color: DiscordColor.Blurple);
                if (!File.Exists(filename))
                {
                    await CreateMessage(e, desc: $"**File** `{filename}` **does not exist!**");
                    return;
                }
                await CreateMessage(e, desc: $"**Playing:** `{filename}`");
                await SetSpeaking(e, true);
                await Music(e, filename);

            }
        }
        #endregion
        #region speak command
        [Command("speak"), Aliases("vchspeak", "voicechannelspeak", "voicespeak", "channelspeak", "voicechspeak"), Description("speaks for you in voice channel :)")]
        public async Task Speak(CommandContext e, params string[] args)
        {
            await e.TriggerTypingAsync();
            await ConnectToVoiceChannel(e);
            string text = "";
            foreach (string arg in args)
            {
                text = text + " " + arg;
            }
            if (text.Remove(0, 1).Length > 200)
            {
                await CreateMessage(e, color: DiscordColor.Red, desc: $"**This request cannot be processed because the length of text is over 200!**\n**Length:** {text.Remove(0, 1).Length}");
                return;
            }

            string speakdata = text.Remove(0, 1);
            string speaktext = speakdata.Remove(0, 3);

            string page = "https://translate.google.com/translate_tts?ie=UTF-8&q=" + System.Web.HttpUtility.UrlEncode(speaktext) + "&tl=" + speakdata.Remove(2, speakdata.Length - 2) + "&client=tw-ob";
            string filename = ShortenName($"Translator/{speakdata.Remove(2, speakdata.Length - 2)}_{System.Web.HttpUtility.UrlEncode(speaktext)}.mp3", ".mp3");

            if (File.Exists(filename))
            {
                await CreateMessage(e, color: DiscordColor.Green, desc: $"**Letters Remaining:** {(200 - text.Remove(0, 1).Length)}\n\n**Speaking:** `{speakdata.Remove(0, 3)}`");
                await SetSpeaking(e, true);
                await Music(e, filename);
            }
            else
            {
                HttpClient cl = new HttpClient();
                HttpResponseMessage response = await cl.GetAsync(page);
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);

                    await CreateMessage(e, color: DiscordColor.Green, desc: $"**Letters Remaining:** {(200 - text.Remove(0, 1).Length)}\n\n**Speaking:** `{speakdata.Remove(0, 3)}`");
                    await SetSpeaking(e, true);
                    await Music(e, filename);
                }
            }
        }
        #endregion
        #region lookatthisdude command
        [Command("lookatthisdude"), Aliases("dude", "lookatdude", "latd", "smiech"), Description("Plays the guy that was laughing.")]
        public async Task LATD(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await ConnectToVoiceChannel(e);
            await CreateMessage(e, imageurl: "http://ww2.hdnux.com/photos/51/63/45/10959205/17/920x920.jpg", color: DiscordColor.Green);
            await SetSpeaking(e, true);
            await Music(e, "Songs/LookAtThisDude.mp3");
        }
        #endregion
    }
}