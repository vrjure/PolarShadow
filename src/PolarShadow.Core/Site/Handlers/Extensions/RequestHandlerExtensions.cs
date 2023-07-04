using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class RequestHandlerExtensions
    {
        //public static async Task<string> ExecuteAsync(this IRequestHandler hander, AnalysisAbility ability, NameSlotValueCollection input, CancellationToken cancellation = default)
        //{
        //    if (ability == null || ability.Request == null || ability.Response == null)
        //    {
        //        return default;
        //    }
        //    using var ms = new MemoryStream();
        //    await hander.ExecuteAsync(ability, ms, input, cancellation);
        //    using var sr = new StreamReader(ms);
        //    return sr.ReadToEnd();
        //}

        //public static async Task<TOutput> ExecuteAsync<TOutput>(this IRequestHandler handler, AnalysisAbility ability, NameSlotValueCollection input, CancellationToken cancellation = default)
        //{
        //    if (ability == null || ability.Request == null || ability.Response == null)
        //    {
        //        return default;
        //    }

        //    using var ms = new MemoryStream();
        //    await handler.ExecuteAsync(ability, ms, input, cancellation);
        //    return JsonSerializer.Deserialize<TOutput>(ms, JsonOption.DefaultSerializer);
        //}
    }
}
