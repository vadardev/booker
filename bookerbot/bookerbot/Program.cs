using BookLibrary.Bot;
using BookLibrary.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        DI.Start(services);
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
        Console.WriteLine(ex.Message);
    }
}