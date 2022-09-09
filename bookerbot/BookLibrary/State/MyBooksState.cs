using bookerbot.Context;
using bookerbot.DataLayer.Repositories.UserBook;

namespace bookerbot.State;

public class MyBooksState : IUserState
{
    public static string Back = "◀️ Назад";

    private readonly UserBookRepository _userBookRepository;

    public MyBooksState(UserBookRepository userBookRepository)
    {
        _userBookRepository = userBookRepository;
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message == Back)
        {
            return await ProfileState.GetResponseMessage(userContext);
        }

        string bookName = message.TrimStart('❌');

        await _userBookRepository.DeleteByBookName(userContext.UserId, bookName);

        return await GetResponseMessage(userContext);
    }

    public async Task<ResponseMessage> GetResponseMessage(UserContext context)
    {
        context.State = EContextState.MyBooks;

        IEnumerable<UserBookView> books = await _userBookRepository.GetByUserId(context.UserId);

        return new ResponseMessage
        {
            Text = "Мои книги:",
            UpButtons = new List<string> { Back },
            Buttons = books.Select(x => "❌" + x.Title).ToList(),
            ResponseMessageType = EResponseMessageType.Text
        };
    }
}