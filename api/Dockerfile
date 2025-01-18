FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["src/api.csproj", "./"]
RUN dotnet restore "api.csproj"
COPY src/ ./
RUN dotnet build "api.csproj" -c Release -o /app/build

# Install playwright browsers during build
RUN dotnet tool install --global Microsoft.Playwright.CLI
ENV PATH="$PATH:/root/.dotnet/tools"
RUN playwright install chromium
RUN playwright install-deps

FROM build AS publish
RUN dotnet publish "api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install Playwright dependencies
RUN apt-get update && apt-get install -y \
    wget \
    curl \
    libgssapi-krb5-2 \
    libx11-6 \
    libx11-xcb1 \
    libxcb1 \
    libxcomposite1 \
    libxcursor1 \
    libxdamage1 \
    libxext6 \
    libxfixes3 \
    libxi6 \
    libxrandr2 \
    libxrender1 \
    libxss1 \
    libxtst6 \
    libglib2.0-0 \
    libnss3 \
    libcups2 \
    libdbus-1-3 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libdrm2 \
    libxkbcommon0 \
    libgtk-3-0 \
    libasound2 \
    libgbm1 \
    fonts-noto-color-emoji \
    fonts-liberation \
    && rm -rf /var/lib/apt/lists/*

# Copy the installed browsers from build stage
COPY --from=build /root/.cache/ms-playwright /root/.cache/ms-playwright

COPY --from=publish /app/publish .
EXPOSE 5250
ENV ASPNETCORE_URLS=http://+:5250
ENTRYPOINT ["dotnet", "api.dll"]