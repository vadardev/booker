using BookLibrary.BusinessLayer.Exchange;
using BookLibrary.Context;

namespace BookLibrary.State;

public class UserConnectState : IUserState
{
    public static string Back = "◀️ Назад";

    private readonly ExchangeState _exchangeState;
    
    public UserConnectState(ExchangeState exchangeState)
    {
        _exchangeState = exchangeState;
    }

    public Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        return _exchangeState.GetResponseMessage(userContext);
    }

    public static ResponseMessage GetResponseMessage(UserContext context, string message)
    {
        return new ResponseMessage
        {
            Text = message,
            UpButtons = new List<string> { Back },
            ResponseMessageType = EResponseMessageType.Text,
        };
    }
}