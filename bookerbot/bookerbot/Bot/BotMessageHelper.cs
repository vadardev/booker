using bookerbot.Context;
using bookerbot.State;
using telegrambotconsole.DataLayer.Repositories.User;

namespace bookerbot.Bot;

public class BotMessageHelper
{
    private readonly StateFactory _stateFactory;
    private readonly UserRepository _userRepository;
    private Dictionary<Guid, UserContext> userContexts = new();
     
    public BotMessageHelper(StateFactory stateFactory, UserRepository userRepository)
    {
        _stateFactory = stateFactory;
        _userRepository = userRepository;
    }
    
    public async Task TryAddUser(long telegramId)
    { 
        UserEntity user = await GetUserByTelegramId(telegramId);
        var userContext = new UserContext(_stateFactory);
        userContext.State = EContextState.Exchange;
        userContext.UserId = user.Id;
        
        userContexts.TryAdd(user.Id, userContext);
    }
    
    public async Task<ResponseMessage> NextState(long telegramId, string message)
    {
        UserContext userContext = await GetUserContext(telegramId);

        return await userContext.Change(message);
    }

    private async Task<UserContext> GetUserContext(long telegramId)
    {
        UserEntity user = await GetUserByTelegramId(telegramId);

        if (!userContexts.TryGetValue(user.Id, out var userContext))
        {
            userContext = new UserContext(_stateFactory);
            userContext.State = EContextState.Exchange;
            userContext.UserId = user.Id;
        }

        return userContext;
    }

    private async Task<UserEntity> GetUserByTelegramId(long telegramId)
    {
        UserEntity? user = await _userRepository.GetByTelegramId(telegramId);

        if (user == null)
        {
            user = new UserEntity
            {
                Id = Guid.NewGuid(),
                TelegramId = telegramId,
                CityId = null,
            };

            await _userRepository.TryAdd(user);
        }

        return user;
    }

}