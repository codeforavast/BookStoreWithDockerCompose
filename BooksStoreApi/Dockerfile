#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
#ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BooksStoreApi/BooksStore.Api.csproj", "BooksStoreApi/"]
COPY ["BookStore.Data/BookStore.Data.csproj", "BookStore.Data/"]
COPY ["BookStore.Service/BookStore.Service.csproj", "BookStore.Service/"]
RUN dotnet restore "BooksStoreApi/BooksStore.Api.csproj"
COPY . .
WORKDIR "/src/BooksStoreApi"
RUN dotnet build "BooksStore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksStore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksStore.Api.dll"]