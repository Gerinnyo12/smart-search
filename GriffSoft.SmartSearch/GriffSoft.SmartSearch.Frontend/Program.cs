using GriffSoft.SmartSearch.Frontend.Extensions;
using GriffSoft.SmartSearch.Frontend.Providers;
using GriffSoft.SmartSearch.Logic.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.RegisterOption<IndexSettings>(nameof(IndexSettings));
builder.RegisterOption<ElasticClientSettings>(nameof(ElasticClientSettings));
builder.RegisterOption<ElasticsearchData>(nameof(ElasticsearchData));

builder.Services.AddSingleton<SearchServiceProvider>();

var app = builder.Build();
await app.InitializeAsync();
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
