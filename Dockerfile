#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CitizensOfficeAppointments.csproj", "."]
RUN dotnet restore "./CitizensOfficeAppointments.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CitizensOfficeAppointments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CitizensOfficeAppointments.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CitizensOfficeAppointments.dll"]