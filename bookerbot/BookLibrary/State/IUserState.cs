using BookLibrary.Context;

namespace BookLibrary.State;

public interface IUserState
{
    Task<ResponseMessage> Handle(UserContext userContext, string message);
}