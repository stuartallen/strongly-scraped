using Api.Configuration;
using Api.CronJobs;
using Api.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ScrapeSettings>(
builder.Configuration.GetSection(nameof(ScrapeSettings)));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<Daily>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ScrapeSettings>();
builder.Services.AddScoped<Scrape>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/scrape", async (Scrape scrape) =>
{
    await scrape.ScrapeThumbnailEventRowAsync().ConfigureAwait(false);
    return "Scraping completed. Check the logs for results.";
});

app.Run();
