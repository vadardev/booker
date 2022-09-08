using System.Net.Http.Headers;
using bookerbot.Context;
using bookerbot.DataLayer.Repositories.Book;
using Newtonsoft.Json;
using telegrambotconsole.DataLayer.Repositories.User;
using telegrambotconsole.DataLayer.Repositories.UserBook;

namespace bookerbot.State;

public class AddBookState : IUserState
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly BookRepository _bookRepository;
    private readonly UserBookRepository _userBookRepository;

    public AddBookState(IHttpClientFactory httpClientFactory,
        BookRepository bookRepository,
        UserBookRepository userBookRepository)
    {
        _httpClientFactory = httpClientFactory;
        _bookRepository = bookRepository;
        _userBookRepository = userBookRepository;
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        userContext.State = EContextState.Profile;

        string isbn = message.Trim();

        BookEntity? book = await _bookRepository.GetByIsbn(isbn);

        if (book == null)
        {
            NewBook? newBook = await GetBook(isbn);

            if (newBook == null)
            {
                return new ResponseMessage
                {
                    Text = "Не удалось найти книгу",
                    UpButtons = new List<string> { ProfileState.MyBooks, ProfileState.City, ProfileState.Back },
                    ResponseMessageType = EResponseMessageType.Text
                };
            }

            book = new BookEntity();
            await _bookRepository.Add(book);
        }

        await _userBookRepository.Add(new UserBookEntity());

        return new ResponseMessage
        {
            Text = "Книга успешно добавлена!",
            UpButtons = new List<string> { ProfileState.MyBooks, ProfileState.City, ProfileState.Back },
            ResponseMessageType = EResponseMessageType.Text
        };
    }

    private async Task<NewBook?> GetBook(string isbn)
    {
        var httpClient = _httpClientFactory.CreateClient("books");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        string uri = $"api/v1/catalog/search/suggests/?q={isbn}&limit_suggest=1&limit_product=1&limit_section=3&limit_podborki=0&SESSID=92633865-91fe-436b-ba34-9e7bb0bab239";
        NewBookResponse? response = JsonConvert.DeserializeObject<NewBookResponse>(await httpClient.GetStringAsync(uri));

        if (response?.Books != null && response.Books.Any())
        {
            return response.Books.First();
        }

        return null;
    }
}

public class NewBook
{
    public string Title { get; set; }
    public string Link { get; set; }
    public Meta Meta { get; set; }
}
    
public class Meta
{
    public string Image { get; set; }
    public decimal Price { get; set; }
    public List<Author> Author { get; set; }
}

public class Author
{
    public string Name { get; set; }
}
    
public class NewBookResponse
{
    [JsonProperty("Data")] public List<NewBook>? Books { get; set; }
}