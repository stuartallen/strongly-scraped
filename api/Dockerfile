FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY ["src/api.csproj", "./"]
RUN dotnet restore "api.csproj"
COPY src/ ./
RUN dotnet build "api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5250
ENV ASPNETCORE_URLS=http://+:5250
ENTRYPOINT ["dotnet", "api.dll"] 