using QuantumBasedQuantTrading.Models;
using QuantumBasedQuantTrading.Repository.Interface;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsAPI;
using System.Net.Http;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using QuantumBasedQuantTrading.Logic.Interface;
using HtmlAgilityPack;
using System.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace QuantumBasedQuantTrading.Logic.Logics
{
    public class BusinessLogic : IBusinessLogic
    {
        IRepository<AllArticles> repository;
        IRepository<FulloutAllArticles> repositoryFullo;
        IRepository<OutputSent> outSentRepo;
        IRepository<AverageSent> avgSentRepo;
        IRepository<MachineLearningModelData> machineLearningModelDataRepo;
        IRepository<RequestParameters> reqparamsrepo;
        private readonly ISettings _settings;
        public BusinessLogic(IRepository<AllArticles> repo, IRepository<FulloutAllArticles> repositoryFullo,ISettings settings, IRepository<OutputSent> outSentRepo, IRepository<AverageSent> avgSentRepo, IRepository<MachineLearningModelData> machineLearningModelDataRepo)
        {
            this.repository = repo;
            this.repositoryFullo = repositoryFullo;
            this.outSentRepo = outSentRepo;
            this.avgSentRepo = avgSentRepo;
            this.machineLearningModelDataRepo= machineLearningModelDataRepo;
            this._settings= settings;
        }
        public async Task AutoCollectNews(RequestParameters item)
        {
            this.reqparamsrepo.Create(item);
            DateTime currentDate = item.startDate;
            DateTime endDate = item.endDate;
            int intervalDays = 1;
            string newsapi = _settings.NewsApi;
            var newsApiClient = new NewsApiClient(newsapi);
            while (currentDate <= endDate)
            {
                DateTime intervalEnd = currentDate.AddDays(intervalDays - 1) <= endDate
                    ? currentDate.AddDays(intervalDays - 1)
                    : endDate;

                var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
                {
                    Q = item.Keyword,
                    From = currentDate,
                    To = intervalEnd,
                    SortBy = SortBys.PublishedAt,
                    PageSize = 100
                });

                if (articlesResponse.Status == Statuses.Ok)
                {
                    if (articlesResponse.TotalResults > 0)
                    {
                        foreach (var article in articlesResponse.Articles)
                        {
                            var articleData = new AllArticles
                            {
                                Title = article.Title,
                                Description = article.Description,
                                URL = article.Url,
                                Published_at = article.PublishedAt ?? DateTime.MinValue
                            };

                            await this.repository.CreateAsync(articleData);
                            await FillTheMissingContent(articleData);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"No articles found from {currentDate:yyyy-MM-dd} to {intervalEnd:yyyy-MM-dd}.");
                    }
                }
                else
                {
                    throw new Exception(
                                    $"Failed to fetch articles for {currentDate:yyyy-MM-dd} to {intervalEnd:yyyy-MM-dd}. " +
                                    $"Error: {articlesResponse.Error?.Message ?? "Unknown error"}"
                                );
                }

                currentDate = currentDate.AddDays(intervalDays);
            }
            await CalculateAndSaveAverageSentimentsAsync();
        }
        #region FullInfo
        private async Task FillTheMissingContent(AllArticles article)
        {
            try
            {
                string ArticleContent = "";
                if (string.IsNullOrWhiteSpace(article.URL))
                {
                    throw new ArgumentException("Article URL cannot be null or empty.", nameof(article.URL));
                }
                else
                {
                    ArticleContent = await ExtractTextFromUrlAsync(article.URL);
                    ArticleContent = HttpUtility.HtmlDecode(ArticleContent);
                }
                var fullarticle = new FulloutAllArticles
                {
                    Title = article.Title,
                    Description = article.Description,
                    Published_at = article.Published_at,
                    Content = ArticleContent
                };
                await this.repositoryFullo.CreateAsync(fullarticle);
                await AnalyzeSentimentAsync(fullarticle);
            }
            catch (Exception ex)
            {
            }
        }
        private static async Task<string> ExtractTextFromUrlAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string htmlContent = await response.Content.ReadAsStringAsync();

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);


                    var paragraphs = doc.DocumentNode.SelectNodes("//p");
                    if (paragraphs == null)
                    {
                        return string.Empty;
                    }

                    return string.Join(" ", paragraphs.Select(p => p.InnerText));
                }
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"Error fetching content from {url}. HTTP issue: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching content from {url}. Details: {ex.Message}", ex);
            }
        }
        #endregion

        #region Sentiment
        private async Task AnalyzeSentimentAsync(FulloutAllArticles fullarticle)
        {
            var input = new
            {
                Title = fullarticle.Title,
                Description = fullarticle.Description,
                Content = fullarticle.Content,
                Published_at = fullarticle.Published_at.ToString("yyyy-MM-dd")
            };
            string jsonInput = JsonSerializer.Serialize(input);


            string pythonExePath = "python";
            string relativePath = "QuantumBasedQuantTrading.AvgSent\\QuantumBasedQuantTrading.AvgSent.py";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string projectRoot = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            if (!projectRoot.EndsWith("\\"))
            {
                projectRoot += "\\";
            }
            string absolutePath = Path.Combine(projectRoot, relativePath);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = absolutePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(psi);
            string test = process.StartInfo.Arguments;
            using StreamWriter inputWriter = process.StandardInput;
            using StreamReader outputReader = process.StandardOutput;
            using StreamReader errorReader = process.StandardError;

            try
            {
                await inputWriter.WriteLineAsync(jsonInput);
                await inputWriter.FlushAsync();

                string output = await outputReader.ReadLineAsync();

                if (string.IsNullOrEmpty(output))
                {
                    string errorOutput = await errorReader.ReadToEndAsync();
                    throw new Exception($"Python script error: {errorOutput}");
                }

                var sentimentResult = JsonSerializer.Deserialize<OutputSent>(output);

                if (sentimentResult == null)
                {
                    throw new Exception("Failed to deserialize sentiment analysis result.");
                }

                sentimentResult.Published_at = fullarticle.Published_at;

                await this.outSentRepo.CreateAsync(sentimentResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error At Python Details: {ex.Message}", ex);
            }
            finally
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
        }
        #endregion
        #region AvgSent
        private async Task CalculateAndSaveAverageSentimentsAsync()
        {
            try
            {
                var outputSents = outSentRepo.ReadAll();

                var averageSentiments = await outputSents
                    .GroupBy(o => o.Published_at.Date)
                    .Select(g => new AverageSent
                    {
                        Published_at = g.Key,
                        AvgTitleSentiment = g.Average(o => o.TitleSentiment),
                        AvgContentSentiment = g.Average(o => o.ContentSentiment),
                        AvgDescriptionSentiment = g.Average(o => o.DescriptionSentiment)
                    })
                    .ToListAsync();

                foreach (var avgSent in averageSentiments)
                {
                    await this.avgSentRepo.CreateAsync(avgSent);
                }

                Console.WriteLine("Average sentiments calculated and saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        #endregion
        #region RUN
        public async Task MlSubprocess(string Symbol, float titleSentiment, float contSentiment, float descSentiment, float open,
            float currentHighPrice, float currentLowPrice, float currentVolume)
        {
            var sentimentData = avgSentRepo.ReadAll();

            var startDate = sentimentData.Min(x => x.Published_at).ToString("yyyy-MM-dd");
            var endDate = sentimentData.Max(x => x.Published_at).ToString("yyyy-MM-dd");

            var inputData = new
            {
                sentiment_data = sentimentData.Select(x => new
                {
                    date = x.Published_at.ToString("yyyy-MM-dd"),
                    title_sentiment = Double.Parse(x.AvgTitleSentiment.ToString(), CultureInfo.InvariantCulture),
                    content_sentiment = Double.Parse(x.AvgContentSentiment.ToString(), CultureInfo.InvariantCulture),
                    description_sentiment = Double.Parse(x.AvgDescriptionSentiment.ToString(), CultureInfo.InvariantCulture)
                }),
                symbol = Symbol,
                start_date = startDate,
                end_date = endDate,
                titleSent = titleSentiment,
                contSent = contSentiment,
                descSent = descSentiment,
                openPrice = open,
                currentHigh = currentHighPrice,
                currentLow = currentLowPrice,
                volume = currentVolume,
            };
            string jsonInput = JsonSerializer.Serialize(inputData);

            string relativePath = "QuantumBasedQuantTrading.AvgSent\\QuantAlgo.py";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string projectRoot = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            if (!projectRoot.EndsWith("\\"))
            {
                projectRoot += "\\";
            }
            string absolutePath = Path.Combine(projectRoot, relativePath);
            string api= _settings.QvantApi;

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = absolutePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                StandardErrorEncoding= Encoding.UTF8,
                StandardOutputEncoding= Encoding.UTF8,
                EnvironmentVariables = { { "IBM_QUANTUM_API_KEY", api }, { "TF_ENABLE_ONEDNN_OPTS", "0" }, { "TF_CPP_MIN_LOG_LEVEL", "3" } }
            };
            using (Process process = Process.Start(start))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start subprocess.");
                } 
                using (var writer = process.StandardInput)
                {
                    await writer.WriteAsync(jsonInput);
                    await writer.FlushAsync();
                }

                string output;
                using (var reader = process.StandardOutput)
                {
                    output = await reader.ReadToEndAsync();
                }

                string errorOutput = string.Empty;
                using (var errorReader = process.StandardError)
                {
                    errorOutput = await errorReader.ReadToEndAsync();
                }

                await process.WaitForExitAsync();

                string filteredErrors = string.Join(
                    Environment.NewLine,
                    errorOutput.Replace("\r\n", "\n") 
                               .Split('\n')
                               .Where(line => !string.IsNullOrWhiteSpace(line) &&
                                              !line.Contains("oneDNN custom operations") &&
                                              !line.Contains("TensorFlow binary is optimized") &&
                                              !line.Contains("tf.placeholder is deprecated") &&
                                              !line.Contains("TimedeltaIndex")&&
                                              !line.Contains("AVX2 FMA")&&
                                              !line.Contains("tf.train.import_meta_graph")&&
                                              !line.Contains("tf.compat.v1.reset_default_graph instead"))
                );


                if (!string.IsNullOrEmpty(filteredErrors))
                {
                    throw new InvalidOperationException($"Subprocess Error: {filteredErrors}");
                }

                try
                {
                    output = output.Trim();

                    int jsonStartIndex = output.LastIndexOf('{');

                    if (jsonStartIndex == -1)
                    {
                        throw new InvalidOperationException("No JSON object found in subprocess output.");
                    }

                    string jsonOutput = output.Substring(jsonStartIndex);

                    var metrics = JsonSerializer.Deserialize<Dictionary<string, double>>(jsonOutput);

                    if (metrics == null)
                    {
                        throw new InvalidOperationException("Deserialized JSON is null.");
                    }
                    var mlresultdata = new MachineLearningModelData
                    {
                        Symbol = Symbol,
                        trainMAE = metrics["train_mae"],
                        valMAE = metrics["val_mae"],
                        testMAE = metrics["test_mae"],
                        predictedValue = metrics["predicted_value"]
                    };
                    await this.machineLearningModelDataRepo.CreateAsync(mlresultdata);
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"Error parsing subprocess output: {ex.Message}. Output: {output}");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Unexpected error: {ex.Message}");
                }
            }
        }
        #endregion
    }
}
