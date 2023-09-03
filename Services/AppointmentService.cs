using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _64CitizenOfficeApp.NET.Helpers;
using CitizensOfficeAppointments.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CitizensOfficeAppointments.Services
{
	public class AppointmentService : BackgroundService
	{
		private readonly ILogger<AppointmentService> _logger;
		private string category;
		private string concern;
		private bool appointments = false;

		public AppointmentService(ILogger<AppointmentService> logger)
		{
			_logger = logger;
			category = StringExtensions.FirstCharToUpper(Environment.GetEnvironmentVariable("CATEGORY"));
			concern = StringExtensions.FirstCharToUpper(Environment.GetEnvironmentVariable("CONCERN"));
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("[{dt}] Starting to fetch appointments for {cat} => {con}", DateTime.UtcNow.ToLongTimeString(), category, concern);

			Timer _timer = new(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
		}

		private async void DoWork(object? state)
		{
			if (!String.IsNullOrWhiteSpace(category))
			{
				if (!String.IsNullOrWhiteSpace(concern))
				{
					ChromeOptions chromeOptions = new ChromeOptions();
					chromeOptions.AddArgument("port=39999");
					IWebDriver driver = new ChromeDriver(chromeOptions);

					_logger.LogInformation("[{dt}] Fetching appointments" , DateTime.UtcNow.ToLongTimeString());

					driver.Navigate().GoToUrl("https://tevis.ekom21.de/stdar/select2?md=4");
					// Kategorie-Akkordion
					driver.FindElement(By.XPath($"//*[contains(text(), '{category}')]")).Click();
					//TODO: below more dynamic for all use cases
					// Anzahl des zu buchenden Anliegen innerhalb der Kategorie erhöhen
					driver.FindElement(By.CssSelector($"[title='Erhöhen der Anzahl des Anliegens Antrag {concern}']")).Click();
					// Weiter
					driver.FindElements(By.Id("WeiterButton")).FirstOrDefault()!.Click();
					// Ok
					driver.FindElements(By.Id("OKButton")).FirstOrDefault()!.Click();
					// Standort auswählen
					driver.FindElement(By.Name("select_location")).Click();
					// Body auslesen
					IWebElement body = driver.FindElement(By.TagName("body"));
					appointments = !body.Text.Contains("Kein freier Termin verfügbar");
					if (appointments)
					{
						_logger.LogInformation("[{dt}] Starting to fetch appointments for {cat} => {con}", DateTime.UtcNow.ToLongTimeString(), category, concern);
						// scrape termine
						List<DateTime> dates = new List<DateTime>()
						{
							DateTime.UtcNow //...
						};
						Smtp.SendMail(dates.ToString());
						return;
					}
					_logger.LogInformation("[{dt}] There is currently no appointments stated on the website", DateTime.UtcNow.ToLongTimeString());
					return;
				}
				_logger.LogInformation("[{dt}] Concern wasn't provided properly provided.", DateTime.UtcNow.ToLongTimeString());
				return;
			}
			_logger.LogInformation("[{dt}] Category wasn't provided properly.", DateTime.UtcNow.ToLongTimeString());
			return;

		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation(
				"Consume Scoped Service Hosted Service is stopping.");

			await base.StopAsync(stoppingToken);
		}
	}
}
