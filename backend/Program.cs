using server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();
app.Run();
