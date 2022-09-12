namespace BookLibrary.BusinessLayer.Exchange;

public class ExchangeModel
{
    public List<LikeUserBookModel> LikeUserBookModels { get; set; } = null!;
}

public class LikeUserBookModel
{
    public string UserName { get; set; } = null!;
    public string LikeBookTitle { get; set; }
    public string UserBookTitle { get; set; }
}