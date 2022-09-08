using bookerbot.Bot;
using bookerbot.DataLayer.DbMapper;
using bookerbot.DataLayer.Repositories.Book;
using bookerbot.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using telegrambotconsole.DataLayer.Repositories.City;
using telegrambotconsole.DataLayer.Repositories.LikeBook;
using telegrambotconsole.DataLayer.Repositories.User;
using telegrambotconsole.DataLayer.Repositories.UserBook;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddHttpClient("books", client => { client.BaseAddress = new Uri("https://book24.ru"); });
        services.AddSingleton<BotStarter>();
        services.AddSingleton<BotMessageHelper>();
        services.AddSingleton<StateFactory>();
        services.AddSingleton<IDbMapper, NpgsqlDapperDbMapper>();

        services.AddTransient<AddBookState>();
        services.AddTransient<AddCityState>();
        services.AddTransient<ExchangeState>();
        services.AddTransient<ProfileState>();
        
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