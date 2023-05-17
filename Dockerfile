FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
COPY . ./app
WORKDIR /app

RUN dotnet publish "./Mockus.WebApi/Mockus.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app

COPY --from=build /app/publish .
ENV DOTNET_USE_POLLING_FILE_WATCHER=true  
ENV ASPNETCORE_URLS=http://+:80 
EXPOSE 80
ENTRYPOINT ["dotnet", "Mockus.WebApi.dll"]