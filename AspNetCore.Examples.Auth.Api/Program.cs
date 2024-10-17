using AspNetCore.Examples.Auth.Api.Authorization;
using AspNetCore.Examples.Auth.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient()
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomAuthorization()
    .AddCustomOpenApi(builder.Configuration);
    

var app = builder.Build();

app.UseCustomOpenApi(builder.Configuration);

app.UseAuthentication();
app.UseCustomAuthorization();

app.MapAuthEndpoints();

app.Run();

public partial class Program { }