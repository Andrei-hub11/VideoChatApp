using VideoChatApp.Api.DependencyInjection;
using VideoChatApp.Api.Middleware;
using VideoChatApp.Api.SignalR.Hubs;
using VideoChatApp.Application;
using VideoChatApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


{
    builder
        .Services.AddPresentation(builder.Configuration)
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

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseMiddleware<UnauthorizedResponseMiddleware>();
app.UseMiddleware<WebSocketsMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.MapHub<VideoChatHub>("/videoChatHub");

app.Run();

public partial class Program { }
