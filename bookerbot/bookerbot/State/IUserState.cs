using bookerbot.Context;

namespace bookerbot.State;

public interface IUserState
{
    Task<ResponseMessage> Handle(UserContext userContext, string message);
}