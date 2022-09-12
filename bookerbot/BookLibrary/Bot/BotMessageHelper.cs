using BookLibrary.Context;
using BookLibrary.DataLayer.Repositories.User;
using BookLibrary.State;
using telegrambotconsole.DataLayer.Repositories.User;

namespace BookLibrary.Bot;

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
    
    public async Task TryAddUser(TelegramUser telegramUser)
    { 
        UserEntity user = await GetUser(telegramUser);
        var userContext = new UserContext(_stateFactory);
        userContext.State = EContextState.Exchange;
        userContext.UserId = user.Id;
        
        userContexts.TryAdd(user.Id, userContext);
    }
    
    public async Task<ResponseMessage> NextState(TelegramUser telegramUser, string message)
    {
        UserContext userContext = await GetUserContext(telegramUser);

        return await userContext.Change(message);
    }

    public async Task<ResponseMessage> ForceConnectUserState(Guid userId, string message)
    {
        if (userContexts.TryGetValue(userId, out var userContext))
        {
            userContexts.Remove(userId);
        }
        
        userContext = new UserContext(_stateFactory);
        userContext.State = EContextState.UserConnect;
        userContext.UserId = userId;

        userContexts.TryAdd(userId, userContext);

        return UserConnectState.GetResponseMessage(userContext, message);
    }

    private async Task<UserContext> GetUserContext(TelegramUser telegramUser)
    {
        UserEntity user = await GetUser(telegramUser);

        if (!userContexts.TryGetValue(user.Id, out var userContext))
        {
            userContext = new UserContext(_stateFactory);
            userContext.State = EContextState.Exchange;
            userContext.UserId = user.Id;

            userContexts.TryAdd(user.Id, userContext);
        }

        return userContext;
    }

    private async Task<UserEntity> GetUser(TelegramUser telegramUser)
    {
        UserEntity? user = await _userRepository.GetByTelegramId(telegramUser.Id);

        if (user == null)
        {
            user = new UserEntity
            {
                Id = Guid.NewGuid(),
                TelegramId = telegramUser.Id,
                UserName = telegramUser.UserName,
                CityId = null,
                ChatId = telegramUser.ChatId,
            };

            await _userRepository.TryAdd(user);
        }

        return user;
    }
    
}