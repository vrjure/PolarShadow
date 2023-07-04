using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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
            var writer = new ConfigurationJsonWriter(new NoLoadConfigurationRoot(configuration.Providers.Where(f => f is PolarShadowJsonConfigurationProvider).ToList()));
            using var jsonWriter = new Utf8JsonWriter(stream, options);
            writer.WriteTo(jsonWriter);
            jsonWriter.Flush();
        }
    }
}
