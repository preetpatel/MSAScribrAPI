using ScribrAPI.Controllers;
using ScribrAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTestScribrAPI
{
    [TestClass]
    public class TranscriptionsControllerUnitTests
    {
        public static readonly DbContextOptions<scriberContext> options
        = new DbContextOptionsBuilder<scriberContext>()
        .UseInMemoryDatabase(databaseName: "testDatabase")
        .Options;
        
        public static readonly IList<Transcription> transcriptions = new List<Transcription>
        {
            new Transcription()
            {
                Phrase = "That's like calling"
            },
            new Transcription()
            {
                Phrase = "your peanut butter sandwich"
            }
        };

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new scriberContext(options))
            {
                // populate the db
                context.Transcription.Add(transcriptions[0]);
                context.Transcription.Add(transcriptions[1]);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new scriberContext(options))
            {
                // clear the db
                context.Transcription.RemoveRange(context.Transcription);
                context.SaveChanges();
            };
        }

        [TestMethod]
        public async Task TestGetSuccessfully()
        {
            using (var context = new scriberContext(options))
            {
                TranscriptionsController transcriptionsController = new TranscriptionsController(context);
                ActionResult<IEnumerable<Transcription>> result = await transcriptionsController.GetTranscription();

                Assert.IsNotNull(result);
                // i should really check to make sure the exact transcriptions are in there, but that requires an equality comparer, 
                // which requires a whole nested class, thanks to C#'s lack of anonymous classes that implement interfaces
            }
        }

        // unfortunately, it can be hard to avoid test method names that are also descriptive
        [TestMethod]
        public async Task TestPutMemeItemNoContentStatusCode()
        {
            using (var context = new scriberContext(options))
            {
                string title = "this is now a different phrase";
                Transcription transcription1 = context.Transcription.Where(x => x.Phrase == transcriptions[0].Phrase).Single();
                transcription1.Phrase = title;

                TranscriptionsController transcriptionsController = new TranscriptionsController(context);
                IActionResult result = await transcriptionsController.PutTranscription(transcription1.TranscriptionId, transcription1) as IActionResult;

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }
    }
}
