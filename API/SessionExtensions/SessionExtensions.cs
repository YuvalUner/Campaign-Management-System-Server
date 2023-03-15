using System.Text.Json;
using API.Utils;

namespace API.SessionExtensions;

/// <summary>
/// Session extension methods for setting and getting objects from the session.<br/>
/// Provides a way to store objects in the session, instead of just strings and numbers.<br/>
/// Use in conjunction with <see cref="Constants"/> to maintain consistency.<br/>
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    /// Sets an object in the session.<br/>
    /// </summary>
    /// <param name="session"></param>
    /// <param name="key">The key of the object</param>
    /// <param name="value">The object's value.</param>
    /// <typeparam name="T"></typeparam>
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    /// <summary>
    /// Gets an object from the session.<br/>
    /// </summary>
    /// <param name="session"></param>
    /// <param name="key">The key of the object.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}