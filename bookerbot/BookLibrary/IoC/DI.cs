using BookLibrary.Bot;
using BookLibrary.BusinessLayer.ShowBook;
using BookLibrary.DataLayer;
using BookLibrary.DataLayer.DbMapper;
using BookLibrary.DataLayer.Repositories.Book;
using BookLibrary.DataLayer.Repositories.City;
using BookLibrary.DataLayer.Repositories.Exchange;
using BookLibrary.DataLayer.Repositories.LikeBook;
using BookLibrary.DataLayer.Repositories.User;
using BookLibrary.DataLayer.Repositories.UserBook;
using BookLibrary.State;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibrary.IoC;

public static class DI
{
    public static void Start(IServiceCollection services)
    {
        services
            .AddHttpClient("books", client => { client.BaseAddress = new Uri("https://book24.ru"); });
        services.AddSingleton<BotStarter>();
        services.AddSingleton<BotMessageHelper>();
        services.AddSingleton<StateFactory>();
        services.AddSingleton<IDbMapper>(x => { return new NpgsqlDapperDbMapper(() => DbConfig.ConnectionString); });

        services.AddSingleton<ShowBookHelper>();

        services.AddTransient<AddBookState>();
        services.AddTransient<AddCityState>();
        services.AddTransient<ExchangeState>();
        services.AddTransient<ProfileState>();
        services.AddTransient<NotFoundExchangeState>();
        services.AddTransient<SuccessAddBookState>();
        services.AddTransient<MyBooksState>();


        services.AddSingleton<BookRepository>();
        services.AddSingleton<UserBookRepository>();
        services.AddSingleton<CityRepository>();
        services.AddSingleton<UserRepository>();
        services.AddSingleton<LikeBookRepository>();
        services.AddSingleton<ExchangeRepository>();
    }
}