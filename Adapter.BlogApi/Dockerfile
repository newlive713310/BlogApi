#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Adapter.BlogApi/Adapter.BlogApi.csproj", "Adapter.BlogApi/"]
RUN dotnet restore "Adapter.BlogApi/Adapter.BlogApi.csproj"
COPY . .
WORKDIR "/src/Adapter.BlogApi"
RUN dotnet build "Adapter.BlogApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Adapter.BlogApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Adapter.BlogApi.dll"]