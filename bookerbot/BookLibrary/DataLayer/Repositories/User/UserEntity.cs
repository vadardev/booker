namespace telegrambotconsole.DataLayer.Repositories.User;

public class UserEntity
{
    public Guid Id { get; set; }
    public long TelegramId { get; set; }
    
    public string? UserName { get; set; }
    public Guid? CityId { get; set; }
    
    public long ChatId { get; set; }
}