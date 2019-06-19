using Newtonsoft.Json;
using ScribrAPI.Model;
using System;
using System.Net;
using System.Xml;

namespace YouTubeHelper
{
    class YouTubeHelper
    {
        static void Main(String[] args) {
            getVideoInfo("BZbChKzedEk");
            Console.ReadLine();
        }

        static Video getVideoInfo(String videoId)
        {
            String APIKey = "AIzaSyBR8fLO7PlD_nttYr7P70c3gMSLyQuGHxg";
            String YouTubeAPIURL = "https://www.googleapis.com/youtube/v3/videos?id=" + videoId + "&key=" + APIKey + "&part=snippet,contentDetails";

            // Use an http client to grab the JSON string from the web.
            String videoInfoJSON = new WebClient().DownloadString(YouTubeAPIURL);

            // Using dynamic object helps us to more effciently extract infomation from a large JSON String.
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(videoInfoJSON);

            // Extract information from the dynamic object.
            String title = jsonObj["items"][0]["snippet"]["title"];
            String thumbnailURL = jsonObj["items"][0]["snippet"]["thumbnails"]["medium"]["url"];
            String durationString = jsonObj["items"][0]["contentDetails"]["duration"];
            String videoUrl = "https://www.youtube.com/watch?v=" + videoId;

            // duration is given in this format: PT4M17S, we need to use a simple parser to get the duration in seconds.
            TimeSpan videoDuration = XmlConvert.ToTimeSpan(durationString);
            int duration = (int)videoDuration.TotalSeconds;

            // Create a new Video Object from the model defined in the API.
            Video video = new Video();
            video.VideoTitle = title;
            video.WebUrl = videoUrl;
            video.VideoLength = (int)videoDuration.TotalSeconds;
            video.IsFavourite = false;
            video.ThumbnailUrl = thumbnailURL;

            return video;
        }
    }
}