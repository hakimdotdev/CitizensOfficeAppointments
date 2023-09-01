namespace CitizensOfficeAppointments
{
	using System;
	using System.Threading.Tasks;
	using CitizensOfficeAppointments.Services;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;

	public static class Program
	{
		public static async Task Main(string[] args)
		{
			await CreateHostBuilder(args).RunConsoleAsync();
		}

		private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseConsoleLifetime()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<AppointmentService>();
				});
	}
}