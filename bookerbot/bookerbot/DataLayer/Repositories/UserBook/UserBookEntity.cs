namespace bookerbot.DataLayer.Repositories.UserBook;

public class UserBookEntity
{
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public DateTime CreateDate { get; set; }
}