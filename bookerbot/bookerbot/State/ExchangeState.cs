using bookerbot.BusinessLayer.ShowBook;
using bookerbot.Context;
using bookerbot.DataLayer.Repositories.LikeBook;
using bookerbot.DataLayer.Repositories.User;
using bookerbot.Images;
using telegrambotconsole.DataLayer.Repositories.User;

namespace bookerbot.State;

public class ExchangeState : IUserState
{
    public static string Like = "❤️ Нравится";
    public static string Dislike = "🚫 Не очень";
    public static string AddBook = "📕 Добавить книгу";
    public static string Profile = "💼 Профиль";
    
    private readonly LikeBookRepository _likeBookRepository;
    private readonly UserRepository _userRepository;
    private readonly ShowBookHelper _showBookHelper;

    public ExchangeState(LikeBookRepository likeBookRepository,
        UserRepository userRepository,
        ShowBookHelper showBookHelper)
    {
        _likeBookRepository = likeBookRepository;
        _userRepository = userRepository;
        _showBookHelper = showBookHelper;
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message == Like)
        {
            if (userContext.ShowBookId.HasValue)
            {
                await _likeBookRepository.Add(new LikeBookEntity
                {
                    BookId = userContext.ShowBookId.Value,
                    UserId = userContext.UserId,
                    IsLike = true,
                    CreateDate = DateTime.UtcNow,
                });
            }

            return await GetResponseMessage(userContext);
        }
        else if (message == Dislike)
        {
            if (userContext.ShowBookId.HasValue)
            {
                await _likeBookRepository.Add(new LikeBookEntity
                {
                    BookId = userContext.ShowBookId.Value,
                    UserId = userContext.UserId,
                    IsLike = false,
                    CreateDate = DateTime.UtcNow,
                });
            }

            return await GetResponseMessage(userContext);
        }
        else if (message == Profile)
        {
            return await ProfileState.GetResponseMessage(userContext);
        }
        else if (message == AddBook)
        {
            UserEntity? user = await _userRepository.Get(userContext.UserId);

            if (user?.CityId == null)
            {
                return await AddCityState.GetResponseMessage(userContext);
            }

            return await AddBookState.GetResponseMessage(userContext);
        }


        return await GetResponseMessage(userContext);
    }

    public async Task<ResponseMessage> GetResponseMessage(UserContext context)
    {
        ShowBookModel? showBookModel = await _showBookHelper.GetShowBook(context.UserId);

        if (showBookModel != null)
        {
            context.State = EContextState.Exchange;
            context.ShowBookId = showBookModel.BookId;

            return new ResponseMessage
            {
                Text = showBookModel.Title,
                UpButtons = new List<string> { Like, Dislike },
                DownButtons = new List<string> { AddBook, Profile },
                PhotoUrl = $"{Utility.GetDirectoryPath}{showBookModel.Isbn}.jpg",
                ResponseMessageType = EResponseMessageType.Photo
            };
        }

        return await NotFoundExchangeState.GetResponseMessage(context);
    }
}