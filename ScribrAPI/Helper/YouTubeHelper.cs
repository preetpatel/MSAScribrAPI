﻿using Newtonsoft.Json;
using ScribrAPI.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace ScribrAPI.Helper
{
    class YouTubeHelper
    {
        public static String GetVideoIdFromURL(String videoURL) {
            // Extract the string after the '=' sign
            // e.g. https://www.youtube.com/watch?v=ehvz3iN8pp4 becomes ehvz3iN8pp4 
            int indexOfFirstId = videoURL.IndexOf("=") + 1;
            String videoId = videoURL.Substring(indexOfFirstId);
            return videoId;
        }

        public static List<Transcription> GetTranscriptions(String videoId)
        {
            String YouTubeVideoURL = "https://www.youtube.com/watch?v=" + videoId;
            String HTMLSource = new WebClient().DownloadString(YouTubeVideoURL);

            // Use regular expression to find the link with the transcription
            String pattern = "timedtext.+lang=en";
            Match match = Regex.Match(HTMLSource, pattern);
            String subtitleLink = "https://www.youtube.com/api/" + match;
            subtitleLink = CleanLink(subtitleLink);

            // Use XmlDocument to load the subtitle XML.
            XmlDocument doc = new XmlDocument();
            doc.Load(subtitleLink);
            XmlNode root = doc.ChildNodes[1];

            // Go through each tag and look for start time and phrase.
            List<Transcription> transcriptions = new List<Transcription>();
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    Transcription transcription = new Transcription
                    {
                        StartTime = (int)Convert.ToDouble(root.ChildNodes[i].Attributes["start"].Value),
                        Phrase = root.ChildNodes[i].InnerText
                    };

                    transcriptions.Add(transcription);
                }
            }
            return transcriptions;
        }

        private static String CleanLink(String subtitleURL)
        {
            subtitleURL = subtitleURL.Replace("\\\\u0026", "&");
            subtitleURL = subtitleURL.Replace("\\", "");
            return (subtitleURL);

        }

        public static Video GetVideoInfo(String videoId)
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
            Video video = new Video
            {
                VideoTitle = title,
                WebUrl = videoUrl,
                VideoLength = (int)videoDuration.TotalSeconds,
                IsFavourite = false,
                ThumbnailUrl = thumbnailURL
            };

            return video;
        }
    }
}