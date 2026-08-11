using server.Extensions;
using server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentityHandlersAndStores();
builder.Services.AddIdentityAuth(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddApiDoc();
builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureCorsPolicy();
app.ConfigureApiDoc();
app.UseExceptionHandler();
app.ConfigureIdentityAuth();
app.MapControllers();

app.Run();
