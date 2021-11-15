#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["aTES.Billing/aTES.Billing.csproj", "aTES.Billing/"]
RUN dotnet restore "aTES.Billing/aTES.Billing.csproj"
COPY . .
WORKDIR "/src/aTES.Billing"
RUN dotnet build "aTES.Billing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "aTES.Billing.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "aTES.Billing.dll"]