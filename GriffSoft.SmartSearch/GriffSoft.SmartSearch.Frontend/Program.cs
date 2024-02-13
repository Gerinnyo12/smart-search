using GriffSoft.SmartSearch.ChangeTracker.Options;
using GriffSoft.SmartSearch.ChangeTracker.Services;
using GriffSoft.SmartSearch.Frontend.Extensions;
using GriffSoft.SmartSearch.Frontend.Providers;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Options;
using GriffSoft.SmartSearch.Logic.Providers;
using GriffSoft.SmartSearch.Logic.Services.Searching;
using GriffSoft.SmartSearch.Logic.Services.Synchronization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.RegisterValidatableOption<ElasticsearchClientOptions>(nameof(ElasticsearchClientOptions));
builder.RegisterValidatableOption<ElasticsearchData>(nameof(ElasticsearchData));
builder.RegisterValidatableOption<CronSchedulerOptions>(nameof(CronSchedulerOptions));

builder.Services.AddSingleton<ElasticsearchClientProvider>();
builder.Services.AddSingleton<ISearchService<ElasticDocument>, ElasticSearchService>();
builder.Services.AddTransient<SearchServiceProvider>();

builder.Services.AddSingleton<ElasticBulkOperationService>();
builder.Services.AddSingleton<ISynchronizerService, ElasticReIndexService>();
builder.Services.AddHostedService<CronSchedulerService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
