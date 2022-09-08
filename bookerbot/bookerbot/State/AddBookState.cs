using bookerbot.Context;

namespace bookerbot.State;

public class AddBookState : IUserState
{
    public Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        throw new NotImplementedException();
    }
}