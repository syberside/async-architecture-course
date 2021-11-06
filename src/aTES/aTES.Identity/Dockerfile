#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["aTES.Identity/aTES.Identity.csproj", "aTES.Identity/"]
RUN dotnet restore "aTES.Identity/aTES.Identity.csproj"
COPY . .
WORKDIR "/src/aTES.Identity"
RUN dotnet build "aTES.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "aTES.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "aTES.Identity.dll"]