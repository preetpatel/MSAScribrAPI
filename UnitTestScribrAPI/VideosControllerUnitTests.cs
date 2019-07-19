using ScribrAPI.Controllers;
using ScribrAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


namespace UnitTestScribrAPI
{
    class VideosControllerUnitTests
    {
        public static readonly IList<Video> videos = new List<Video>
        {
            new Video()
            {
                VideoTitle = "video1",
                WebUrl = "https://notreal.fakedomain/video",
                ThumbnailUrl = "https://alsofake.co.fake/thumbnail"
            }
        };
    }
}
