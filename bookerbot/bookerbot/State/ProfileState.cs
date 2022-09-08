using bookerbot.Context;

namespace bookerbot.State;

public class ProfileState : IUserState
{
    public Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        throw new NotImplementedException();
    }
}