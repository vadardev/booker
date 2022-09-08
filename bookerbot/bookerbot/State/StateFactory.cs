using bookerbot.Context;
using Microsoft.Extensions.DependencyInjection;

namespace bookerbot.State;

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
        }

        return _serviceProvider.GetRequiredService<ExchangeState>();
    }
}