using bookerbot.Context;
using bookerbot.DataLayer.Repositories.User;
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
    
    public async Task TryAddUser(long telegramId, string? userName)
    { 
        UserEntity user = await GetUserByTelegramId(telegramId, userName);
        var userContext = new UserContext(_stateFactory);
        userContext.State = EContextState.Exchange;
        userContext.UserId = user.Id;
        
        userContexts.TryAdd(user.Id, userContext);
    }
    
    public async Task<ResponseMessage> NextState(long telegramId, string? userName, string message)
    {
        UserContext userContext = await GetUserContext(telegramId, userName);

        return await userContext.Change(message);
    }

    private async Task<UserContext> GetUserContext(long telegramId, string? userName)
    {
        UserEntity user = await GetUserByTelegramId(telegramId, userName);

        if (!userContexts.TryGetValue(user.Id, out var userContext))
        {
            userContext = new UserContext(_stateFactory);
            userContext.State = EContextState.Exchange;
            userContext.UserId = user.Id;

            userContexts.TryAdd(user.Id, userContext);
        }

        return userContext;
    }

    private async Task<UserEntity> GetUserByTelegramId(long telegramId, string? userName)
    {
        UserEntity? user = await _userRepository.GetByTelegramId(telegramId);

        if (user == null)
        {
            user = new UserEntity
            {
                Id = Guid.NewGuid(),
                TelegramId = telegramId,
                UserName = userName,
                CityId = null,
            };

            await _userRepository.TryAdd(user);
        }

        return user;
    }

}