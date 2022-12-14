using BookLibrary.Context;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibrary.State;

public class StateFactory
{
    private readonly IServiceProvider _serviceProvider;

    public StateFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IUserState GetState(EContextState state)
    {
        switch (state)
        {
            case EContextState.AddBook:
                return _serviceProvider.GetRequiredService<AddBookState>();
            case EContextState.AddCity:
                return _serviceProvider.GetRequiredService<AddCityState>();
            case EContextState.Exchange:
                return _serviceProvider.GetRequiredService<ExchangeState>();
            case EContextState.Profile:
                return _serviceProvider.GetRequiredService<ProfileState>();
            case EContextState.NotFoundExchange:
                return _serviceProvider.GetRequiredService<NotFoundExchangeState>();
            case EContextState.SuccessAddBook:
                return _serviceProvider.GetRequiredService<SuccessAddBookState>();
            case EContextState.MyBooks:
                return _serviceProvider.GetRequiredService<MyBooksState>();
            case EContextState.UserConnect:
                return _serviceProvider.GetRequiredService<UserConnectState>();
        }

        return _serviceProvider.GetRequiredService<ExchangeState>();
    }
}