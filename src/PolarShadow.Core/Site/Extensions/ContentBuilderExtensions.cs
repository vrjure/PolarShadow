using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class ContentBuilderExtensions
    {
        /// <summary>
        /// 构建模板内容
        /// </summary>
        public static void Write(this IContentWriter builder, Stream output, JsonElement tempate, IParameter parameter)
        {
            using var jsonWriter = new Utf8JsonWriter(output, JsonOption.DefaultWriteOption);
            builder.Write(jsonWriter, tempate, parameter);
            jsonWriter.Flush();
        }
    }
}
