using AspNetCore.Examples.Auth.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient()
    .AddCustomAuthenticacion(builder.Configuration)
    .AddCustomAuthorization()
    .AddCustomOpenApi(builder.Configuration)
    .AddControllers();
    

var app = builder.Build();

app.UseCustomOpenApi(builder.Configuration);

app.UseAuthentication();
app.UseCustomAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }