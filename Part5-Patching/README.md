# PATCH Request

`JSONPatch` is a method to update documents partially on an API, in which the value to be changed is described exactly how we want to modify a document, i.e. we can change a value/ a field of the document without having to send along the rest of the unchanged values.

First of all, we are going to install the following **NuGet** packages:

* Microsoft.AspNetCore.JsonPatch (2.2.0)
* AutoMapper.Extensions.Microsoft.DependencyInjection (6.1.1)

Other than the above NuGet packages, you should have a look back to check if the following NuGet packages are already installed in your project.

* Microsoft.AspNet.WebApi.Core
* Microsoft.AspNetCore.App
* Microsoft.AspNetCore.Razor.Design
* Microsoft.EntityFrameworkCore.Design
* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.NETCore.App
* Swashbuckle.AspNetCore

## Data Access Layer (DAL)

***The Repository***    
The repository is intended to create an abstraction layer between the data access layer (DAL) and the business logic layer of an application. Implementing these patterns can help insulate your application from changes in the data store and can facilitate automated unit testing or test-driven development (TDD). (source: [Microsoft](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application?fbclid=IwAR0339QgLfYu2DuxWmkjsI0y1oxcyaDdZknQvQtP11SMbu_XZjOU5IXNcVI))

The following illustration shows the differences when the controller and context classes use the repository

![Repository](./img/repo1.png)

There are different ways to implement the repository. The approach to implementing an abstraction layer shown in this step is one of the options.

### 1. IVideoRepository

You can refer to this [link](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application?fbclid=IwAR0339QgLfYu2DuxWmkjsI0y1oxcyaDdZknQvQtP11SMbu_XZjOU5IXNcVI) to find out more about Repository and Unit of Work Patterns in an ASP.NET MVC Application

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScribrAPI.Model;

namespace ScribrAPI.DAL
{
    public interface IVideoRepository : IDisposable
    {
        IEnumerable<Video> GetVideos();
        Video GetVideoByID(int VideoId);
        void InsertVideo(Video video);
        void DeleteVideo(int VideoId);
        void UpdateVideo(Video video);
        void Save();
    }
}
```

### 2. VideoRepository
VideoRepository.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScribrAPI.Model;

namespace ScribrAPI.DAL
{
    public class VideoRepository : IVideoRepository, IDisposable
    {
        private scriberContext context;

        public VideoRepository(scriberContext context)
        {
            this.context = context;
        }

        public IEnumerable<Video> GetVideos()
        {
            return context.Video.ToList();
        }

        public Video GetVideoByID(int id)
        {
            return context.Video.Find(id);
        }

        public void InsertVideo(Video video)
        {
            context.Video.Add(video);
        }

        public void DeleteVideo(int videoId)
        {
            Video video = context.Video.Find(videoId);
            context.Video.Remove(video);
        }

        public void UpdateVideo(Video video)
        {
            context.Entry(video).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
```
## Model

### AutoMapper Profile

MapperProfile.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace ScribrAPI.Model
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Video, VideoDTO>();
            CreateMap<VideoDTO, Video>();
        }
    }
}
```
## Video
Video.cs

```csharp
[DataContract]
    public class VideoDTO
    {
        [DataMember]
        public int VideoId { get; set; }

        [DataMember]
        public string VideoTitle { get; set; }

        [DataMember]
        public int VideoLength { get; set; }

        [DataMember]
        public string WebUrl { get; set; }

        [DataMember]
        public string ThumbnailUrl { get; set; }

        [DataMember]
        public bool IsFavourite { get; set; }
    }
```

## Videos Controllers
VideoController.cs

```csharp
//PUT with PATCH to handle isFavourite
        [HttpPatch("update/{id}")]
        public VideoDTO Patch(int id, [FromBody]JsonPatchDocument<VideoDTO> videoPatch)
        {
            //get original video object from the database
            Video originVideo = videoRepository.GetVideoByID(id);
            //use automapper to map that to DTO object
            VideoDTO videoDTO = _mapper.Map<VideoDTO>(originVideo);
            //apply the patch to that DTO
            videoPatch.ApplyTo(videoDTO);
            //use automapper to map the DTO back ontop of the database object
            _mapper.Map(videoDTO, originVideo);
            //update video in the database
            _context.Update(originVideo);
            _context.SaveChanges();
            return videoDTO;
        }
```

## Startup.cs

Add the following code in Startup.cs

```csharp
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

// This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<scriberContext>();
            services.AddAutoMapper(typeof(Startup));

...//code continue
```

`node -v`  

[here.](https://www.npmjs.com/get-npm)  

_`npm init <initializer>`_

http://localhost:3000/

**src**
