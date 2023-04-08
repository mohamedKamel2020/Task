using System.Diagnostics;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using GoogleCalendar.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoogleCalendar.Controllers
{
    public class HomeController : Controller
    {
        private readonly string[] _scopes = { CalendarService.Scope.Calendar };
        private readonly string _applicationName = "Calendar API Integration";
        private readonly ILogger<HomeController> _logger;
        private CalendarService _calendarService;
        private string calenderId
        {
            get
            {
                return HttpContext.Session.GetString("calenderId");
            }
            set
            {
                HttpContext.Session.SetString("calenderId", value);
            }
        }

        public HomeController(ILogger<HomeController> logger)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            //var applicationConfigurations = configuration.GetSection("Google-Auth").Value;

            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [GoogleScopedAuthorize(CalendarService.ScopeConstants.Calendar)]
        public async Task<IActionResult> Login([FromServices] IGoogleAuthProvider auth)
        {
            if (_calendarService == null)
            {
                var userCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = "893787613838-iak58jo6r1o51uerkc9r16cqk9tdbq6k.apps.googleusercontent.com",
                        ClientSecret = "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",
                    },
                    _scopes,
                    "user",
                    CancellationToken.None);
                _calendarService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = userCredential,
                    ApplicationName = _applicationName
                });
            }
       
                var googleCredential = await auth.GetCredentialAsync();
                _calendarService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = googleCredential,
                    ApplicationName = _applicationName,
                });
   
                

            var calendarList = await _calendarService.CalendarList.List().ExecuteAsync();
            calenderId = calendarList.Items[0].Id;
            var CalendarEvents = await _calendarService.Events.List(calenderId).ExecuteAsync();
            return View(CalendarEvents);
        }
        //-----insert event------------------
        public IActionResult InsertEvent()
        {
            return View("InsertEvent");
        }
        [HttpPost]
        public async Task<IActionResult> InsertEvent(Models.Event newEvent)
        {

            var calendarEvent = new Google.Apis.Calendar.v3.Data.Event()
            {
                Summary = newEvent.EventTitle,
                Description = newEvent.Description,
                Start = new EventDateTime { DateTime = newEvent.Start },
                End = { DateTime = newEvent.End }
            };
            String calendarId = "primary";
            if (newEvent.End <= newEvent.Start)
            {
                Console.WriteLine("-----");
            }
            else
            {
                EventsResource.InsertRequest request = _calendarService.Events.Insert(calendarEvent, calendarId);
                Google.Apis.Calendar.v3.Data.Event createdEvent = await request.ExecuteAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromServices] IGoogleAuthProvider auth)
        {
            try
            {

                if (_calendarService == null)
                {
                    var googleCredential = await auth.GetCredentialAsync();
                    _calendarService = new CalendarService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = googleCredential,
                        ApplicationName = _applicationName,
                    });
                }
                string eventId = Request.Path.Value.Split('/').LastOrDefault();
                if (_calendarService != null)
                {
                    var deleteResult = await _calendarService.Events.Delete(calenderId, eventId).ExecuteAsync();
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}