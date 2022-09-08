using bookerbot.Context;

namespace bookerbot.State;

public class ProfileState : IUserState
{
    public static string MyBooks = "📚 Мои книги";
    public static string City = "🌆 Город";
    public static string Back = "◀️ Назад";

    public ProfileState()
    {
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message == MyBooks)
        {
            userContext.State = EContextState.Profile;

            return new ResponseMessage
            {
                Text = "Профиль",
                UpButtons = new List<string> { MyBooks, City, Back },
                ResponseMessageType = EResponseMessageType.Text
            };
        }
        else if (message == City)
        {
            userContext.State = EContextState.Profile;

            return new ResponseMessage
            {
                Text = "Профиль",
                UpButtons = new List<string> { MyBooks, City, Back },
                ResponseMessageType = EResponseMessageType.Text
            };
        }

        userContext.State = EContextState.Exchange;

        return new ResponseMessage
        {
            Text = "Название книги 1",
            UpButtons = new List<string> { ExchangeState.Like, ExchangeState.Dislike },
            DownButtons = new List<string> {ExchangeState.AddBook, ExchangeState.Profile },
            PhotoUrl = "https://cdn.book24.ru/v2/ASE000000000862783/COVER/cover3d1__w220.jpg",
            ResponseMessageType = EResponseMessageType.Photo
        };
    }
}