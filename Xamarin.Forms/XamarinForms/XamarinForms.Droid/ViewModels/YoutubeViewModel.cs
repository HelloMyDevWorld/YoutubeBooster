using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XamarinForms.Models;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Android.Graphics;
using System.IO;
using System.Text.RegularExpressions;

namespace XamarinForms.ViewModels
{
    public class YoutubeViewModel : INotifyPropertyChanged
    {
        private const string ApiKey = "AIzaSyDHKxh3EuJlQPpBomX0LOK0IA5q7PySeLM";
        private string apiUrlForVideosDetails = "https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics&id="
            + "{0}"
            + "&key="
            + ApiKey;

        private List<YoutubeItem> _youtubeItems;
        private string search;
        public List<YoutubeItem> YoutubeItems
        {
            get { return _youtubeItems; }
            set
            {
                _youtubeItems = value;
                OnPropertyChanged();
            }
        }

        public YoutubeViewModel(string search)
        {
            this.search = search;
            InitDataAsync();
        }

        public async Task InitDataAsync()
        {
            var videoIds = await GetVideoIds();
        }


        private async Task<List<string>> GetVideoIds()
        {
            List<string> videos = new List<string>();
            try
            {
                YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyDBM5kWALJfZljMRj2M03R2oGCzcvVRFfI",
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = search;
                searchListRequest.MaxResults = 10;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            videos.Add(searchResult.Id.VideoId);
                            break;
                    }
                }
                YoutubeItems = await GetVideosDetailsAsync(videos);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return videos;
        }

        private async Task<List<YoutubeItem>> GetVideosDetailsAsync(List<string> videoIds)
        {

            var videoIdsString = "";
            foreach (var s in videoIds)
            {
                videoIdsString += s + ",";
            }

            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(string.Format(apiUrlForVideosDetails, videoIdsString));

            var youtubeItems = new List<YoutubeItem>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);

                var items = response.Value<JArray>("items");

                foreach (var item in items)
                {
                    var snippet = item.Value<JObject>("snippet");
                    var statistics = item.Value<JObject>("statistics");

                    var youtubeItem = new YoutubeItem
                    {
                        background = "#ff0000",
                        isenablebutton = "true",
                        textbutton = "Download",
                        Title = snippet.Value<string>("title"),
                        Description = snippet.Value<string>("description"),
                        ChannelTitle = snippet.Value<string>("channelTitle"),
                        PublishedAt = snippet.Value<DateTime>("publishedAt"),
                        VideoId = item?.Value<string>("id"),
                        DefaultThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("default")?.Value<string>("url"),
                        MediumThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("medium")?.Value<string>("url"),
                        HighThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("high")?.Value<string>("url"),
                        StandardThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("standard")?.Value<string>("url"),
                        MaxResThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("maxres")?.Value<string>("url"),

                        ViewCount = statistics?.Value<int>("viewCount"),
                        LikeCount = statistics?.Value<int>("likeCount"),
                        DislikeCount = statistics?.Value<int>("dislikeCount"),
                        FavoriteCount = statistics?.Value<int>("favoriteCount"),
                        CommentCount = statistics?.Value<int>("commentCount"),

                        Tags = (from tag in snippet?.Value<JArray>("tags") select tag.ToString())?.ToList(),
                    };

                    var url = @"/data/data/com.youtube.xam/files/";
                //    var format = ".mp4";
                    foreach (var el in Directory.GetFiles(url))
                    {
                        if (el.ToString().Contains(YoutubeViewModel.RemoveIllegalPathCharacters(youtubeItem.Title)))
                        { 
                                youtubeItem.isenablebutton = "false";
                                youtubeItem.background = "#2C6700";
                                youtubeItem.textbutton = "Downloaded";
                            }
                        
                    }

                    youtubeItems.Add(youtubeItem);
                }

                return youtubeItems;
            }
            catch (Exception exception)
            {
                return youtubeItems;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

    }
}