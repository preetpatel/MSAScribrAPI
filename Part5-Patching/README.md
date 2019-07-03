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

## 1.Data Access Layer (DAL)

***The Repository***    
The repository is intended to create an abstraction layer between the data access layer (DAL) and the business logic layer of an application. Implementing these patterns can help insulate your application from changes in the data store and can facilitate automated unit testing or test-driven development (TDD). (source: [Microsoft](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application?fbclid=IwAR0339QgLfYu2DuxWmkjsI0y1oxcyaDdZknQvQtP11SMbu_XZjOU5IXNcVI))

The following illustration shows the differences when the controller and context classes use the repository

![Repository](./img/repo1.png)

You can refer to this [link](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application?fbclid=IwAR0339QgLfYu2DuxWmkjsI0y1oxcyaDdZknQvQtP11SMbu_XZjOU5IXNcVI) to find out more about Repository and Unit of Work Patterns in an ASP.NET MVC Application

There are different ways to implement the repository. The approach to implementing an abstraction layer shown in this step is one of the options.

### Creating The Video Repository Class

In the DAL folder, create a class file named `IVideoRepository.cs` with the following code:

```csharp
using System;
using System.Collections.Generic;
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

This code declares a typical set of CRUD methods (Create, Read, Update, and Delete), including two read methods:
* `GetVideos()` method returns all Video entities, and
* `GetVideoByID(int VideoId)` finds a single Video entity by ID.

In the DAL folder, create a class file named `VideoRepository.cs` file, which implements the IVideoRepository interface, with the following code:

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

The database context is defined in a class variable, and the constructor expects a parameter of the context:

```csharp
private scriberContext context;

public VideoRepository(scriberContext context)
{
    this.context = context;
}
```
### Change the Videos Controller to Use the Repository

In VideosController.cs, add the following code - the Video Repository - as a IVideoRepository instance and call it in the constructor `VideosController(scriberContext context, IMapper mapper)`.

```csharp
using Microsoft.AspNetCore.Mvc;
using ScribrAPI.DAL;

//some existing code lines

public class VideosController : ControllerBase
    {
        private IVideoRepository videoRepository;

        public VideosController(scriberContext context, IMapper mapper)
            {
                this.videoRepository = new VideoRepository(new scriberContext());
            }
```

## 2. Model

### Using Automapper In ASP.net Core

#### Setup

From the `Package Manager Console`, we will install the following Nuget package :

`Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection`

This will also in turn install the Automapper nuget package if you don’t have it already.

Inside your ConfigureServices method of your startup.cs, add a call to add the AutoMapper required services like so :

```csharp
public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<scriberContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAutoMapper(typeof(Startup));
//some existing code lines
```
#### Adding AutoMapper Profile

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

```csharp
public class VideosController : ControllerBase
    {
        private IVideoRepository videoRepository;
        private readonly IMapper _mapper;
        private readonly scriberContext _context;

        public VideosController(scriberContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            this.videoRepository = new VideoRepository(new scriberContext());
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

**src**
