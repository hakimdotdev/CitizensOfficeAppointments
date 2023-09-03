using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using _64CitizenOfficeApp.NET.Helpers;
using CitizensOfficeAppointments.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

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

			Timer _timer = new(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
		}

		private async void DoWork(object? state)
		{
			if (!String.IsNullOrWhiteSpace(category))
			{
				if (!String.IsNullOrWhiteSpace(concern))
				{
					ChromeOptions chromeOptions = new ChromeOptions();
					chromeOptions.AddArgument("port=39999");
					chromeOptions.AddArgument("--start-maximized");
					IWebDriver driver = new ChromeDriver(chromeOptions);
					Actions actions = new(driver);

					_logger.LogInformation("[{dt}] Fetching appointments" , DateTime.Now.ToLongTimeString());

					driver.Navigate().GoToUrl("https://tevis.ekom21.de/stdar/select2?md=4");
					Thread.Sleep(1000);
					// Kategorie-Akkordion
					driver.FindElement(By.Id("cookie_msg_btn_yes")).Click();
					var accordion = driver.FindElement(By.XPath($"//*[contains(text(), '{category}')]"));
					Thread.Sleep(1000);
					actions.MoveToElement(accordion).Click().Perform();
					Thread.Sleep(1000);

					//TODO: below more dynamic for all use cases
					// Anzahl des zu buchenden Anliegen innerhalb der Kategorie erhöhen
					var plusButton = driver.FindElement(By.CssSelector($"[title='Erhöhen der Anzahl des Anliegens Antrag {concern}']"));
					actions.MoveToElement(plusButton).Click().Perform();
					Thread.Sleep(1000);

					// Weiter
					var weiterButton = driver.FindElements(By.Id("WeiterButton")).FirstOrDefault()!;
					actions.MoveToElement(weiterButton).Click().Perform();
					Thread.Sleep(1000);

					// Ok
					var okButton = driver.FindElements(By.Id("OKButton")).FirstOrDefault()!;
					actions.MoveToElement(okButton).Click().Perform();
					Thread.Sleep(1000);

					// Standort auswählen
					var locationButton = driver.FindElement(By.Name("select_location"));
					actions.MoveToElement(locationButton).Click().Perform();


					// Body auslesen
					IWebElement body = driver.FindElement(By.TagName("body"));
					if (!body.Text.Contains("Kein freier Termin verfügbar"))
					{
						_logger.LogInformation("[{dt}] Found appointments for {cat} => {con}.", DateTime.Now.ToLongTimeString(), category, concern);
						Smtp.SendMail(body.Text);
						return;
					}
					_logger.LogInformation("[{dt}] There is currently no appointments stated on the website", DateTime.Now.ToLongTimeString());
					driver.Quit();
					return;
				}
				_logger.LogInformation("[{dt}] Concern wasn't provided properly provided.", DateTime.Now.ToLongTimeString());
				return;
			}
			_logger.LogInformation("[{dt}] Category wasn't provided properly.", DateTime.Now.ToLongTimeString());
			return;

		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation(
				"[{dt}] AppointmentService stopping.", DateTime.Now.ToLongDateString);

			await base.StopAsync(stoppingToken);
		}
	}
}
