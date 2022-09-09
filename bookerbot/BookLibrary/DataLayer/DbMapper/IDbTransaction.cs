namespace BookLibrary.DataLayer.DbMapper
{
    public interface IDbTransactionScope : IDisposable
    {
        void Commit();
    }
}