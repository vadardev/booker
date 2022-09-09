using bookerbot.Context;
using bookerbot.DataLayer.Repositories.Book;
using bookerbot.Images;

namespace bookerbot.State;

public class SuccessAddBookState : IUserState
{
    public static string Back = "◀️ Назад";

    private readonly BookRepository _bookRepository;

    public SuccessAddBookState(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        return ProfileState.GetResponseMessage(userContext);
    }

    public async Task<ResponseMessage> GetResponseMessage(UserContext context, Guid bookId)
    {
        BookEntity? book = await _bookRepository.Get(bookId);
        
        context.State = EContextState.SuccessAddBook;

        return new ResponseMessage
        {
            Text = $"Книга успешно добавлена: {book.Title}",
            UpButtons = new List<string> { Back },
            PhotoUrl = $"{Utility.GetDirectoryPath}{book.Isbn}.jpg",
            ResponseMessageType = EResponseMessageType.Photo
        };
    }
}