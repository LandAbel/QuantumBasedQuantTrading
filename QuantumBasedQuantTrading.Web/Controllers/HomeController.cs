using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuantumBasedQuantTrading.Web.Models;
using System.Diagnostics;
using QuantumBasedQuantTrading.Models;
using QuantumBasedQuantTrading.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Text.Json;
using System.Globalization;
using QuantumBasedQuantTrading.Logic.Interface;

namespace QuantumBasedQuantTrading.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly QuantumBasedQuantTradingDbContext _db;
        private readonly IBusinessLogic _businessLogic;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, QuantumBasedQuantTradingDbContext db,IBusinessLogic businessLogic, IEmailSender emailSender)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _businessLogic= businessLogic;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> DelegateAdmin()
        {
            var principal = this.User;
            var user = await _userManager.GetUserAsync(principal);
            var role = new IdentityRole()
            {
                Name = "Admin"
            };
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(role);
            }
            await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAdmin(string uid)
        {
            var user = _userManager.Users.FirstOrDefault(t => t.Id == uid);
            await _userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction(nameof(Users));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GrantAdmin(string uid)
        {
            var user = _userManager.Users.FirstOrDefault(t => t.Id == uid);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
            {
                return BadRequest("Failed to assign role");
            }

            return RedirectToAction(nameof(Users));
        }
        public IActionResult Users()
        {
            return View(_userManager.Users);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        #region Add Normal News

        [Authorize(Roles = "Admin")]
        public IActionResult AddAllNews()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAllNews([FromForm] AllArticles article)
        {
            try
            {
                _db.AllArticlesSet.Add(article);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Your manual entry has been successfully processed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(AddAllNews));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAllNewsFromFile(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                TempData["Error"] = "Please upload a valid JSON file.";
                return RedirectToAction(nameof(AddAllNews));
            }

            try
            {
                using var stream = new StreamReader(jsonFile.OpenReadStream());
                var jsonContent = await stream.ReadToEndAsync();

                var articles = System.Text.Json.JsonSerializer.Deserialize<List<AllArticles>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (articles != null && articles.Any())
                {
                    _db.AllArticlesSet.AddRange(articles);
                    await _db.SaveChangesAsync();
                    TempData["Message"] = "Your JSON file was successfully processed.";
                }
                else
                {
                    TempData["Error"] = "The uploaded file does not contain valid article data.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToAction(nameof(AddAllNews));
        }
        #endregion

        #region Add Full News

        [Authorize(Roles = "Admin")]
        public IActionResult AddFullNews()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddFullNews([FromForm] FulloutAllArticles article)
        {
            try
            {
                _db.FulloutAllArticlesSet.Add(article);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Your manual entry has been successfully processed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(AddFullNews));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddFullNewsFromFile(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                TempData["Error"] = "Please upload a valid JSON file.";
                return RedirectToAction(nameof(AddFullNews));
            }

            try
            {
                using var stream = new StreamReader(jsonFile.OpenReadStream());
                var jsonContent = await stream.ReadToEndAsync();

                var articles = System.Text.Json.JsonSerializer.Deserialize<List<FulloutAllArticles>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (articles != null && articles.Any())
                {
                    _db.FulloutAllArticlesSet.AddRange(articles);
                    await _db.SaveChangesAsync();
                    TempData["Message"] = "Your JSON file was successfully processed.";
                }
                else
                {
                    TempData["Error"] = "The uploaded file does not contain valid article data.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToAction(nameof(AddFullNews));
        }
        #endregion

        #region Add Output Sentiments

        [Authorize(Roles = "Admin")]
        public IActionResult AddOutputSent()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOutputSent([FromForm] OutputSent entry)
        {
            try
            {
                _db.OutputSentSet.Add(entry);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Your manual entry has been successfully processed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(AddOutputSent));
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOutputSentFromFile(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Error"] = "Please upload a valid CSV file.";
                return RedirectToAction(nameof(AddOutputSent));
            }

            try
            {
                using var stream = new StreamReader(csvFile.OpenReadStream());
                var csvContent = await stream.ReadToEndAsync();

                var entries = new List<OutputSent>();
                var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines.Skip(1))
                {
                    var values = line.Split(',');
                    if (values.Length != 4)
                    {
                        TempData["Error"] = "CSV file format is incorrect.";
                        return RedirectToAction(nameof(AddOutputSent));
                    }

                    var entry = new OutputSent
                    {
                        Published_at = DateTime.Parse(values[0], CultureInfo.InvariantCulture),
                        TitleSentiment = double.Parse(values[1], CultureInfo.InvariantCulture),
                        ContentSentiment = double.Parse(values[2], CultureInfo.InvariantCulture),
                        DescriptionSentiment = double.Parse(values[3], CultureInfo.InvariantCulture)
                    };

                    entries.Add(entry);
                }

                if (entries.Any())
                {
                    _db.OutputSentSet.AddRange(entries);
                    await _db.SaveChangesAsync();
                    TempData["Message"] = "Your CSV file was successfully processed.";
                }
                else
                {
                    TempData["Error"] = "The uploaded file does not contain valid data.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToAction(nameof(AddOutputSent));
        }
        #endregion

        #region Add Average Sentiments

        [Authorize(Roles = "Admin")]
        public IActionResult AddAverageSent()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAverageSent([FromForm] AverageSent entry)
        {
            try
            {
                _db.AverageSentSet.Add(entry);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Your manual entry has been successfully processed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(AddAverageSent));
        }




        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAverageSentFromFile(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Error"] = "Please upload a valid CSV file.";
                return RedirectToAction(nameof(AddAverageSent));
            }

            try
            {
                using var stream = new StreamReader(csvFile.OpenReadStream());
                var csvContent = await stream.ReadToEndAsync();

                var entries = new List<AverageSent>();
                var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                // Skip header row and parse each line
                foreach (var line in lines.Skip(1))
                {
                    var values = line.Split(',');
                    if (values.Length != 4)
                    {
                        TempData["Error"] = "CSV file format is incorrect.";
                        return RedirectToAction(nameof(AddAverageSent));
                    }

                    var entry = new AverageSent
                    {
                        Published_at = DateTime.Parse(values[0], CultureInfo.InvariantCulture),
                        AvgTitleSentiment = double.Parse(values[1], CultureInfo.InvariantCulture),
                        AvgContentSentiment = double.Parse(values[2], CultureInfo.InvariantCulture),
                        AvgDescriptionSentiment = double.Parse(values[3], CultureInfo.InvariantCulture)
                    };

                    entries.Add(entry);
                }

                if (entries.Any())
                {
                    _db.AverageSentSet.AddRange(entries);
                    await _db.SaveChangesAsync();
                    TempData["Message"] = "Your CSV file was successfully processed.";
                }
                else
                {
                    TempData["Error"] = "The uploaded file does not contain valid data.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToAction(nameof(AddAverageSent));
        }

        #endregion


        #region Add entrys 
        [Authorize(Roles = "Admin")]
        public IActionResult AutoCollectNews()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AutoCollectNews([FromForm] RequestParameters request)
        {
            try
            {
                if ((request.endDate - request.startDate).TotalDays > 30)
                {
                    TempData["Error"] = "The date range exceeds one month. Please try with a range of less than a month.";
                    return RedirectToAction(nameof(AutoCollectNews));
                }
                await _businessLogic.AutoCollectNews(request);
                TempData["Message"] = "Your automatic data update has been successfully processed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }
            return RedirectToAction(nameof(AutoCollectNews));
        }


        #endregion
        #region update and predict
        [Authorize()]
        public IActionResult MachineLearning()
        {
            return View();
        }
        [Authorize()]
        public IActionResult MachineLearningView()
        {
            var latestResult = _db.MachineLearningSet
                      .OrderByDescending(x => x.MLResultID)
                      .FirstOrDefault();

            return View(latestResult);
        }
        [Authorize()]
        [HttpPost]
        public async Task<IActionResult> MachineLearning([FromForm]MachineLearningInputModel model)
        {
            try
            {
                await _businessLogic.MlSubprocess(
                    model.Symbol,
                    model.TitleSentiment,
                    model.ContSentiment,
                    model.DescSentiment,
                    model.OpenPrice,
                    model.CurrentHighPrice,
                    model.CurrentLowPrice,
                    model.CurrentVolume
                );


                TempData["Message"] = "Your automatic data update has been successfully processed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }
            return RedirectToAction(nameof(MachineLearningView));
        }

        #endregion
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}