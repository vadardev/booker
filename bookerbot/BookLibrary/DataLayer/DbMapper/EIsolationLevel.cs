namespace bookerbot.DataLayer.DbMapper
{
    public enum EIsolationLevel
    {
        ReadUncommited,
        ReadCommited,
        RepeatableRead,
        Serializable
    }
}