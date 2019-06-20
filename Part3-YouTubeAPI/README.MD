YouTube provides an official API for getting information on a YouTube video. This is known as the `YouTube Data API V3`. Our first goal to create a program that retrieves information from this API and store it in our database.

To start using the YouTube API, take a look at this link
https://developers.google.com/youtube/v3/getting-started

We will need to create a project on Google's Developer console: https://console.developers.google.com/

On the search bar on the top, search for YouTube Data API V3, then choose add credentials to your project.

Again, choose YouTube Data API V3, We will be calling the API from "Other UI" and we will be accessing public data.

After you've done that, you should have an API key, this API key allows us to make the API request to YouTube.

If you've read the documentation on the YouTube API, you would find that the following link would give us what we need to store in the database.

https://www.googleapis.com/youtube/v3/videos?id=`VIDEOIDHERE`&key=`APIKEYHERE`&part=snippet,contentDetails

What the API returns in a JSON formatted string which we can manipulate easily in ASP.NET.

In order to use this API in ASP.NET, we need to create a so-called helper class, the purpose of this helper class is to deal with all YouTube API retrieving/crawling. Create a new folder in the project called Helper and add a new class under that folder, we will call it `YouTubeHelper`, paste in the following code

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScribrAPI.Model;

namespace ScribrAPI.Helper
{
    public class YouTubeHelper
    {
        public static void Main(string[] args){
            GetVideoInfo("BZbChKzedEk");
        }

        public static void GetVideoInfo(String videoId)
        {

        }
    }
}
```

There are now two entry points in the program, we need to disable the entry point into the API. Navigate to `Program.cs` as change the method `Main()` to `Main1()` as shown below. We will change this back later.

```C#
public static void Main1(string[] args)
{
    CreateWebHostBuilder(args).Build().Run();
}
``` 


Install `Newtonsoft.Json` from NuGet manager, we will be using this to deal with the content that eh YouTube API returns.

**The purpose of `GetVideoInfo()` is to return a Video object that the API can use to create an entry in the database.**

We will first fetch the JSON into our application. Add the following code in the `GetVideoInfo()`

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

Note that this method returns a video object, so we need to change the method signature to tell the compiler that we are returning an video object. Change `public static void GetVideoInfo(String videoId)` to `public static Video GetVideoInfo(String videoId)`

Now that the method is done, we can test this by calling this method in `Main()`
```C#
public static void Main(String[] args) {
    GetVideoInfo("BZbChKzedEk");
}
```

Set a breakpoint in the last line of the method so we can take a look at the video object we've constructed, we can do this by clicking on the empty space on the left until the red dot shown below appears.
![alt text](Images/setBreakpoint.PNG "setBreakpoint")

If we run the program now, we can see that the video object contains the information we need.
![alt text](Images/videoObject.PNG "videoObject")

We are done with constructing the Video object! We are now ready to incorporate this little helper function into our API.

We want to use the `YouTubeHelper` whenever we receive a POST request in the `/Video` endpoint.

To do this, we need to go back to the API project.
Delete the `Main()` in YouTubeHelper and change `Main1()` back to `Main()` in `Program.cs` so that the API launches again normally.


We now need to modify the VideoController, navigate to the method that handles POST request in VideoController. Instead of receiving a whole video object from the request body, we will edit this method so that it takes an YouTube URL.

In order to process a POST request, we first need to obtain the request body so we know what information to process. Modify the signature of the `PostVideo()` to the following
```c#
public async Task<ActionResult<Video>> PostVideo([FromBody]dynamic data)
```
Again, this is similar to what we did before, we are converting a string of text to a dynamic object so that it gets resolved at runtime.
We'll extract the URL from the dynamic object

```C#
String URL = data["url"];
```

These two lines above tells the program to extract the value from the "url" key in the json request body.

Now that we have the URL, we can finally call the `GetVideoInfo(String videoId)` in `YouTubeHelper`. Wait... Can we? Note our plan is to pass the entire YouTube url in the request body, but, `GetVideoInfo()` only deals with videoId, not the entire Video link.

We will need to create another method in the helper class to extract video id from the video link.

```C#
static String GetVideoIdFromURLURLURLURLURLURLURLURLURL(String videoURL) {
    // Extract the string after the = sign
    // e.g. https://www.youtube.com/watch?v=ehvz3iN8pp4 becomes ehvz3iN8pp4 
    int indexOfFirstId = videoURL.IndexOf("=") + 1;
    String videoId = videoURL.Substring(indexOfFirstId);
    return videoId;
}
```

We will now call the two methods we created in our `PostVideo()` in `VideoController.cs`, your code should be similar to the following:
```C#
// POST: api/Videos
[HttpPost]
public async Task<ActionResult<Video>> PostVideo([FromBody]dynamic data)
{
    // Constructing the video object from our helper function
    String videoURL = data["url"];
    String videoId = YouTubeHelper.GetVideoIdFromURLURLURLURLURLURLURLURL(videoURL);
    Video video = YouTubeHelper.GetVideoInfo(videoId);

    // Add this video object to the database
    _context.Video.Add(video);
    await _context.SaveChangesAsync();

    // Return success code and the info on the video object
    return CreatedAtAction("GetVideo", new { id = video.VideoId }, video);
}
```
If we launch the application now and pass the following JSON in the request body
```JSON
{
    "url" : "https://www.youtube.com/watch?v=-Cu3wW8rNAs"
}
```

We would receive a response with the video info.
![alt text](Images/PostVideoJson.PNG "PostVideo JSON response")

And if you make a GET request to /Videos in Swagger UI, you would see your newly added video.