using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TaleTrail.API.Model;

/// <summary>
/// Represents the reading status of a book in a user's library
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReadingStatus
{
    /// <summary>
    /// Book is in the user's to-read list
    /// </summary>
    [EnumMember(Value = "ToRead")]
    ToRead = 0,

    /// <summary>
    /// User is currently reading this book
    /// </summary>
    [EnumMember(Value = "InProgress")]
    InProgress = 1,

    /// <summary>
    /// User has completed reading this book
    /// </summary>
    [EnumMember(Value = "Completed")]
    Completed = 2,

    /// <summary>
    /// User has dropped/abandoned this book
    /// </summary>
    [EnumMember(Value = "Dropped")]
    Dropped = 3
}