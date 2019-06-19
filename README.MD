YouTube provides an official API for getting information on a YouTube video. This is known as the `YouTube Data API V3`. Our first goal of the web crawler is to retrieve information from this API and store it in our database.

To start using the YouTube API, take a look at this link
https://developers.google.com/youtube/v3/getting-started

We will need to create a project on Google's Developer console: https://console.developers.google.com/

On the search bar on the top, search for YouTube Data API V3, then choose add credentials to your project.

Again, choose YouTube Data API V3, We will be calling the API from "Other UI" and we will be accessing public data.

After you've done that, you should have an API key, this API key allows us to make the API request to YouTube.

If you've read the documentation on the YouTube API, you would find that the following link would give us what we need to store in the database.

https://www.googleapis.com/youtube/v3/videos?id=`VIDEOIDHERE`&key=`APIKEYHERE`&part=snippet,contentDetails

What the API returns in a JSON formatted string which we can manipulate easily in ASP.NET.

In order to use this API in ASP.NET, we need to create a new console app. Right click on the API Solution in Visual Studio and add a new project. Choose `Console App (.NET Core)` and name it YouTubeHelper. Rename the newly generated Program.cs under YouTubeHelper namespace to YouTubeHelper and paste in the following start code:

```C#
using System;
using ScribrAPI.Model;

namespace YouTubeHelper
{
    class YouTubeHelper
    {
        static void Main(String[] args) {
            getVideoInfo("videoId");
            Console.ReadLine();
        }

        static void getVideoInfo(String videoId)
        {
            Console.WriteLine(videoId);
        }
    }
}
```

The `getVideoInfo()` will be called from our API and the `Main()` is for us to test the code.

Right click on YouTubeHelper project and select set as start project.

Right click on YouTubeHelper project again and go to build dependencies - Project dependencies and tick ScribrAPI as we will need to use the Video Model later.

Start the application and you should be able to see the string videoId shown in the app.

We are now ready to make the API call.

Install `Newtonsoft.Json` from NuGet manager.


**The purpose of this method is to return a Video object so that the API can use it to create an entry in the database.**

We will first fetch the JSON into our application. Add the following code in the `getVideoInfo()`

```C#
String APIKey = "YOURAPIKEY";
String YouTubeAPIURL = "https://www.googleapis.com/youtube/v3/videos?id=" + videoId + "&key=" + APIKey + "&part=snippet,contentDetails";

// Use an http client to grab the JSON string from the web.
String videoInfoJSON = new WebClient().DownloadString(APIurl);

// Using dynamic object helps us to more effciently extract infomation from a large JSON String.
dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(videoInfoJSON);
```

The videoInfoJSON String has been parsed into a dynamic object which would allow us to manipulate the information easily. Next we will extract the necessary information from the jsonObj

```C#
// Extract information from the dynamic object.
String title = jsonObj["items"][0]["snippet"]["title"];
String thumbnailURL = jsonObj["items"][0]["snippet"]["thumbnails"]["medium"]["url"];
String durationString = jsonObj["items"][0]["contentDetails"]["duration"];
String videoUrl = "https://www.youtube.com/watch?v=" + videoId;

// duration is given in this format: PT4M17S, we need to use a simple parser to get the duration in seconds.
TimeSpan videoDuration = XmlConvert.ToTimeSpan(durationString);
int duration = (int) videoDuration.TotalSeconds;
```

Now that we have everything we need, we will create a Video object from our API model!
```C#
// Create a new Video Object from the model defined in the API.
Video video = new Video();
video.VideoTitle = title;
video.WebUrl = videoUrl;
video.VideoLength = (int)videoDuration.TotalSeconds;
video.IsFavourite = false;
video.ThumbnailUrl = thumbnailURL;
return video;
```

Note that this method returns a video object, so we need to change the method signature to tell the compiler that we are returning an video object. Change `static void getVideoInfo(String videoId)` to `static Video getVideoInfo(String videoId)`

Now that the method is done, we can test this by calling this method in `Main()`
static void Main(String[] args) {
    getVideoInfo("BZbChKzedEk");
    Console.ReadLine();
}

Set a breakpoint in the last line of the method so we can take a look at the video object we've constructed, we can do this by clicking on the empty space on the left until the red dot shown below appears.
![alt text](images/setBreakpoint.PNG "setBreakpoint")

If we run the program now, we can see that the video object contains the information we need.
![alt text](images/videoObject.PNG "videoObject")

We are done with constructing the Video info! We are now ready to incorporate this little helper function into our API.