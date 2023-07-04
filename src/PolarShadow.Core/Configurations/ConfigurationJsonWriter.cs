using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core.Configurations
{
    internal class ConfigurationJsonWriter
    {
        private IConfiguration _configuration;
        public ConfigurationJsonWriter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            WriteObject(writer, _configuration);
        }

        private void WriteObject(Utf8JsonWriter writer, IConfiguration section)
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

        private void WriteSection(Utf8JsonWriter writer, IConfigurationSection section)
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

        private void WriteArray(Utf8JsonWriter writer, IConfigurationSection section)
        {
            writer.WriteStartArray(section.Key);
            foreach (var item in section.GetChildren())
            {
                WriteObject(writer, item);
            }
            writer.WriteEndArray();
        }

        private void WriteProperty(Utf8JsonWriter writer, IConfigurationSection section)
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
