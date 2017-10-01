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

namespace Morbot
{


    
    public class Commands
    {
        #region whoami command
        [Command("whoami")]
        public async Task whoami(CommandContext e)
        {
            await e.Message.RespondAsync("I am MorcBot, and my programmer(Morc) wants to have this bot as help for Discord server.");
            Program.CWrite("Command whoami was executed by " + e.User.Username + "#" + e.User.Discriminator + " on server:" + e.Guild.Name, ConsoleColor.DarkGreen);
        }
#endregion
        #region latestvideo command
        [Command("latestvideo")]
        public async Task latestvideo(CommandContext e)
        {
            Program.CWrite("Command latestvideo was executed by " + e.User.Username + "#" + e.User.Discriminator + " on server:" + e.Guild.Name, ConsoleColor.DarkGreen);
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

                        await e.Message.RespondAsync(playlistItemsListResponse.Items[0].Snippet.Title + " https://youtu.be/" + playlistItemsListResponse.Items[0].Snippet.ResourceId.VideoId);

                        nextPageToken = playlistItemsListResponse.NextPageToken;
                    }
                }
            }
            catch (Exception exy)
            {

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
            Program.CWrite("Command weather was executed by " + e.User.Username + "#" + e.User.Discriminator + " on server:" + e.Guild.Name, ConsoleColor.DarkGreen);
            string data;
            string page = "http://api.openweathermap.org/data/2.5/weather?q=Topolcany&mode=json&APPID=f087a4bd2e59b76b49fe81f9de972f7e";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                
                data = await content.ReadAsStringAsync();
                RootObject oRootObject = new RootObject();
                oRootObject = JsonConvert.DeserializeObject<RootObject>(data);
                string weathertype = null;
                if (oRootObject.weather[0].description == "clear sky")
                    weathertype = ":sunny:" + " - Sunny";
                await e.Message.RespondAsync("Town near Morc - Topoľčany:\n" + (oRootObject.main.temp - 273.15) + "°C \n" + weathertype);

            }
        }

        #endregion
        #region time command
                [Command("time")]
                    public async Task time (CommandContext e)
                    {
                        Program.CWrite("Command time was executed by " + e.User.Username + "#" + e.User.Discriminator + " on server:" + e.Guild.Name, ConsoleColor.DarkGreen);
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
                    }
        #endregion
        #region randomwindows command

        string[] versions = { "w95.png", "w98.png", "wme.png", "w2k.png", "w7.png", "wvista.png", "wxp.png", "w8.png", "w10.png" };
                [Command("randomwindows")]
                public async Task randomwindows(CommandContext e)
                {
                    Program.CWrite("Command randomwindows was executed by " + e.User.Username + "#" + e.User.Discriminator + " on server:" + e.Guild.Name, ConsoleColor.DarkGreen);
            Random rnd = new Random();
            string ver = versions[rnd.Next(0, versions.Length)];
            await e.Message.RespondWithFileAsync(ver);
                }

        #endregion

    }
    
}