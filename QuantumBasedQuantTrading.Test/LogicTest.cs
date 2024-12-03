using Moq;
using NewsAPI.Constants;
using NewsAPI.Models;
using NewsAPI;
using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Logic.Logics;
using QuantumBasedQuantTrading.Models;
using QuantumBasedQuantTrading.Repository.Interface;

namespace QuantumBasedQuantTrading.Test
{
    [TestFixture]
    public class LogicTest
    {
        AllArticlesLogic articleLogic;
        FullOutLogic fullOutLogic;
        OutputSentLogic outputLogic;
        AverageSentLogic avgLogic;
        BusinessLogic businessLogic;
        Mock<ISettings> mockSettings;
        Mock<IRepository<AllArticles>> mockArticlesRepo;
        Mock<IRepository<FulloutAllArticles>> mockFullArticlesRepo;
        Mock<IRepository<OutputSent>> mockOutputSentRepo;
        Mock<IRepository<AverageSent>> mockAvgSentRepo;
        Mock<IRepository<RequestParameters>> mockReqParamRepo;
        Mock<IRepository<MachineLearningModelData>> mockMLMDRepo;

        [SetUp]
        public void Initialize()
        {
            AllArticles articlesrow1 = new AllArticles { ArticleID = 1, Description = "Neutral statment",
                Title = "Berkshire Hataway", Published_at = new DateTime(2024, 11, 20), URL = "https://google.com" };
            AllArticles articlesrow2 = new AllArticles { ArticleID = 2, Description = "Neutral statment", 
                Title = "Berkshire Hataway", Published_at = new DateTime(2024, 11, 21), URL = "https://google.com" };
            AllArticles articlesrow3 = new AllArticles { ArticleID = 3, Description = "Neutral statment",
                Title = "Berkshire Hataway", Published_at = new DateTime(2024, 11, 22), URL = "" };
            var allarticles = new List<AllArticles>() { articlesrow1, articlesrow2, articlesrow3 }.AsQueryable();

            FulloutAllArticles fullarticlesrow1 = new FulloutAllArticles { ArticleFullID = 1, Description = "Neutral statment", Title = "Berkshire Hataway",
                Published_at = new DateTime(2024, 11, 20), Content="Neutral statment" };
            FulloutAllArticles fullarticlesrow2 = new FulloutAllArticles
            {
                ArticleFullID = 1,
                Description = "Neutral statment",
                Title = "Berkshire Hataway",
                Published_at = new DateTime(2024, 11, 21),
                Content = "Neutral statment"
            };
            FulloutAllArticles fullarticlesrow3 = new FulloutAllArticles
            {
                ArticleFullID = 1,
                Description = "Neutral statment",
                Title = "Berkshire Hataway",
                Published_at = new DateTime(2024, 11, 22),
                Content = ""
            };
            var fullarticles = new List<FulloutAllArticles>() { fullarticlesrow1, fullarticlesrow2, fullarticlesrow3 }.AsQueryable();

            RequestParameters requestParameters1 = new RequestParameters { RequestParamID = 1, Keyword = "Berkshire Hataway", endDate = new DateTime(2024, 11, 22), startDate = new DateTime(2024, 11, 20) };
            var requestParameters = new List<RequestParameters>() { requestParameters1 }.AsQueryable();

            MachineLearningModelData mlData1 = new MachineLearningModelData { MLResultID = 1, Symbol = "BRK-B",
                testMAE = 0.001, valMAE = 0.001, trainMAE= 0.001, predictedValue=415};
            var MLData = new List<MachineLearningModelData>() { mlData1 }.AsQueryable();

            OutputSent row1 = new OutputSent { OutputSentID = 1, ContentSentiment = 0.4, DescriptionSentiment = 0.4, TitleSentiment = 0.4, Published_at = new DateTime(2024, 11, 20) };
            OutputSent row2 = new OutputSent { OutputSentID = 2, ContentSentiment = 0.6, DescriptionSentiment = 0.6, TitleSentiment = 0.6, Published_at = new DateTime(2024, 11, 20) };
            var outpusents = new List<OutputSent>() { row1, row2 }.AsQueryable();

            AverageSent avgrow1 = new AverageSent { AverageSentID = 1, AvgContentSentiment = 0.4, AvgDescriptionSentiment = 0.4, AvgTitleSentiment = 0.4, Published_at = new DateTime(2024, 11, 20) };
            AverageSent avgrow2 = new AverageSent { AverageSentID = 2, AvgContentSentiment = 0.6, AvgDescriptionSentiment = 0.6, AvgTitleSentiment = 0.6, Published_at = new DateTime(2024, 11, 21) };
            AverageSent avgrow3 = new AverageSent { AverageSentID = 3, AvgContentSentiment = 0.4, AvgDescriptionSentiment = 0.4, AvgTitleSentiment = 0.4, Published_at = new DateTime(2024, 11, 22) };
            AverageSent avgrow4 = new AverageSent { AverageSentID = 4, AvgContentSentiment = 0.6, AvgDescriptionSentiment = 0.6, AvgTitleSentiment = 0.6, Published_at = new DateTime(2024, 11, 23) };
            AverageSent avgrow5 = new AverageSent { AverageSentID = 1, AvgContentSentiment = 0.4, AvgDescriptionSentiment = 0.4, AvgTitleSentiment = 0.4, Published_at = new DateTime(2024, 11, 24) };
            AverageSent avgrow6 = new AverageSent { AverageSentID = 2, AvgContentSentiment = 0.6, AvgDescriptionSentiment = 0.6, AvgTitleSentiment = 0.6, Published_at = new DateTime(2024, 11, 25) };
            AverageSent avgrow7 = new AverageSent { AverageSentID = 3, AvgContentSentiment = 0.4, AvgDescriptionSentiment = 0.4, AvgTitleSentiment = 0.4, Published_at = new DateTime(2024, 11, 26) };
            AverageSent avgrow8 = new AverageSent { AverageSentID = 4, AvgContentSentiment = 0.6, AvgDescriptionSentiment = 0.6, AvgTitleSentiment = 0.6, Published_at = new DateTime(2024, 11, 27) };
            var avgsents = new List<AverageSent>() { avgrow1, avgrow2, avgrow3, avgrow4, avgrow5, avgrow6, avgrow7, avgrow8 }.AsQueryable();

            mockSettings = new Mock<ISettings>();
            //mockSettings.Setup(x => x.QvantApi).Returns("API_KEY");
            mockSettings.Setup(x => x.NewsApi).Returns("API_KEY2");
            mockArticlesRepo = new Mock<IRepository<AllArticles>>();
            mockArticlesRepo.Setup(x => x.ReadAll()).Returns(allarticles);
            mockFullArticlesRepo = new Mock<IRepository<FulloutAllArticles>>();
            mockFullArticlesRepo.Setup(x => x.ReadAll()).Returns(fullarticles);
            mockReqParamRepo = new Mock<IRepository<RequestParameters>>();
            mockReqParamRepo.Setup(x => x.ReadAll()).Returns(requestParameters);
            mockMLMDRepo = new Mock<IRepository<MachineLearningModelData>>();
            mockMLMDRepo.Setup(x => x.ReadAll()).Returns(MLData);
            mockOutputSentRepo = new Mock<IRepository<OutputSent>>();
            mockOutputSentRepo.Setup(x => x.ReadAll()).Returns(outpusents);
            mockAvgSentRepo = new Mock<IRepository<AverageSent>>();
            mockAvgSentRepo.Setup(x => x.ReadAll()).Returns(avgsents);



            articleLogic = new AllArticlesLogic(mockArticlesRepo.Object);
            fullOutLogic = new FullOutLogic(mockFullArticlesRepo.Object);
            outputLogic = new OutputSentLogic(mockOutputSentRepo.Object);
            businessLogic = new BusinessLogic(
                mockArticlesRepo.Object,
                mockFullArticlesRepo.Object,
                mockSettings.Object,
                mockOutputSentRepo.Object,
                mockAvgSentRepo.Object,
                mockMLMDRepo.Object
            );

        }

        [Test]
        public void Test_CreateFulloutArticle()
        {
            var fulloutArticle = new FulloutAllArticles
            {
                ArticleFullID = 1,
                Description = "Neutral statement",
                Title = "Berkshire Hathaway neutral statement",
                Published_at = new DateTime(2024, 11, 20),
                Content = "Neutral statement"
            };

            fullOutLogic.Create(fulloutArticle);

            mockFullArticlesRepo.Verify(repo => repo.Create(It.Is<FulloutAllArticles>(
                x => x.ArticleFullID == fulloutArticle.ArticleFullID &&
                     x.Description == fulloutArticle.Description &&
                     x.Title == fulloutArticle.Title &&
                     x.Published_at == fulloutArticle.Published_at &&
                     x.Content == fulloutArticle.Content
            )), Times.Once);
        }
        [Test]
        public void Test_CreateFulloutArticle_NullObject()
        {
            Assert.Throws<ArgumentNullException>(() => fullOutLogic.Create(null));
        }
        [Test]
        public void Test_CreateFulloutArticle_NullObject_Message()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => fullOutLogic.Create(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'item')", exception.Message, "Exception message mismatch.");
        }

        [Test]
        public void Test_GetAllArticles()
        {
            var articles = articleLogic.ReadAll();
            Assert.AreEqual(3, articles.Count(), "Expected 3 articles to be retrieved.");
            Assert.IsTrue(articles.Any(a => a.ArticleID == 1), "Article with ID 1 not found.");
        }
        [Test]
        public void Test_CreateArticle_WithRequiredFields()
        {
            var validArticle = new AllArticles
            {
                ArticleID = 4, 
                Title = "Required Title",  
                Published_at = new DateTime(2024, 11, 22)  
            };

            articleLogic.Create(validArticle);

            mockArticlesRepo.Verify(repo => repo.Create(It.Is<AllArticles>(
                x => x.ArticleID == validArticle.ArticleID &&
                     x.Title == validArticle.Title &&
                     x.Published_at == validArticle.Published_at
            )), Times.Once, "The article with required fields was not created.");
        }
        [Test]
        public void Test_DeleteArticle()
        {
            articleLogic.Delete(1);
            mockArticlesRepo.Verify(repo => repo.Delete(It.Is<int>(id => id == 1)), Times.Once, "Delete was not called with the correct ID.");
        }

        [Test]
        public async Task MlSubprocess_ShouldThrowException_WhenPythonProcessFails()
        {
            var mockData = new List<AverageSent>
            {
                new AverageSent { Published_at = DateTime.UtcNow.Date, AvgTitleSentiment = 0.6, AvgContentSentiment = 0.7, AvgDescriptionSentiment = 0.5 }
            }.AsQueryable();

            mockAvgSentRepo.Setup(repo => repo.ReadAll()).Returns(mockData);
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await businessLogic.MlSubprocess("AAPL", 0.6f, 0.7f, 0.5f, 150, 155, 145, 100000)
            );
            StringAssert.Contains("resulting train set will be empty", exception.Message);
        }
        [Test]
        public async Task Test_AutoCollectNews_ShouldThrowException_ForInvalidApiResponse()
        {
            var requestParameters = new RequestParameters
            {
                Keyword = "Berkshire Hathaway",
                startDate = new DateTime(2024, 11, 20),
                endDate = new DateTime(2024, 11, 21)
            };

            var mockNewsApiClient = new Mock<NewsApiClient>("INVALID_API_KEY");

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await businessLogic.AutoCollectNews(requestParameters)
            );

            StringAssert.Contains("Your API key is invalid or incorrect.", exception.Message);
        }
        [Test]
        public async Task Test_MlSubprocess_ShouldThrowException_ForEmptySentimentData()
        {
            mockAvgSentRepo.Setup(repo => repo.ReadAll()).Returns(Enumerable.Empty<AverageSent>().AsQueryable());

            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await businessLogic.MlSubprocess("AAPL", 0.6f, 0.7f, 0.5f, 150, 155, 145, 100000)
            );

            StringAssert.Contains("Sequence contains no elements", exception.Message, "Exception message mismatch.");
        }

    }
}