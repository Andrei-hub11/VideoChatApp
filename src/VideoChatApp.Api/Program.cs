using VideoChatApp.Api.DependencyInjection;
using VideoChatApp.Api.Middleware;
using VideoChatApp.Application;
using VideoChatApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseMiddleware<UnauthorizedResponseMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.Run();
