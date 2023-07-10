using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddPolarShadowJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange) => AddPolarShadowJsonFile(builder, null, path, optional, reloadOnChange);

        public static IConfigurationBuilder AddPolarShadowJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            return builder.AddPolarShadowJsonFile(s =>
            {
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ResolveFileProvider();
            });
        }

        internal static IConfigurationBuilder AddPolarShadowJsonFile(this IConfigurationBuilder builder, Action<PolarShadowJsonConfigurationSource> configureSource) => builder.Add(configureSource);

        public static void WritePolarShadowConfigurationToJsonFile(this IConfigurationRoot configuration, string filePath, JsonWriterOptions options)
        {
            using var fs = new FileStream(filePath, FileMode.Truncate, FileAccess.Write, FileShare.Read);
            WritePolarShadowConfigurationToStream(configuration, fs, options);
        }

        public static void WritePolarShadowConfigurationToStream(this IConfigurationRoot configuration, Stream stream, JsonWriterOptions options)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, options);
            new NoLoadConfigurationRoot(configuration.Providers.Where(f => f is PolarShadowJsonConfigurationProvider).ToList()).WriteToJsonObject(jsonWriter);
            jsonWriter.Flush();
        }

        public static JsonElement ToJsonElement(this IConfiguration configuration)
        {
            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms);
            WriteToJsonObject(configuration, jsonWriter);
            jsonWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            using var doc = JsonDocument.Parse(ms);
            return doc.RootElement.Clone();
        }

        public static void WriteToJsonObject(this IConfiguration configuration, Utf8JsonWriter writer)
        {
            WriteObject(writer, configuration);
        }

        private static void WriteObject(Utf8JsonWriter writer, IConfiguration section)
        {
            writer.WriteStartObject();
            foreach (var item in section.GetChildren())
            {
                if (item.Value != null)
                {
                    WriteProperty(writer, item);
                }
                else
                {
                    WriteSection(writer, item);
                }
            }
            writer.WriteEndObject();
        }

        private static void WriteSection(Utf8JsonWriter writer, IConfigurationSection section)
        {
            var first = section.GetChildren().FirstOrDefault();
            if (first == null)
            {
                writer.WriteNull(section.Key);
                return;
            }

            if (int.TryParse(first.Key, out int _))
            {
                WriteArray(writer, section);
            }
            else
            {
                writer.WritePropertyName(section.Key);
                WriteObject(writer, section);
            }
        }

        private static void WriteArray(Utf8JsonWriter writer, IConfigurationSection section)
        {
            writer.WriteStartArray(section.Key);
            foreach (var item in section.GetChildren())
            {
                WriteObject(writer, item);
            }
            writer.WriteEndArray();
        }

        private static void WriteProperty(Utf8JsonWriter writer, IConfigurationSection section)
        {
            if (decimal.TryParse(section.Value, out decimal decVal))
            {
                writer.WriteNumber(section.Key, decVal);
            }
            else if (bool.TryParse(section.Value, out bool boolVal))
            {
                writer.WriteBoolean(section.Key, boolVal);
            }
            else
            {
                writer.WriteString(section.Key, section.Value);
            }
        }
    }
}
