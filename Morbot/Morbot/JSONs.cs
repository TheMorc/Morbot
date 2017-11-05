using System;
using System.Collections.Generic;
using System.Text;

namespace Morbot
{
    class JSONs
    {
        #region JSON for play command
        public class Format1
        {
            public string manifest_url { get; set; }
            public string ext { get; set; }
            public int? fps { get; set; }
            public double tbr { get; set; }
            public object language { get; set; }
            public string format_id { get; set; }
            public string vcodec { get; set; }
            public int abr { get; set; }
            public string acodec { get; set; }
            public int? width { get; set; }
            public int? asr { get; set; }
            public string url { get; set; }
            public int? height { get; set; }
            public string container { get; set; }
            public string protocol { get; set; }
            public int filesize { get; set; }
            public string format_note { get; set; }
            public string format { get; set; }
            public string player_url { get; set; }
            public string resolution { get; set; }
        }
        public class RequestedFormat1
        {
            public string acodec { get; set; }
            public int width { get; set; }
            public object language { get; set; }
            public string manifest_url { get; set; }
            public string ext { get; set; }
            public string format_id { get; set; }
            public int height { get; set; }
            public string url { get; set; }
            public double tbr { get; set; }
            public object asr { get; set; }
            public int fps { get; set; }
            public string protocol { get; set; }
            public string vcodec { get; set; }
            public int filesize { get; set; }
            public string format_note { get; set; }
            public string format { get; set; }
            public int? abr { get; set; }
            public string player_url { get; set; }
        }
        public class Thumbnail1
        {
            public string url { get; set; }
            public string id { get; set; }
        }
        public class Subtitles1
        {
        }
        public class AutomaticCaptions1
        {
        }
        public class RootObjectvideo1
        {
            public object resolution { get; set; }
            public string webpage_url_basename { get; set; }
            public string fulltitle { get; set; }
            public List<Format1> formats { get; set; }
            public object end_time { get; set; }
            public int height { get; set; }
            public List<RequestedFormat1> requested_formats { get; set; }
            public object is_live { get; set; }
            public object chapters { get; set; }
            public int duration { get; set; }
            public string description { get; set; }
            public List<string> tags { get; set; }
            public int abr { get; set; }
            public object creator { get; set; }
            public string extractor_key { get; set; }
            public string acodec { get; set; }
            public int age_limit { get; set; }
            public string uploader_id { get; set; }
            public string _filename { get; set; }
            public object playlist { get; set; }
            public string license { get; set; }
            public int fps { get; set; }
            public object playlist_index { get; set; }
            public int dislike_count { get; set; }
            public string thumbnail { get; set; }
            public List<Thumbnail1> thumbnails { get; set; }
            public object requested_subtitles { get; set; }
            public string extractor { get; set; }
            public object stretched_ratio { get; set; }
            public List<string> categories { get; set; }
            public string uploader_url { get; set; }
            public object annotations { get; set; }
            public string ext { get; set; }
            public int view_count { get; set; }
            public double average_rating { get; set; }
            public Subtitles1 subtitles { get; set; }
            public string display_id { get; set; }
            public string format_id { get; set; }
            public string vcodec { get; set; }
            public object season_number { get; set; }
            public int width { get; set; }
            public object episode_number { get; set; }
            public AutomaticCaptions1 automatic_captions { get; set; }
            public string uploader { get; set; }
            public object alt_title { get; set; }
            public object start_time { get; set; }
            public string webpage_url { get; set; }
            public int like_count { get; set; }
            public string title { get; set; }
            public string id { get; set; }
            public string upload_date { get; set; }
            public string format { get; set; }
            public object series { get; set; }
            public object vbr { get; set; }
        }
        public class Fragment1
        {
            public string path { get; set; }
            public double? duration { get; set; }
        }
        public class Thumbnail2
        {
            public string url { get; set; }
            public string id { get; set; }
        }
        public class Subtitles2
        {
        }
        public class RequestedFormat2
        {
            public string vcodec { get; set; }
            public object filesize { get; set; }
            public List<Fragment2> fragments { get; set; }
            public string manifest_url { get; set; }
            public object language { get; set; }
            public string format { get; set; }
            public double tbr { get; set; }
            public string acodec { get; set; }
            public string format_id { get; set; }
            public string url { get; set; }
            public int? height { get; set; }
            public string protocol { get; set; }
            public string fragment_base_url { get; set; }
            public int? asr { get; set; }
            public string format_note { get; set; }
            public int? width { get; set; }
            public string ext { get; set; }
            public int? fps { get; set; }
            public string container { get; set; }
            public int? abr { get; set; }
        }
        public class AutomaticCaptions2
        {
        }
        public class Fragment2
        {
            public string path { get; set; }
            public double? duration { get; set; }
        }
        public class Format2
        {
            public object filesize { get; set; }
            public string vcodec { get; set; }
            public List<Fragment2> fragments { get; set; }
            public string manifest_url { get; set; }
            public object language { get; set; }
            public string container { get; set; }
            public string acodec { get; set; }
            public string url { get; set; }
            public int? height { get; set; }
            public string protocol { get; set; }
            public string fragment_base_url { get; set; }
            public string format_note { get; set; }
            public string ext { get; set; }
            public int? width { get; set; }
            public string format { get; set; }
            public double tbr { get; set; }
            public string format_id { get; set; }
            public int? asr { get; set; }
            public int abr { get; set; }
            public int? fps { get; set; }
            public string resolution { get; set; }
            public string player_url { get; set; }
        }
        public class RootObjectvideo2
        {
            public string description { get; set; }
            public string vcodec { get; set; }
            public string license { get; set; }
            public List<string> tags { get; set; }
            public int like_count { get; set; }
            public string uploader_id { get; set; }
            public string upload_date { get; set; }
            public string ext { get; set; }
            public object season_number { get; set; }
            public object stretched_ratio { get; set; }
            public int age_limit { get; set; }
            public int abr { get; set; }
            public object vbr { get; set; }
            public string id { get; set; }
            public string uploader_url { get; set; }
            public string _filename { get; set; }
            public object series { get; set; }
            public List<Thumbnail2> thumbnails { get; set; }
            public string format_id { get; set; }
            public string title { get; set; }
            public string fulltitle { get; set; }
            public string webpage_url { get; set; }
            public object annotations { get; set; }
            public Subtitles2 subtitles { get; set; }
            public object requested_subtitles { get; set; }
            public List<RequestedFormat2> requested_formats { get; set; }
            public object playlist_index { get; set; }
            public object chapters { get; set; }
            public string webpage_url_basename { get; set; }
            public object playlist { get; set; }
            public object resolution { get; set; }
            public List<string> categories { get; set; }
            public string acodec { get; set; }
            public double average_rating { get; set; }
            public int dislike_count { get; set; }
            public string thumbnail { get; set; }
            public object episode_number { get; set; }
            public object alt_title { get; set; }
            public int height { get; set; }
            public string extractor { get; set; }
            public object creator { get; set; }
            public int duration { get; set; }
            public int width { get; set; }
            public AutomaticCaptions2 automatic_captions { get; set; }
            public string format { get; set; }
            public object start_time { get; set; }
            public object is_live { get; set; }
            public object end_time { get; set; }
            public string uploader { get; set; }
            public int view_count { get; set; }
            public string extractor_key { get; set; }
            public List<Format2> formats { get; set; }
            public string display_id { get; set; }
            public int fps { get; set; }
        }
        #endregion
        #region JSON for weather command

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
            public double pressure { get; set; }
            public int humidity { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public double sea_level { get; set; }
            public double grnd_level { get; set; }
        }
        public class Wind
        {
            public double speed { get; set; }
            public double deg { get; set; }
        }
        public class Rain
        {
            public double __invalid_name__3h { get; set; }
        }
        public class Clouds
        {
            public int all { get; set; }
        }
        public class Sys
        {
            public double message { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
        }
        public class RootObjectW2
        {
            public Coord coord { get; set; }
            public List<Weather> weather { get; set; }
            public string @base { get; set; }
            public Main main { get; set; }
            public Wind wind { get; set; }
            public Rain rain { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public Sys sys { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }

        #endregion
        #region JSON for norrisjoke command
        public class RootObjectnorris
        {
            public object category { get; set; }
            public string icon_url { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public string value { get; set; }
        }

        #endregion
        #region JSON for translate command
        public class RootObjectlanguages
        {
            public List<string> dirs { get; set; }
        }
        public class RootObjectresult
        {
            public int code { get; set; }
            public string lang { get; set; }
            public List<string> text { get; set; }
        }

        #endregion
        #region JSON for IMG command
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
        #endregion
        #region JSON for GIF command

        public class Hit
        {
            public int previewHeight { get; set; }
            public int likes { get; set; }
            public int favorites { get; set; }
            public string tags { get; set; }
            public int webformatHeight { get; set; }
            public int views { get; set; }
            public int webformatWidth { get; set; }
            public int previewWidth { get; set; }
            public int comments { get; set; }
            public int downloads { get; set; }
            public string pageURL { get; set; }
            public string previewURL { get; set; }
            public string webformatURL { get; set; }
            public int imageWidth { get; set; }
            public int user_id { get; set; }
            public string user { get; set; }
            public string type { get; set; }
            public int id { get; set; }
            public string userImageURL { get; set; }
            public int imageHeight { get; set; }
        }

        public class GIFRootObject
        {
            public int totalHits { get; set; }
            public List<Hit> hits { get; set; }
            public int total { get; set; }
        }
        #endregion

    }
}
