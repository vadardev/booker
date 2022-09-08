namespace bookerbot.DataLayer.Repositories.LikeBook;

public class LikeBookEntity
{
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }
    public bool IsLike { get; set; }
    public DateTime CreateDate { get; set; }
}