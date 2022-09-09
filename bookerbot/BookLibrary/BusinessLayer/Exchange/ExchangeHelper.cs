using BookLibrary.DataLayer.Repositories.Exchange;
using BookLibrary.DataLayer.Repositories.User;
using Telegram.Bot;

namespace BookLibrary.BusinessLayer.Exchange;

public class ExchangeHelper
{
   private readonly UserRepository _userRepository;
   private readonly ExchangeRepository _exchangeRepository;
   
   public ExchangeHelper(UserRepository userRepository,
      ExchangeRepository exchangeRepository)
   {
      _userRepository = userRepository;
      _exchangeRepository = exchangeRepository;
   }

   public async Task Start(ITelegramBotClient botClient)
   {
      var users = await _userRepository.GetAll();

      foreach (var user in users)
      {
         try
         {
           var exchanges = await _exchangeRepository.GetExchange(user.Id);
            
            
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
      }
   }
}