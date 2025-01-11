using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Roguelike.World.Configuration
{
    internal class TilesConfig
    {
        public string Foreground { get; set; }
        public string Background { get; set; }
        public string Obstruction { get; set; }
        public string Type { get; set; }

        [JsonConverter(typeof(CharacterConverter))]
        public int Glyph { get; set; }

        private static Dictionary<TileType, Tile> _configTiles;
        public static Tile Get(TileType tileType)
        {
            if (_configTiles == null) LoadConfiguration();
            if (!_configTiles.TryGetValue(tileType, out var tile))
                throw new Exception($"Missing tile configuration for tile type \"{tileType}\".");
            return tile;
        }

        private static void LoadConfiguration()
        {
            var tilesJson = File.ReadAllText(Constants.TileConfiguration);
            var tiles = JsonConvert.DeserializeObject<List<TilesConfig>>(tilesJson);
            _configTiles = tiles.ToDictionary(a => Enum.Parse<TileType>(a.Type, true), ConvertFromConfigurationTile);
        }

        private static Tile ConvertFromConfigurationTile(TilesConfig tileConfig)
        {
            return new Tile(Enum.Parse<TileType>(tileConfig.Type, true))
            {
                Foreground = HexToColor(tileConfig.Foreground),
                Background = HexToColor(tileConfig.Background),
                Glyph = tileConfig.Glyph,
                Obstruction = Enum.Parse<ObstructionType>(tileConfig.Obstruction, true)
            };
        }

        private static Color HexToColor(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
                throw new ArgumentException("Hex color cannot be null or empty.");

            hexColor = hexColor.TrimStart('#');

            if (hexColor.Length != 6 && hexColor.Length != 8)
                throw new FormatException("Hex color must be 6 or 8 characters long.");

            // Parse RGB components
            byte r = byte.Parse(hexColor[..2], NumberStyles.HexNumber);
            byte g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);

            // Parse alpha if provided, otherwise default to 255 (fully opaque)
            byte a = hexColor.Length == 8
                ? byte.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber)
                : (byte)255;

            return new Color(r, g, b, a);
        }

        class CharacterConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(int);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string strValue = (string)reader.Value;
                    if (!string.IsNullOrEmpty(strValue) && strValue.Length == 1)
                    {
                        // Convert single character string to its ASCII integer value
                        return (int)strValue[0];
                    }
                    throw new JsonSerializationException("String must contain exactly one character.");
                }
                else if (reader.TokenType == JsonToken.Integer)
                {
                    // Return the integer value as is
                    return Convert.ToInt32(reader.Value);
                }
                throw new JsonSerializationException("Unsupported JSON token for integer or single-character string.");
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Serialize the integer as a number
                writer.WriteValue(value);
            }
        }
    }
}
