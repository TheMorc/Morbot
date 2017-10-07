﻿using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using System.Linq;

namespace Morbot
{ 
    
    public class Commands
    {
        //HELP FOR COMMANDS 
        #region help for commands
        public static async Task CreateMessage(CommandContext e, string titleurl = null,string imageurl = null,string thumbnailurl = null,string url = null, string desc = "", string title = "Morbot (Made in :flag_sk:)", DiscordColor color = default(DiscordColor),bool sendToUser = false)
        {

            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Color = color,
                Description = desc,
                ImageUrl = imageurl,
                ThumbnailUrl = thumbnailurl,
                Url = url
            };
            if(sendToUser)
            await e.Member.SendMessageAsync("", embed: embed);
            else
            await e.RespondAsync("", embed: embed);
        }
        #endregion

        //admin only commands!
        #region servers command
        [Command("servers"),RequirePermissions(DSharpPlus.Permissions.ManageGuild),Hidden]
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
                        serverlist = server +"\n"+ serverlist;
                    }
                }
            await CreateMessage(ex, desc: "Servers: " + serverlist, sendToUser: true,color: DiscordColor.Cyan);
        }
        #endregion

        //Commands | 
        #region whoami command
        [Command("whoami")]
        public async Task Whoami(CommandContext e)
        {
            await CreateMessage(e,desc: "I am Morbot, and my programmer(Morc) wants to have this bot as help for Discord server.",color: DiscordColor.Cyan);
        }
        #endregion
        #region test command
        [Command("test"),RequireOwner]
        public async Task test(CommandContext e)
        {
            await CreateMessage(e);
        }
        #endregion
        #region latestvideo command
        [Command("latestvideo")]
        public async Task Latestvideo(CommandContext e)
        {
            string api = "";
            bool empty = false;
            try
            {
                api = File.ReadAllLines("ytapikey")[0];
            }
            catch
            {
                empty = true;
            }
            if (empty)
            {
                await e.Message.RespondAsync("\u200B" + e.User.Mention + " Bot has incorrectly set API Keys.");

            }
            else
            {

                try
                {
                    var yt = new YouTubeService(new BaseClientService.Initializer()
                    {
                        ApiKey = File.ReadAllLines("ytapikey")[0]
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
                    
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + " Error occured when sending video link. Contact programmer..");

                }
            }
        }
#endregion
        #region weather command
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }
        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }
        public class Main
        {
            public double temp { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
        }
        public class Wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
        }
        public class Clouds
        {
            public int all { get; set; }
        }
        public class Sys
        {
            public int type { get; set; }
            public int id { get; set; }
            public double message { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
        }
        public class RootObjectW
        {
            public Coord coord { get; set; }
            public List<Weather> weather { get; set; }
            public string @base { get; set; }
            public Main main { get; set; }
            public int visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public Sys sys { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }
        [Command("weather")]
            public async Task CWeather (CommandContext e)
            {
            string data = "";
            string api = "";
            bool empty = false;
            string weathertype = null;
            double temp = 0;
            try
            {
                api = File.ReadAllLines("openwapikey")[0];
            }
            catch
            {
                empty = true;
            }
            if(empty)
            {
                await e.Message.RespondAsync("\u200B" + e.User.Mention + " Bot has incorrectly set API Keys.");

            }
            else
            {
                string page = "http://api.openweathermap.org/data/2.5/weather?q=Topolcany&mode=json&APPID=" + File.ReadAllLines("openwapikey")[0];
                using (HttpClient cl = new HttpClient())
                {
                    data = await cl.GetStringAsync(page);
                    RootObjectW oRootObject = new RootObjectW();
                    oRootObject = JsonConvert.DeserializeObject<RootObjectW>(data);
                    weathertype = null;
                    DiscordColor wcolor = DiscordColor.None;
                    temp = oRootObject.main.temp - 273.15;
                    if (oRootObject.weather[0].description == "clear sky") { 
                        weathertype = ":sunny:" + " - Sunny";
                        wcolor = DiscordColor.Yellow;
                    }
                    if (oRootObject.weather[0].description == "few clouds") { 
                        weathertype = ":cloud:" + " - Clouds";
                        wcolor = DiscordColor.Gray;
                    }
                    if (oRootObject.weather[0].description == "light rain") { 
                        weathertype = ":cloud_rain:" + " - Rain";
                        wcolor = DiscordColor.Cyan;
                    }
                    await CreateMessage(e,desc:"Town near Morc - Topoľčany:\n" + temp + "°C \n" + weathertype,color: wcolor);
                }
            }
           
        }

        #endregion
        #region randomnorrisjoke command
        public class RootObjectnorris
        {
            public object category { get; set; }
            public string icon_url { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public string value { get; set; }
        }
        [Command("randomnorrisjoke"),Aliases("norris","norrisjoke","chucknorris","chuck","chuckjoke","randomchuckjoke")]
        public async Task ChuckNorris(CommandContext e)
        {
            string data = "";
            string url="";
                string page = "https://api.chucknorris.io/jokes/random";
                using (HttpClient cl = new HttpClient())
                {
                    data = await cl.GetStringAsync(page);
                    RootObjectnorris chuck = new RootObjectnorris();
                    chuck = JsonConvert.DeserializeObject<RootObjectnorris>(data);
                    url = chuck.url;
                    await CreateMessage(e, title: "Chuck Norris joke:", desc: chuck.value,thumbnailurl: chuck.icon_url,url: "https://api.chucknorris.io",color: DiscordColor.Green);
                }
        }

        #endregion
        #region time command
        [Command("time")]
            public async Task Time (CommandContext e)
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
            await CreateMessage(e, desc: "Morc's time zone is UTC+01:00 so the time is: " + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes,color: DiscordColor.Orange);
        }
        #endregion
        #region randomwindows command
        string[] versions = { "w95.png", "w98.png", "wme.png", "w2k.png", "w7.png", "wvista.png", "wxp.png", "w8.png", "w10.png" };
        [Command("randomwindows"),Aliases("RandomWindows","RandWind","randwind","ranwin","randomwin","RandomWin","rwin","randomw","randw")]
        public async Task RandomWindows(CommandContext e)
        {
                    Random rnd = new Random();
                    string ver = versions[rnd.Next(0, versions.Length)];
                    await e.Message.RespondWithFileAsync(ver);
        }
        #endregion
        #region cat command
        [Command("meow"), Aliases("cat", "kitty", "catpicture", "meov", "mjau")]
        public async Task Meow(CommandContext e)
        {
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://random.cat/meow");
                var pData = JObject.Parse(data);
                url = pData["file"].ToString();
                await CreateMessage(e, imageurl: url,color: DiscordColor.Green);
            }
            
        }
        #endregion
        #region dog command
        [Command("woof"),Aliases("dog", "puppy","dogpicture","hau","haw")]
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
        [Group("changestatus",CanInvokeWithoutSubcommand = true), Aliases("status")]
        public class Status
        {
            public async Task ExecuteGroupAsync(CommandContext e,string name = "")
            {
                if(name == "BETA")
                {
                    await BETA(e);
                    await CreateMessage(e, desc: "Status set to " + name,color:DiscordColor.Green);
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
                else if(name == "")
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
                string gamename = Program.prefix + "help|BETA Mode";
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
                string gamename = Program.prefix + "help|WIP Mode";
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
                string gamename = Program.prefix + "help|FIX Mode";
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
                string gamename = Program.prefix + "help|Bot ready.";
                DiscordGame game = new DiscordGame()
                {
                    StreamType = GameStreamType.NoStream,
                    Name = gamename
                };
                await e.Client.UpdateStatusAsync(game);
            }
        }
        #endregion
        #region randomgifv2 command
        public class Data
        {
            public string type { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public string image_original_url { get; set; }
            public string image_url { get; set; }
            public string image_mp4_url { get; set; }
            public string image_frames { get; set; }
            public string image_width { get; set; }
            public string image_height { get; set; }
            public string fixed_height_downsampled_url { get; set; }
            public string fixed_height_downsampled_width { get; set; }
            public string fixed_height_downsampled_height { get; set; }
            public string fixed_width_downsampled_url { get; set; }
            public string fixed_width_downsampled_width { get; set; }
            public string fixed_width_downsampled_height { get; set; }
            public string fixed_height_small_url { get; set; }
            public string fixed_height_small_still_url { get; set; }
            public string fixed_height_small_width { get; set; }
            public string fixed_height_small_height { get; set; }
            public string fixed_width_small_url { get; set; }
            public string fixed_width_small_still_url { get; set; }
            public string fixed_width_small_width { get; set; }
            public string fixed_width_small_height { get; set; }
            public string username { get; set; }
            public string caption { get; set; }
        }

        public class Meta
        {
            public int status { get; set; }
            public string msg { get; set; }
            public string response_id { get; set; }
        }

        public class RootObjectG
        {
            public Data data { get; set; }
            public Meta meta { get; set; }
        }
        [Command("randomgif"), Aliases("randgif")]
        public async Task GIFSearch(CommandContext e, [RemainingText]string arg1 = "")
            {
                string data = "";
                string gifby = "";
                
                string gifurl = "";
                Random urlRandomizer = new Random();
                string[] GIFtype = { "cat", "dog" };
                string api = "";
                bool empty = false;
                try
                {
                    api = File.ReadAllLines("giphyapikey")[0];
                }
                catch
                {
                    empty = true;
                }
                if (empty)
                {
                await CreateMessage(e, desc: "Incorrecty set API keys!", color: DiscordColor.Red);

                }
                else
                {
                    string page = null;
                    if(arg1 == "")
                    {
                    arg1 = GIFtype[urlRandomizer.Next(0, GIFtype.Length)];
                       page = "http://api.giphy.com/v1/gifs/random?q=cat&tag=" + arg1 + "&api_key=" + File.ReadAllLines("giphyapikey")[0];

                    }
                    else
                    {
                        page = "http://api.giphy.com/v1/gifs/random?q=cat&tag=" + arg1  + "&api_key=" + File.ReadAllLines("giphyapikey")[0];
                    }
                    using (HttpClient client = new HttpClient())
                    using (HttpResponseMessage response = await client.GetAsync(page))
                    using (HttpContent content = response.Content)
                    {
                        data = await content.ReadAsStringAsync();
                        RootObjectG oRootObject = new RootObjectG();
                        oRootObject = JsonConvert.DeserializeObject<RootObjectG>(data);
                        gifurl = oRootObject.data.image_url;
                        if (oRootObject.data.username == "")
                        {
                            gifby = "";
                        }
                        else
                        {
                            gifby = "GIF By: " + oRootObject.data.username;
                        }
                        await CreateMessage(e,desc:gifby, imageurl: gifurl, color: DiscordColor.Green);
                }
                    
                }
            
            
        }
        #endregion

        //deprecated/old commands
        #region old version of randomgif command.. for historical purposes
        //#region randomgif command
        //public class Data
        //{
        //    public string type { get; set; }
        //    public string id { get; set; }
        //    public string url { get; set; }
        //    public string image_original_url { get; set; }
        //    public string image_url { get; set; }
        //    public string image_mp4_url { get; set; }
        //    public string image_frames { get; set; }
        //    public string image_width { get; set; }
        //    public string image_height { get; set; }
        //    public string fixed_height_downsampled_url { get; set; }
        //    public string fixed_height_downsampled_width { get; set; }
        //    public string fixed_height_downsampled_height { get; set; }
        //    public string fixed_width_downsampled_url { get; set; }
        //    public string fixed_width_downsampled_width { get; set; }
        //    public string fixed_width_downsampled_height { get; set; }
        //    public string fixed_height_small_url { get; set; }
        //    public string fixed_height_small_still_url { get; set; }
        //    public string fixed_height_small_width { get; set; }
        //    public string fixed_height_small_height { get; set; }
        //    public string fixed_width_small_url { get; set; }
        //    public string fixed_width_small_still_url { get; set; }
        //    public string fixed_width_small_width { get; set; }
        //    public string fixed_width_small_height { get; set; }
        //    public string username { get; set; }
        //    public string caption { get; set; }
        //}

        //public class Meta
        //{
        //    public int status { get; set; }
        //    public string msg { get; set; }
        //    public string response_id { get; set; }
        //}

        //public class RootObjectG
        //{
        //    public Data data { get; set; }
        //    public Meta meta { get; set; }
        //}
        //[Command("randomgif")]
        //public async Task randomgif(CommandContext e)
        //{
        //    WriteCommandsExec(e);
        //    string data = "";
        //    string gifby = "";
        //    string gifurl = "";
        //    Random urlRandomizer = new Random();
        //    string[] GIFtype = { "cat", "dog" };
        //    string api = "";
        //    bool empty = false;
        //    try
        //    {
        //        api = File.ReadAllLines("giphyapikey")[0];
        //    }
        //    catch
        //    {
        //        empty = true;
        //    }
        //    if (empty)
        //    {
        //        WriteCommandFailed(e, "Giphy API Key file is EMPTY!");
        //        await e.Message.RespondAsync("\u200B" + e.User.Mention + " Bot has incorrectly set API Keys.");

        //    }
        //    else
        //    {
        //        string page = "http://api.giphy.com/v1/gifs/random?q=cat&tag=" + GIFtype[urlRandomizer.Next(0, GIFtype.Length)] + "&api_key=" + File.ReadAllLines("giphyapikey")[0];
        //        using (HttpClient client = new HttpClient())
        //        using (HttpResponseMessage response = await client.GetAsync(page))
        //        using (HttpContent content = response.Content)
        //        {
        //            data = await content.ReadAsStringAsync();
        //            RootObjectG oRootObject = new RootObjectG();
        //            oRootObject = JsonConvert.DeserializeObject<RootObjectG>(data);
        //            gifurl = oRootObject.data.image_url;
        //            if (oRootObject.data.username == "")
        //            {
        //                gifby = "";
        //            }
        //            else
        //            {
        //                gifby = "GIF By: " + oRootObject.data.username;
        //            }
        //            await e.RespondAsync("\u200B" + e.User.Mention + " " + gifurl + "\n \n" + gifby);
        //        }

        //        WriteCommandSucceeded(e, "Sent GIF: " + gifurl);
        //    }
        //}

        //#endregion
        #endregion
        #region commands command.. new beta of DSharpPlus adds help command..
        //#region commands command
        //[Command("commands")]
        //public async Task commands(CommandContext ex)
        //{
        //    WriteCommandsExec(ex);
        //    string commandlist = null;
        //    foreach (string server in ex.CommandsNext.RegisteredCommands.Values.Select(e => e.Name))
        //    {
        //        string help = "";
        //        try { help = commandlist.Remove(server.Length, commandlist.Length - server.Length); } catch { }
        //        if (help == server) { }
        //        else
        //        { commandlist = server + "\n--" + commandlist; }
        //    }
        //    await ex.Message.RespondAsync("\u200B" + ex.User.Mention + "\nCommand List:\n" + "--" + commandlist.Remove(commandlist.Length - 2, 2));
        //    WriteCommandSucceeded(ex, " Sent command list.");
        //}
        //#endregion
#endregion
    }
}