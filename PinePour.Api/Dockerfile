FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PinePour.Api.csproj ./
RUN dotnet restore PinePour.Api.csproj -p:SkipSpaProjectReference=true

COPY . .
RUN dotnet publish PinePour.Api.csproj -c Release -o /app/publish -p:SkipSpaProjectReference=true --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "PinePour.Api.dll"]
