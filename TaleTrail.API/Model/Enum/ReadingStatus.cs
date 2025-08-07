namespace TaleTrail.API.Model.Enums;

/// <summary>
/// Represents the reading status of a book in a user's library
/// </summary>
public enum ReadingStatus
{
    /// <summary>
    /// Book is in the user's to-read list
    /// </summary>
    ToRead = 0,

    /// <summary>
    /// User is currently reading this book
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// User has completed reading this book
    /// </summary>
    Completed = 2,

    /// <summary>
    /// User has dropped/abandoned this book
    /// </summary>
    Dropped = 3
}