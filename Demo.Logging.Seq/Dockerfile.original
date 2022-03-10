#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Demo.Logging.Seq/Demo.Logging.Seq.csproj", "Demo.Logging.Seq/"]
RUN dotnet restore "Demo.Logging.Seq/Demo.Logging.Seq.csproj"
COPY . .
WORKDIR "/src/Demo.Logging.Seq"
RUN dotnet build "Demo.Logging.Seq.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Logging.Seq.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Logging.Seq.dll"]