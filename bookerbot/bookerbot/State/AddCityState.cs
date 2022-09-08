using bookerbot.Context;
using bookerbot.DataLayer.Repositories.City;
using bookerbot.DataLayer.Repositories.User;

namespace bookerbot.State;

public class AddCityState : IUserState
{
    public static string Back = "◀️ Назад";

    private readonly CityRepository _cityRepository;
    private readonly UserRepository _userRepository;

    public AddCityState(CityRepository cityRepository,
        UserRepository userRepository)
    {
        _cityRepository = cityRepository;
        _userRepository = userRepository;
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message != Back)
        {
            string cityName = message.Trim().ToUpper();

            CityEntity? city = await _cityRepository.GetByName(cityName);

            if (city == null)
            {
                city = new CityEntity
                {
                    Id = Guid.NewGuid(),
                    Name = cityName,
                };
                await _cityRepository.Add(city);
            }

            await _userRepository.SetCityId(userContext.UserId, city.Id);
        }

        return await ProfileState.GetResponseMessage(userContext);
    }

    public static async Task<ResponseMessage> GetResponseMessage(UserContext context)
    {
        context.State = EContextState.AddCity;

        return new ResponseMessage
        {
            Text = "Напишите ваш город:",
            UpButtons = new List<string> { AddCityState.Back },
            ResponseMessageType = EResponseMessageType.Text
        };
    }
}