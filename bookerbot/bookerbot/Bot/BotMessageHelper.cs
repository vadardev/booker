using bookerbot.Context;
using bookerbot.State;

namespace bookerbot.Bot;

public class BotMessageHelper
{
    private readonly StateFactory _stateFactory;
    private Dictionary<Guid, UserContext> userContexts = new();
     
    public BotMessageHelper(StateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }
    
    public async Task TryAddUser(long telegramId)
    {
        //  UserEntity user = await GetUserByTelegramId(telegramId);
        var userContext = new UserContext(_stateFactory);
        userContext.State = EContextState.Exchange;

        userContexts.TryAdd(Guid.NewGuid(), userContext);
    }
}