using AspNetCore.Examples.Auth.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers().Services
    .AddEndpointsApiExplorer()
    .AddCustomSwagger(builder.Configuration)
    .AddCustomAuthenticacion(builder.Configuration)
    .AddCustomAuthorization();

var app = builder.Build();

app.UseCustomSwagger(app.Configuration);

app.UseAuthentication();
app.UseCustomAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }