using System.Diagnostics.CodeAnalysis;
using Api.Configuration;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace Api.Utils;

/// <summary>
/// Utility class for web scraping operations.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated by dependency injection.")]
internal sealed class Scrape
{
    private static readonly Action<ILogger, string, Exception?> LogScrapingError =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(3, nameof(LogScrapingError)), "Error scraping headings from {Url}");

    private static readonly Action<ILogger, string, Exception?> LogElementNotFound =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(4, nameof(LogElementNotFound)), "Element not found: {Element}");

    private static readonly Action<ILogger, string, Exception?> LogElementFound =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(5, nameof(LogElementFound)), "Element found: {Element}");

    private readonly ILogger<Scrape> logger;
    private readonly ScrapeSettings settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scrape"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="settings">The settings instance.</param>
    public Scrape(ILogger<Scrape> logger, IOptions<ScrapeSettings> settings)
    {
        this.logger = logger;
        this.settings = settings.Value;
    }

    /// <summary>
    /// Scrapes the first thumbnail event row from the specified URL.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ScrapeThumbnailEventRowAsync()
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync(settings.EventsUrl);

            await page.WaitForSelectorAsync(".card");
            await page.WaitForSelectorAsync(".card-title");

            var cards = await page.QuerySelectorAllAsync(".card");
            // foreach (var card in cards)
            // {
            //     await ExtractCardContentAsync(card);
            // }
            await ExtractCardContentAsync(cards[0]);
        }
        catch (Exception ex)
        {
            LogScrapingError(this.logger, settings.EventsUrl, ex);
            throw;
        }
    }

    private async Task ExtractCardContentAsync(IElementHandle card)
    {
        var body = await card.QuerySelectorAsync(".card-body");
        if (body == null)
        {
            LogElementNotFound(this.logger, ".card-body", null);
            return;
        }

        var titleElement = await body.QuerySelectorAsync(".card-title");
        if (titleElement == null)
        {
            LogElementNotFound(this.logger, ".card-title", null);
            return;
        }

        var dateContainer = await body.QuerySelectorAsync("#date");
        if (dateContainer == null)
        {
            LogElementNotFound(this.logger, "#date", null);
            return;
        }

        var dateContainerSpans = await dateContainer.QuerySelectorAllAsync("span");
        if (dateContainerSpans.Count < 2)
        {
            LogElementNotFound(this.logger, "#date", null);
            return;
        }

        var monthElement = dateContainerSpans[0];
        var dayElement = dateContainerSpans[1];

        var pTags = await body.QuerySelectorAllAsync("p");
        if (pTags.Count < 2)
        {
            LogElementNotFound(this.logger, "city/state or meet director", null);
            return;
        }

        var cityState = pTags[0];
        var meetDirectorContainer = pTags[1];
        var meetDirectorContainerSpans = await meetDirectorContainer.QuerySelectorAllAsync("span");
        var meetDirectorElement = meetDirectorContainerSpans[1];

        if (meetDirectorElement == null)
        {
            LogElementNotFound(this.logger, "meet director", null);
            return;
        }

        var titleText = await titleElement.TextContentAsync();
        var monthText = await monthElement.TextContentAsync();
        var dayText = await dayElement.TextContentAsync();
        var cityStateText = await cityState.TextContentAsync();
        var meetDirectorText = await meetDirectorElement.TextContentAsync();

        Console.WriteLine($"Title: {titleText}");
        Console.WriteLine($"Date: {monthText} {dayText}");
        Console.WriteLine($"City State: {cityStateText}");
        Console.WriteLine($"Meet Director: {meetDirectorText}");
    }
}
