using bookerbot.Context;

namespace bookerbot.State;

public class NotFoundExchangeState : IUserState
{
    public static string Profile = "💼 Профиль";
    public static string Update = "Обновить";

    private readonly ExchangeState _exchangeState;

    public NotFoundExchangeState(ExchangeState exchangeState)
    {
        _exchangeState = exchangeState;
    }

    public Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message == Profile)
        {
            return ProfileState.GetResponseMessage(userContext);
        }

        return _exchangeState.GetResponseMessage(userContext);
    }

    public static Task<ResponseMessage> GetResponseMessage(UserContext context)
    {
        context.State = EContextState.NotFoundExchange;

        return Task.FromResult(new ResponseMessage
        {
            Text = "Подбираем книги для обмена...",
            UpButtons = new List<string> { Profile, Update },
            ResponseMessageType = EResponseMessageType.Text
        });
    }
}