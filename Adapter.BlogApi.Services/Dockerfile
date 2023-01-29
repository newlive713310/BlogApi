FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore "Adapter.BlogApi.Services/Adapter.BlogApi.Services.csproj"

COPY . .
WORKDIR "/src/Adapter.BlogApi.Services"
RUN dotnet build "Adapter.BlogApi.Services.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Adapter.BlogApi.Services.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS http://*:10540
EXPOSE 10540

ENTRYPOINT ["dotnet", "Adapter.BlogApi.Services.dll"]