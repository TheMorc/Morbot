using System;
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
        public static void WriteCommandsExec(CommandContext e)
        {
            Program.CWrite("Command " + e.Command.Name + " was executed by " + e.User.Username + "#" + e.User.Discriminator + " on server:" + e.Guild.Name, ConsoleColor.DarkGreen);
        }
        public static void WriteCommandSucceeded(CommandContext e,string whatitdid)
        {
            Program.CWrite("Executing command " + e.Command.Name + " succeeded without problem. What it did: " +whatitdid, ConsoleColor.DarkGreen);
        }
        public static void WriteCommandFailed(CommandContext e,string reason)
        {
            Program.CWrite("Executing command " + e.Command.Name + " failed! Reason: " + reason, ConsoleColor.Red);
        }
        #endregion
        //admin only commands!
        #region servers command
        [Command("servers"),RequirePermissions(DSharpPlus.Permissions.ManageGuild),Hidden]
        public async Task servers(CommandContext ex)
        {
            string serverlist = null;
            WriteCommandsExec(ex);
            foreach (string server in ex.Client.Guilds.Values.Select(e => e.Name))
                {


                    Program.CWrite(server);
                    string help = "";
                    try { help = serverlist.Remove(server.Length, serverlist.Length - server.Length); } catch { }
                    if (help == server) { }
                    else
                    {
                        serverlist = server +"\n"+ serverlist;
                    }
                }
                await ex.Member.SendMessageAsync("\u200B" + ex.User.Mention +"\nServers:\n" + serverlist);
            WriteCommandSucceeded(ex, " Sent list of servers on which this bot is!");
        }
        #endregion
        //Commands | 
        #region whoami command
        [Command("whoami")]
        public async Task whoami(CommandContext e)
        {
            WriteCommandsExec(e);
            await e.Message.RespondAsync("\u200B" + e.User.Mention + " I am MorcBot, and my programmer(Morc) wants to have this bot as help for Discord server.");
            WriteCommandSucceeded(e, " Sent info about bot.");
        }
        #endregion
        #region commands command
        [Command("commands")]
        public async Task commands(CommandContext ex)
        {
            WriteCommandsExec(ex);
            string commandlist = null;
            foreach (string server in ex.CommandsNext.RegisteredCommands.Values.Select(e => e.Name))
            {
                string help = "";
                try { help = commandlist.Remove(server.Length, commandlist.Length - server.Length); } catch { }
                if (help == server) { }
                else
                { commandlist = server + "\n--" + commandlist;}
            }
            await ex.Message.RespondAsync("\u200B" + ex.User.Mention + "\nCommand List:\n" + "--" +  commandlist.Remove(commandlist.Length-2,2));
            WriteCommandSucceeded(ex, " Sent command list.");
        }
        #endregion
        #region latestvideo command
        [Command("latestvideo")]
        public async Task latestvideo(CommandContext e)
        {
            WriteCommandsExec(e);
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
                WriteCommandFailed(e, "YouTube Data API Key file is EMPTY!");
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
                            
                            await e.Message.RespondAsync("\u200B" + playlistItemsListResponse.Items[0].Snippet.Title + " " + ytlink);
                            WriteCommandSucceeded(e, "Sent yt link: " + ytlink);
                            nextPageToken = playlistItemsListResponse.NextPageToken;
                        }
                    }
                }
                catch (Exception exy)
                {

                    WriteCommandFailed(e, "Failed sending link. Log:");
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + " Error occured when sending video link. Contact programmer..");
                    Program.CWrite(exy.ToString(), ConsoleColor.Red);

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
            public async Task weather (CommandContext e)
            {
            WriteCommandsExec(e);
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
                WriteCommandFailed(e,"OpenWeather API Key file is EMPTY!");
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
                    temp = oRootObject.main.temp - 273.15;
                    if (oRootObject.weather[0].description == "clear sky")
                        weathertype = ":sunny:" + " - Sunny";
                    if (oRootObject.weather[0].description == "few clouds")
                        weathertype = ":cloud:" + " - Clouds";
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + "\n Town near Morc - Topoľčany:\n" + temp + "°C \n" + weathertype);
                }
                WriteCommandSucceeded(e, "Sent info about weather: " + temp + "°C " + weathertype);
            }
           
        }

        #endregion
        #region time command
        [Command("time")]
            public async Task time (CommandContext e)
            {
            WriteCommandsExec(e);
            string minutes = null;
            if (DateTime.Now.TimeOfDay.Minutes.ToString().Length == 1)
            {
                minutes = "0" + DateTime.Now.TimeOfDay.Minutes.ToString();
            }
            else
            {
                minutes = DateTime.Now.TimeOfDay.Minutes.ToString();
            }
            await e.Message.RespondAsync("\u200B" + e.User.Mention + " Morc's time zone is UTC+01:00 so the time is: " + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes);


            WriteCommandSucceeded(e,"Sent time.");
        }
        #endregion
        #region randomwindows command

        string[] versions = { "w95.png", "w98.png", "wme.png", "w2k.png", "w7.png", "wvista.png", "wxp.png", "w8.png", "w10.png" };
                [Command("randomwindows"),Aliases("RandomWindows","RandWind","randwind","ranwin","randomwin","RandomWin","rwin","randomw","randw")]
                public async Task randomwindows(CommandContext e)
                {
            WriteCommandsExec(e);
            Random rnd = new Random();
            string ver = versions[rnd.Next(0, versions.Length)];
            await e.Message.RespondAsync("\u200B" + e.User.Mention);
            await e.Message.RespondWithFileAsync(ver);


            WriteCommandSucceeded(e,"Sent " + ver);
        }

        #endregion
        #region cat command
        [Command("meow"), Aliases("cat", "kitty", "catpicture", "meov", "mjau")]
        public async Task meow(CommandContext e)
        {
            WriteCommandsExec(e);
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://random.cat/meow");
                var pData = JObject.Parse(data);
                url = pData["file"].ToString();
                await e.RespondAsync("\u200B" + e.User.Mention + " " + url);
            }

            WriteCommandSucceeded(e, "Sent cute cat picture. Link: "+ url);
        }
        #endregion
        #region dog command
        [Command("woof"),Aliases("dog", "puppy","dogpicture","hau","haw")]
        public async Task woof(CommandContext e)
        {
            WriteCommandsExec(e);
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://random.dog/woof.json");
                var pData = JObject.Parse(data);
                url = pData["url"].ToString();
                await e.RespondAsync("\u200B" + e.User.Mention + " " + url);
            }
            WriteCommandSucceeded(e,"Sent cute dog picture. Link: " + url);
        }
        #endregion
        #region status command
        [Group("changestatus",CanInvokeWithoutSubcommand = true), Aliases("status")]
        public class status
        {
            public async Task ExecuteGroupAsync(CommandContext e,string name = "")
            {
                if(name == "BETA")
                {
                    await BETA(e);
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + "\nStatus set to BETA");
                }
                else if (name == "WIP")
                {
                    await WIP(e);
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + "\nStatus set to WIP");
                }
                else if (name == "FIX")
                {
                    await FIX(e);
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + "\nStatus set to FIX");
                }
                else if (name == "READY")
                {
                    await READY(e);
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + "\nStatus set to READY");
                }
                else if(name == "")
                {
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + "\nSelect status from one of these: BETA WIP FIX READY .");
                    WriteCommandFailed(e, "User didnt specify any status.");
                }
                else
                {
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + name +"\nisn't a status. Select status from one of these: BETA WIP FIX READY .");
                    WriteCommandFailed(e, "User specified status " + name + " but this status does not exists. Sending status selection message.");
                }
            }
            [Command("null1")]
            public async Task BETA(CommandContext e)
            {
                WriteCommandsExec(e);
                string gamename = null;
                DiscordGame game = new DiscordGame();
                game.StreamType = GameStreamType.NoStream;
                gamename = Program.prefix + "help|BETA Mode";
                game.Name = gamename;
                await e.Client.UpdateStatusAsync(game);
                WriteCommandSucceeded(e,"Changed bot Playing status to BETA");
            }
            [Command("null2")]
            public async Task WIP(CommandContext e)
            {
                WriteCommandsExec(e);
                string gamename = null;
                DiscordGame game = new DiscordGame();
                game.StreamType = GameStreamType.NoStream;
                gamename = Program.prefix + "help|WIP Mode";
                game.Name = gamename;
                await e.Client.UpdateStatusAsync(game);
                WriteCommandSucceeded(e, "Changed bot Playing status to WIP");
            }
            [Command("null3")]
            public async Task FIX(CommandContext e)
            {
                WriteCommandsExec(e);
                string gamename = null;
                DiscordGame game = new DiscordGame();
                game.StreamType = GameStreamType.NoStream;
                gamename = Program.prefix + "help|FIX Mode";
                game.Name = gamename;
                await e.Client.UpdateStatusAsync(game);
                WriteCommandSucceeded(e, "Changed bot Playing status to FIX");
            }
            [Command("null4")]
            public async Task READY(CommandContext e)
            {
                WriteCommandsExec(e);
                string gamename = null;
                DiscordGame game = new DiscordGame();
                game.StreamType = GameStreamType.NoStream;
                gamename = Program.prefix + "help|Bot ready.";
                game.Name = gamename;
                await e.Client.UpdateStatusAsync(game);
                WriteCommandSucceeded(e, "Changed bot Playing status to READY");
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
        public async Task gifSearch(CommandContext e, [RemainingText]string arg1)
            {
                WriteCommandsExec(e);
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
                    WriteCommandFailed(e, "Giphy API Key file is EMPTY!");
                    await e.Message.RespondAsync("\u200B" + e.User.Mention + " Bot has incorrectly set API Keys.");

                }
                else
                {
                    string page = null;
                    if(arg1 == "")
                    {
                       page = "http://api.giphy.com/v1/gifs/random?q=cat&tag=" + GIFtype[urlRandomizer.Next(0, GIFtype.Length)] + "&api_key=" + File.ReadAllLines("giphyapikey")[0];

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
                        await e.RespondAsync("\u200B" + e.User.Mention + " " + gifurl + "\n \n" + gifby);
                    }

                    WriteCommandSucceeded(e, "Searching for GIF: " + arg1 + " Sent GIF: " + gifurl);
                }
            
            
        }
        #endregion
        //old version of randomgif command.. for historical purposes
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
    }
}