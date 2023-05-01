using AuthorBook.API;
using AuthorBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuthorBookContext>
    (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AuthorBookDB")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/authors", async (AuthorBookContext context) =>
    await context.Authors.Select(a => new { Name = a.FullName(), a.Id }).ToListAsync());

app.MapGet("/authors/firstandlastname/{first}/{last}", async (string first, string last, AuthorBookContext context) =>
    await context.Authors
    .Where(a => a.Name.FirstName.ToLower() == first.ToLower() &&
    a.Name.LastName.ToLower() == last.ToLower())
    .Select(a => new { Name = a.FullName(), a.Id })
    .ToListAsync());

app.MapGet("/authors/firstname/{first}", async (string first, AuthorBookContext context) =>
    await context.Authors
    .Where(a =>
    a.Name.FirstName.ToLower().StartsWith(first.ToLower()))
    .Select(a => new { Name = a.FullName(), a.Id })
    .ToListAsync());
app.MapGet("/authors/lastname/{last}", async (string last, AuthorBookContext context) =>
    await context.Authors
    .Where(a =>
    a.Name.LastName.ToLower().StartsWith(last.ToLower()))
    .Select(a => new { Name = a.FullName(), a.Id })
    .ToListAsync());
app.MapPost("/authors/seed", async (AuthorBookContext context) =>
 {
     await Seeder.SeedTheData(context);
 });

app.Run();



