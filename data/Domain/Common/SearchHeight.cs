using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common;

[JsonConverter(typeof(SearchHeightJsonConverter))]
public class SearchHeight : EnumValueObject<SearchHeight, SearchHeight.SearchHeights>
{
    public static SearchHeight All => new(SearchHeights.All);
    public static SearchHeight OnlyLowGround => new(SearchHeights.OnlyLowGround);
    public static SearchHeight OnlyHighGround => new(SearchHeights.OnlyHighGround);
    public static SearchHeight HighestPossible => new(SearchHeights.HighestPossible);
    
    private SearchHeight(SearchHeights @enum) : base(@enum) { }
        
    private SearchHeight(string? from) : base(from) { }
    
    public enum SearchHeights
    {
        All,
        OnlyLowGround,
        OnlyHighGround,
        HighestPossible
    }
    
    private class SearchHeightJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SearchHeight);
        }
            
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var id = (SearchHeight)value!;
            serializer.Serialize(writer, id.ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) 
                return null;
                
            var value = serializer.Deserialize<string>(reader);
            return new SearchHeight(value);
        }
    }
}