namespace TaleTrail.API.Enums
{
    public enum ReadingStatus
    {
        WannaRead,
        InProgress,
        Completed,
        Dropped
    }

    public static class ReadingStatusExtensions
    {
        public static string ToDbString(this ReadingStatus status)
        {
            return status switch
            {
                ReadingStatus.WannaRead => "wanna_read",
                ReadingStatus.InProgress => "in_progress",
                ReadingStatus.Completed => "completed",
                ReadingStatus.Dropped => "dropped",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }

        public static ReadingStatus FromDbString(string dbValue)
        {
            return dbValue switch
            {
                "wanna_read" => ReadingStatus.WannaRead,
                "in_progress" => ReadingStatus.InProgress,
                "completed" => ReadingStatus.Completed,
                "dropped" => ReadingStatus.Dropped,
                _ => throw new ArgumentOutOfRangeException(nameof(dbValue), dbValue, null)
            };
        }

        public static bool IsValidDbString(string dbValue)
        {
            return dbValue is "wanna_read" or "in_progress" or "completed" or "dropped";
        }
    }
}