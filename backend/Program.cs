using server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentityHandlersAndStores();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
