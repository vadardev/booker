using bookerbot.Bot;
using bookerbot.BusinessLayer;
using bookerbot.BusinessLayer.ShowBook;
using bookerbot.DataLayer;
using bookerbot.DataLayer.DbMapper;
using bookerbot.DataLayer.Repositories.Book;
using bookerbot.DataLayer.Repositories.City;
using bookerbot.DataLayer.Repositories.LikeBook;
using bookerbot.DataLayer.Repositories.User;
using bookerbot.DataLayer.Repositories.UserBook;
using bookerbot.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using telegrambotconsole.DataLayer.Repositories.User;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddHttpClient("books", client => { client.BaseAddress = new Uri("https://book24.ru"); });
        services.AddSingleton<BotStarter>();
        services.AddSingleton<BotMessageHelper>();
        services.AddSingleton<StateFactory>();
        services.AddSingleton<IDbMapper>(x =>
        {
            return new NpgsqlDapperDbMapper(() => DbConfig.ConnectionString);
        });

        services.AddSingleton<ShowBookHelper>();
        
        services.AddTransient<AddBookState>();
        services.AddTransient<AddCityState>();
        services.AddTransient<ExchangeState>();
        services.AddTransient<ProfileState>();
        services.AddTransient<NotFoundExchangeState>();
        
        services.AddSingleton<BookRepository>();
        services.AddSingleton<UserBookRepository>();
        services.AddSingleton<CityRepository>();
        services.AddSingleton<UserRepository>();
        services.AddSingleton<LikeBookRepository>();
        
        
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