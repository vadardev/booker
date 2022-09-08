using bookerbot.State;

namespace bookerbot.Context;

public class UserContext
{
    private readonly StateFactory _stateFactory;

    public EContextState State { get; set; }

    public UserContext(StateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }

    public Task<ResponseMessage> Change(string message)
    {
        IUserState state = _stateFactory.GetState(State);

        return state.Handle(this, message);
    }
}