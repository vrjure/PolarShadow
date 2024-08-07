using Microsoft.OpenApi.Models;
using PolarShadow.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PolarShadow.Api.Swagger
{
    public class SwaggerGenOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!operation.Responses.TryGetValue("200", out OpenApiResponse? result200))
            {
                result200 = new OpenApiResponse { Description = "Success" };
                operation.Responses.Add("200", result200);
            }

            if (!operation.Responses.ContainsKey("other"))
            {
                operation.Responses.Add("other", new OpenApiResponse { Description = "Other error" });
            }

            if(!result200.Content.TryGetValue("application/json", out OpenApiMediaType? jsonContent))
            {
                jsonContent = new OpenApiMediaType();
                result200.Content.Add("application/json", jsonContent);
            }

            Type resultType = null;
            if (context.MethodInfo.ReturnType == typeof(Task))
            {
                resultType = typeof(Result);
            }
            else if(context.MethodInfo.ReturnType.IsGenericType)
            {
                resultType = typeof(Result<>).MakeGenericType(context.MethodInfo.ReturnType.GenericTypeArguments[0]);
            }
            else
            {
                resultType = context.MethodInfo.ReturnType;
            }

            jsonContent.Schema = context.SchemaGenerator.GenerateSchema(resultType, context.SchemaRepository);
        }
    }
}
