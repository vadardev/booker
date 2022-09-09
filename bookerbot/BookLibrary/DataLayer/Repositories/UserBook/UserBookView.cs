namespace bookerbot.DataLayer.Repositories.UserBook;

public class UserBookView
{
    public Guid Id { get; set; }
    public string Isbn { get; set; }
    public string Title { get; set; }
    public string Authors { get; set; }
    public string PhotoUrl { get; set; }
    public string? SiteUrl { get; set; }
    public decimal Price { get; set; }
    public DateTime CreateDate { get; set; } 
}