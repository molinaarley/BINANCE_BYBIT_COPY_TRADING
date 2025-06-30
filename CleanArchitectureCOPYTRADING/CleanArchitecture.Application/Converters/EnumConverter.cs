using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Mappings;
using CleanArchitecture.Domain.MapAttribute;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Converters
{
    public class EnumConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, List<KeyValuePair<object, string>>> _mapping = new ConcurrentDictionary<Type, List<KeyValuePair<object, string>>>();

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            Type type = Nullable.GetUnderlyingType(objectType) ?? objectType;
            if (!_mapping.TryGetValue(type, out var value))
            {
                value = AddMapping(type);
            }

            string text = reader.Value?.ToString();
            if (text == null)
            {
                object? defaultValue = GetDefaultValue(objectType, type);
                if (defaultValue != null)
                {
                    Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff} | Warning | Received null enum value, but property type is not a nullable enum. EnumType: {type.Name}. If you think {type.Name} should be nullable please open an issue on the Github repo");
                }

                return defaultValue;
            }

            if (!GetValue(type, value, text, out var result))
            {
                object defaultValue2 = GetDefaultValue(objectType, type);
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (defaultValue2 != null)
                    {
                        Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff} | Warning | Received empty string as enum value, but property type is not a nullable enum. EnumType: {type.Name}. If you think {type.Name} should be nullable please open an issue on the Github repo");
                    }
                }
                else
                {
                    Trace.WriteLine(string.Format("{0:yyyy/MM/dd HH:mm:ss:fff} | Warning | Cannot map enum value. EnumType: {1}, Value: {2}, Known values: {3}. If you think {4} should added please open an issue on the Github repo", DateTime.Now, type.Name, reader.Value, string.Join(", ", value.Select((KeyValuePair<object, string> m) => m.Value)), reader.Value));
                }

                return defaultValue2;
            }

            return result;
        }

        private static object? GetDefaultValue(Type objectType, Type enumType)
        {
            if (Nullable.GetUnderlyingType(objectType) != null)
            {
                return null;
            }

            return Activator.CreateInstance(enumType);
        }

        private static List<KeyValuePair<object, string>> AddMapping(Type objectType)
        {
            List<KeyValuePair<object, string>> list = new List<KeyValuePair<object, string>>();
            MemberInfo[] members = objectType.GetMembers();
            foreach (MemberInfo memberInfo in members)
            {
                object[] customAttributes = memberInfo.GetCustomAttributes(typeof(MapAttribute), inherit: false);
                for (int j = 0; j < customAttributes.Length; j++)
                {
                    string[] values = ((MapAttribute)customAttributes[j]).Values;
                    foreach (string value in values)
                    {
                        list.Add(new KeyValuePair<object, string>(Enum.Parse(objectType, memberInfo.Name), value));
                    }
                }
            }

            _mapping.TryAdd(objectType, list);
            return list;
        }

        private static bool GetValue(Type objectType, List<KeyValuePair<object, string>> enumMapping, string value, out object? result)
        {
            string value2 = value;
            KeyValuePair<object, string> keyValuePair = enumMapping.FirstOrDefault<KeyValuePair<object, string>>((KeyValuePair<object, string> kv) => kv.Value.Equals(value2, StringComparison.InvariantCulture));
            if (keyValuePair.Equals(default(KeyValuePair<object, string>)))
            {
                keyValuePair = enumMapping.FirstOrDefault<KeyValuePair<object, string>>((KeyValuePair<object, string> kv) => kv.Value.Equals(value2, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!keyValuePair.Equals(default(KeyValuePair<object, string>)))
            {
                result = keyValuePair.Key;
                return true;
            }

            try
            {
                result = Enum.Parse(objectType, value2, ignoreCase: true);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        [return: NotNullIfNotNull("enumValue")]
        public static string? GetString<T>(T enumValue)
        {
            T enumValue2 = enumValue;
            Type typeFromHandle = typeof(T);
            typeFromHandle = Nullable.GetUnderlyingType(typeFromHandle) ?? typeFromHandle;
            if (!_mapping.TryGetValue(typeFromHandle, out var value))
            {
                value = AddMapping(typeFromHandle);
            }

            object obj;
            if (enumValue2 != null)
            {
                obj = value.FirstOrDefault((KeyValuePair<object, string> v) => v.Key.Equals(enumValue2)).Value;
                if (obj == null)
                {
                    return enumValue2.ToString();
                }
            }
            else
            {
                obj = null;
            }

            return (string?)obj;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            string @string = GetString(value);
            writer.WriteValue(@string);
        }
    }
}
