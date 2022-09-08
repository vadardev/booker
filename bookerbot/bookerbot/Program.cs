using bookerbot.Bot;
using bookerbot.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient();
        services.AddSingleton<BotStarter>();
        services.AddSingleton<BotMessageHelper>();
        services.AddSingleton<StateFactory>();

        services.AddTransient<AddBookState>();
        services.AddTransient<AddCityState>();
        services.AddTransient<ExchangeState>();
        services.AddTransient<ProfileState>();
    }).UseConsoleLifetime();
    
var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
{
    var services = serviceScope.ServiceProvider;

    try
    {
        var botStarter = services.GetRequiredService<BotStarter>();
        await botStarter.Run();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error Occured");
    }
}