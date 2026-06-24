using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace RecyclingWorld.Utilities
{
    public static class SessionExtensions // This class provides extension methods for the ISession interface, allowing you to easily store and retrieve complex objects in the session using JSON serialization. The SetObject method serializes an object to a JSON string and stores it in the session under a specified key, while the GetObject method retrieves the JSON string from the session, deserializes it back into an object of the specified type, and returns it. If the key does not exist in the session, GetObject returns the default value for the type (which is null for reference types).
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        { session.SetString(key, JsonSerializer.Serialize(value)); 
        
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}

// The SessionExtensions class provides two extension methods for the ISession interface: SetObject and GetObject. These methods allow you to store and retrieve complex objects in the session by serializing them to JSON strings. The SetObject method takes a key and a value, serializes the value to JSON, and stores it in the session under the specified key. The GetObject method retrieves the JSON string from the session using the key, deserializes it back into an object of the specified type, and returns it. If the key does not exist in the session, GetObject returns the default value for that type (which is null for reference types). This approach simplifies working with complex data in sessions without needing to manually handle serialization and deserialization each time.