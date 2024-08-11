using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Services
{
    public static class JsonOptions
    {
        static JsonOptions()
        {
            DefaultSerializer = new JsonSerializerOptions();
            Default(DefaultSerializer);

            FormatSerializer = new JsonSerializerOptions(DefaultSerializer)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                WriteIndented = true,
            };

            DefaultWriteOption = new JsonWriterOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            FormatWriteOption = new JsonWriterOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Indented = true,
            };
        }

        public static JsonSerializerOptions DefaultSerializer { get; }

        public static JsonSerializerOptions FormatSerializer { get; }

        public static JsonWriterOptions DefaultWriteOption { get; }

        public static JsonWriterOptions FormatWriteOption { get; }

        public static void Default(JsonSerializerOptions option)
        {
            option.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            option.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            option.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            option.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            //option.Converters.Add(new DateTimeConverter());
            
        }
    }
}
