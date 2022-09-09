namespace BookLibrary.BusinessLayer.ShowBook;

public class ShowBookModel
{
    public Guid BookId { get; set; }
    public string Isbn { get; set; }
    public string Title { get; set; }
    public string PhotoUrl { get; set; }
}