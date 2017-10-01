using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
namespace Morbot
{
    class Program
    {
        //simpul introduction to this bot
        static DiscordClient discord;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello to my bot!");


            if (!File.Exists("token"))
            {
                File.Create("token");
                Console.WriteLine("Created token file.");
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
                    Console.WriteLine("Contents of Token file are empty! This discord bot cannot work until you fill everything!");
                }
                if (!empty)
                    Console.WriteLine("Token file already exists. Contents: " + txt);

            }
            if (!File.Exists("ytapikey"))
            {
                File.Create("ytapikey");
                Console.WriteLine("Created Youtube Data API Key file.");
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
                    Console.WriteLine("Contents of Youtube Data API Key file are empty! This discord bot cannot work until you fill everything!");
                }
                if (!empty)
                    Console.WriteLine("Youtube Data API Key file already exists. Contents: " + txt);

            }

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            //initializing discord client that is needed to have a working discord bot(really? i dont need this line but as always do it! xDDD)
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllLines("token")[0],
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("/!whoami"))
                {
                    await e.Message.RespondAsync("I am MorcBot, you friendly bot that can show you many things(not the nude ones..).");
                    Console.WriteLine("Command /!whoami was executed by " + e.Author.Username + "#" + e.Author.Discriminator + " on server:" + e.Guild.Name);
                }

            };
            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("/!latestvideo"))
                {
                    await e.Message.RespondAsync("Getting latest video from Morc");
                    Console.WriteLine("Command /!latestvideo was executed by " + e.Author.Username + "#" + e.Author.Discriminator + " on server:" + e.Guild.Name);

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
                        Console.WriteLine(exy);
                    }

                }
            };
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}