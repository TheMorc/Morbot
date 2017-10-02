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
namespace Morbot
{ 
    public class Commands
    {
        #region help for commands
        public static void WriteCommandsExec(CommandContext e)
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
            Program.CWrite("Command " + e.Command.Name + " was executed by " + e.User.Username + "#" + e.User.Discriminator + " on server:" + e.Guild.Name + " " + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes, ConsoleColor.DarkGreen);
        }
        public static void WriteCommandSucceeded(CommandContext e,string whatitdid)
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

            Program.CWrite("Executing command " + e.Command.Name + " succeeded without problem. What it did: " +whatitdid+  " " + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes, ConsoleColor.DarkGreen);


        }
        public static void WriteCommandFailed(CommandContext e,string reason)
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

            Program.CWrite("Executing command " + e.Command.Name + " failed! Reason: " + reason  + " "+ DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes, ConsoleColor.Red);


        }
        #endregion
        #region whoami command
        [Command("whoami")]
        public async Task whoami(CommandContext e)
        {
            WriteCommandsExec(e);
            await e.Message.RespondAsync("I am MorcBot, and my programmer(Morc) wants to have this bot as help for Discord server.");
            WriteCommandSucceeded(e," Sent info about bot.");
        }
#endregion
        #region latestvideo command
        [Command("latestvideo")]
        public async Task latestvideo(CommandContext e)
        {
            WriteCommandsExec(e);
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
                        await e.Message.RespondAsync(playlistItemsListResponse.Items[0].Snippet.Title + " " + ytlink;
                        WriteCommandSucceeded(e, "Sent yt link: " + ytlink);
                        nextPageToken = playlistItemsListResponse.NextPageToken;
                    }
                }
            }
            catch (Exception exy)
            {

                WriteCommandFailed(e, "Failed sending link. Log:");
                Program.CWrite(exy.ToString(), ConsoleColor.Red);

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

        public class RootObject
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
            string data;
            string weathertype = null;
            double temp = 0;
            string page = "http://api.openweathermap.org/data/2.5/weather?q=Topolcany&mode=json&APPID=f087a4bd2e59b76b49fe81f9de972f7e";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                
                data = await content.ReadAsStringAsync();
                RootObject oRootObject = new RootObject();
                oRootObject = JsonConvert.DeserializeObject<RootObject>(data);
                weathertype = null;
                temp = oRootObject.main.temp - 273.15);
                if (oRootObject.weather[0].description == "clear sky")
                    weathertype = ":sunny:" + " - Sunny";
                await e.Message.RespondAsync("Town near Morc - Topoľčany:\n" + temp + "°C \n" + weathertype);

            }

            WriteCommandSucceeded(e, "Sent info about weather: " + temp + "°C " + weathertype);
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
            await e.Message.RespondAsync("Morc's time zone is UTC+01:00 so the time is: "+DateTime.Now.TimeOfDay.Hours.ToString() + ":" + minutes);


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
                await e.RespondAsync(url);
            }

            WriteCommandSucceeded(e, "Sent cute cat picture. Link: "+ url);
        }
        #endregion
        #region dog command
        [Command("woof"),Aliases("dog", "doggy","dogpicture","hau","haw")]
        public async Task woof(CommandContext e)
        {
            WriteCommandsExec(e);
            string url = null;
            using (HttpClient cl = new HttpClient())
            {
                string data = await cl.GetStringAsync("https://random.dog/woof.json");
                var pData = JObject.Parse(data);
                url = pData["url"].ToString();
                await e.RespondAsync(url);
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
                }
                else if (name == "WIP")
                {
                    await WIP(e);
                }
                else if (name == "FIX")
                {
                    await FIX(e);
                }
                else if (name == "READY")
                {
                    await READY(e);
                }
                
                if(name == "")
                {
                    await e.Message.RespondAsync(e.User.Mention + " Select status from one of these: BETA WIP FIX READY .");
                    WriteCommandFailed(e, "User didnt specify any status.");
                }
                else
                {
                    await e.Message.RespondAsync(e.User.Mention + name +" isn't a status. Select status from one of these: BETA WIP FIX READY .");

                    WriteCommandFailed(e, "User specified status " + name + "but this status does not exists. Sending status selection message.");
                }
            }
            

            [Command("null1")]
            public async Task BETA(CommandContext e)
        {
                WriteCommandsExec(e);
               
                string gamename = null;
                Game game = new Game();
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
                Game game = new Game();
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
                Game game = new Game();
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
                Game game = new Game();
                game.StreamType = GameStreamType.NoStream;
                gamename = Program.prefix + "help|Bot ready.";
                game.Name = gamename;
                await e.Client.UpdateStatusAsync(game);
                WriteCommandSucceeded(e, "Changed bot Playing status to READY");

            }
        }
        #endregion
    }
}