using BookLibrary.Bot;
using BookLibrary.BusinessLayer.Exchange;
using BookLibrary.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;



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
        ITelegramBotClient bot = new TelegramBotClient("5186218806:AAEe2niWFOV3m9_U1pI95n7OrpjlB0z8gMY");

        var exchangeHelper = services.GetRequiredService<ExchangeHelper>();

        while (true)
        {
            await exchangeHelper.Start(bot);

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

