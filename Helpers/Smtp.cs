using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CitizensOfficeAppointments.Helpers
{
	public class Smtp
	{
		private static string email;
		private static string emailHost;
		private static string emailUser;
		private static string emailPassword;
		private static ILogger _logger;
		public Smtp(ILogger<Smtp> logger)
		{
			_logger = logger;
			emailHost = Environment.GetEnvironmentVariable("EMAILHOST");
			emailUser = Environment.GetEnvironmentVariable("EMAILUSER");
			emailPassword = Environment.GetEnvironmentVariable("EMAILPASSWORD");
			email = Environment.GetEnvironmentVariable("EMAIL");

		}
		public static async Task<bool> SendMail(string body)
		{

			var smtpClient = new SmtpClient(emailHost)
			{
				Port = 587,
				Credentials = new NetworkCredential(emailUser, emailPassword),
				EnableSsl = true
			};

			var message = new MailMessage(email, email)
			{
				Body = body,
				Subject = "Bürgeramt Darmstadt: Termine Vorhanden",
			};

			try
			{
				smtpClient.Send(message);
				_logger.LogInformation("[{dt}] Successfully sent Mail upon recieving available appointments", DateTime.Now.ToLongDateString);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError("[{dt}] Couldn't send Mail upon recieving available appointments. {ex}", DateTime.Now.ToLongDateString, ex);
				return false;
			}


		}
	}
}
