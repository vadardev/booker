using bookerbot.Context;
using bookerbot.DataLayer.Repositories.User;
using telegrambotconsole.DataLayer.Repositories.User;

namespace bookerbot.State;

public class ProfileState : IUserState
{
    public static string AddBook = "📕 Добавить книгу";
    public static string MyBooks = "📚 Мои книги";
    public static string City = "🌆 Город";
    public static string Back = "◀️ Назад";

    private readonly ExchangeState _exchangeState;
    private readonly UserRepository _userRepository;

    public ProfileState(ExchangeState exchangeState,
        UserRepository userRepository)
    {
        _exchangeState = exchangeState;
        _userRepository = userRepository;
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message == AddBook)
        {
            UserEntity? user = await _userRepository.Get(userContext.UserId);

            if (user?.CityId == null)
            {
                return await AddCityState.GetResponseMessage(userContext);
            }

            return await AddBookState.GetResponseMessage(userContext);
        }

        if (message == MyBooks)
        {
            return await ProfileState.GetResponseMessage(userContext);
        }
        else if (message == City)
        {
            return await ProfileState.GetResponseMessage(userContext);
        }

        return await _exchangeState.GetResponseMessage(userContext);
    }

    public static async Task<ResponseMessage> GetResponseMessage(UserContext context, string? message = null)
    {
        context.State = EContextState.Profile;

        return new ResponseMessage
        {
            Text = message ?? "Профиль",
            UpButtons = new List<string> { AddBook, MyBooks },
            DownButtons = new List<string> {  City, Back },
            ResponseMessageType = EResponseMessageType.Text
        };
    }
}