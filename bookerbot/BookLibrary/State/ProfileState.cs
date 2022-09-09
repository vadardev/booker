using BookLibrary.Context;
using BookLibrary.DataLayer.Repositories.User;
using telegrambotconsole.DataLayer.Repositories.User;

namespace BookLibrary.State;

public class ProfileState : IUserState
{
    public static string AddBook = "📕 Добавить книгу";
    public static string MyBooks = "📚 Мои книги";
    public static string City = "🌆 Город";
    public static string Back = "◀️ Назад";

    private readonly ExchangeState _exchangeState;
    private readonly MyBooksState _myBooksState;
    private readonly UserRepository _userRepository;

    public ProfileState(ExchangeState exchangeState,
        MyBooksState myBooksState,
        UserRepository userRepository)
    {
        _exchangeState = exchangeState;
        _myBooksState = myBooksState;
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
            return await _myBooksState.GetResponseMessage(userContext);
        }
        else if (message == City)
        {
            return await AddCityState.GetResponseMessage(userContext);
        }

        if (message == Back)
        {
            return await _exchangeState.GetResponseMessage(userContext);
        }

        return await GetResponseMessage(userContext);
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