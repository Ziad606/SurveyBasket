using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SurveyBasket.Api;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddHybridCache();

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}




app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

//app.UseHangfireDashboard("/jobs", new DashboardOptions
//{
//    Authorization =
//    [
//        new HangfireCustomBasicAuthenticationFilter
//        {
//            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
//            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
//        }
//    ],
//    DashboardTitle = "Survey Basket Dashboard",
//    //IsReadOnlyFunc = (DashboardContext context) => true,
//});

//var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

//using var scope = scopeFactory.CreateScope();

//var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

//RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendPollsNotification(null), Cron.Daily);

app.UseCors("AllowAll");
//app.UseCors("MyPolicy");

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();
app.MapHealthChecks("health", new HealthCheckOptions{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
