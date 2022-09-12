using BookLibrary.Bot;
using BookLibrary.DataLayer.Repositories.Exchange;
using BookLibrary.DataLayer.Repositories.User;
using Telegram.Bot;

namespace BookLibrary.BusinessLayer.Exchange;

public class ExchangeHelper
{
   private readonly UserRepository _userRepository;
   private readonly ExchangeRepository _exchangeRepository;
   private readonly BotStarter _botStarter;

   private Dictionary<Guid, ExchangeModel> _exchanges = new();

   public ExchangeHelper(UserRepository userRepository,
      ExchangeRepository exchangeRepository,
      BotStarter botStarter)
   {
      _userRepository = userRepository;
      _exchangeRepository = exchangeRepository;
      _botStarter = botStarter;
   }

   public async Task Start(ITelegramBotClient botClient)
   {
      var users = await _userRepository.GetAll();

      foreach (var user in users)
      {
         try
         {
            if (!_exchanges.TryGetValue(user.Id, out var userExchanges))
            {
               var exchanges = await _exchangeRepository.GetExchange(user.Id);

               userExchanges = new ExchangeModel();
               foreach (var exchangeView in exchanges)
               {
                  userExchanges.LikeUserBookModels.Add(new LikeUserBookModel
                  {
                     UserName = exchangeView.UserName,
                     LikeBookTitle = exchangeView.LikeBookTitle,
                     UserBookTitle = exchangeView.UserBookTitle,
                  });
               }
            }

            var likeUserBook = userExchanges.LikeUserBookModels.FirstOrDefault();

            if (likeUserBook != null)
            {
               var exchanges = userExchanges.LikeUserBookModels
                  .Where(x => x.UserName == likeUserBook.UserName
                              && x.LikeBookTitle == likeUserBook.LikeBookTitle
                              && x.UserBookTitle == likeUserBook.UserBookTitle).ToList();

               _exchanges.Remove(user.Id);

               if (exchanges.Any())
               {
                  _exchanges.TryAdd(user.Id, new ExchangeModel
                  {
                     LikeUserBookModels = exchanges,
                  });
               }

               string message = $@"@{likeUserBook.UserName} понравилась ваша книга {likeUserBook.UserBookTitle} и готов обменятся 
на {likeUserBook.LikeBookTitle}";
               
               await _botStarter.SendMessage(botClient, user.ChatId, user.Id, message, CancellationToken.None);
            }
            else
            {
               _exchanges.Remove(user.Id);
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
      }
   }
}