namespace Tim.DataAccess.Configuration
{
    public enum SqlServerRetryEvent
    {
        VictimDeadlock = 1205,
        ResolvingStatus = 983,
        ReadonlyTrySaveChanges = 978,
        Network = 53,
        Timeout1 = -1,
        Timeout2 = -2,
    }
}
