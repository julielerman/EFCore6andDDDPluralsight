using Infrastructure.Data;
using Infrastructure.Data.Services;
using Microsoft.EntityFrameworkCore;
using PublisherMiniFrontEnd.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["baseUrls:apiBase"]) });
builder.Services.AddScoped<ContractedAuthorsService>();
builder.Services.AddScoped<ContractSearchService>();
builder.Services.AddDbContext<SearchContext>
    (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PubDB")));
var app = builder.Build();

app.MapGet("/authors", async (ContractedAuthorsService authorService) =>
    await authorService.ListAllAuthorsAsync());

app.MapGet("/authors/firstname/{first}",async (string first, ContractedAuthorsService authorService) =>
    await authorService.GetAuthorsByFirstNameAsync(first));

app.MapGet("/authors/lastname/{last}", async (string last, ContractedAuthorsService authorService) =>
    await authorService.GetAuthorsByLastNameAsync(last));

app.MapGet("contracts/ByAuthorLastName{last}",
    async(string last, ContractSearchService searcher)=>
    await searcher.GetContractPickListForAuthorLastName(last));

app.MapGet("contracts/ByInitDateRange{start,end}",
    async (string start,string end, ContractSearchService searcher) =>
    await searcher.GetContractPickListForInitiatedDateRange
      (DateTime.Parse(start), DateTime.Parse(end)));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Run();

