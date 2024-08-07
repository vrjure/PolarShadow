namespace PolarShadow.Api.Utilities.Extensions
{
    public static class FileSafeOperateExtensions
    {

        public static IApplicationBuilder InitializeFileSafeOperate(this WebApplication app)
        {
            var fso = app.Services.GetService<FileSafeOperate>();
            if (fso != null)
            {
                fso.Initialize();
            }

            return app;
        }
    }
}
