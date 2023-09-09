# CitizensOfficeAppointments
[![.NET](https://github.com/hakimdotdev/CitizensOfficeAppointments/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hakimdotdev/CitizensOfficeAppointments/actions/workflows/dotnet.yml)[![Docker](https://github.com/hakimdotdev/CitizensOfficeAppointments/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/hakimdotdev/CitizensOfficeAppointments/actions/workflows/docker-publish.yml)

.NET Application with Docker support to check the citizens office in Darmstadt for appointments.

## Notes 

- Supposed to be ran as Docker container
- Checks if there are appointments for the set concern
- Sends out appointments via E-Mail
- Should, in perspective reflect all concerns

## Docker

### Configuration

All configuration is provided by environment variables.

| Name                 | Description                                                                      | Example                              |
|----------------------|----------------------------------------------------------------------------------|--------------------------------------|
| CATEGORY			   | Category of the concern														  | Passwesen				             |
| CONCERN              | Concern of the appointment														  | Personalausweis						 |
| EMAILHOST            | Smtp url of the E-Mail host													  | smtp.strato.de		                 |
| EMAILUSER			   | E-Mail login user or address									                  | firstname@lastname.de                |
| EMAILUSER		   | E-Mail login password															  | mys3cur3p4ss						 |
| EMAIL				   | Recipient																		  | firstname@lastname.de                |

### Getting Started

```
 docker run -p 39999:39999 -e CATEGORY=x -e CONCERN=x -e EMAILHOST=x -e EMAILUSER=x -e EMAILPASSWORD=x -e EMAIL=x hakimdotnet/citizensofficeappointments:latest
