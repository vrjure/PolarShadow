using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using PolarShadow.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class ImageAttached
    {
        private static readonly Uri _defaultPlaceholderUri = new Uri("avares://PolarShadow/Assets/images/item-background.jpg");
        static ImageAttached()
        {
            SrcProperty.Changed.AddClassHandler<Image>((o, e) => SrcPropertyChanged(e));
        }
        public static readonly AttachedProperty<Uri> SrcProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, Uri>("Source", _defaultPlaceholderUri);
        public static Uri GetSrc(Image image)
        {
            return image.GetValue(SrcProperty);
        }
        public static void SetSrc(Image image, Uri value)
        {
            image.SetValue(SrcProperty, value);
        }


        public static readonly AttachedProperty<Uri> PlaceholderSrcProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, Uri>("PlaceholderSrc");
        public static Uri GetPlaceholderSrc(Image image)
        {
            return image.GetValue(PlaceholderSrcProperty);
        }
        public static void SetPlaceholderSrc(Image image, Uri value)
        {
            image.SetValue(PlaceholderSrcProperty, value);
        }


        public static readonly AttachedProperty<IBufferCache> CacheProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, IBufferCache>("Cache");
        public static IBufferCache GetCache(Image image)
        {
            return image.GetValue(CacheProperty);
        }
        public static void SetCache(Image image, IBufferCache value)
        {
            image.SetValue(CacheProperty, value);
        }


        public static readonly AttachedProperty<IDictionary<string, string>> HeadersProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, IDictionary<string, string>>("Headers");
        public static IDictionary<string, string> GetHeaders(Image image) => image.GetValue(HeadersProperty);
        public static void SetHeaders(Image image, IDictionary<string, string> value) => image.SetValue(HeadersProperty, value);

        private static void SrcPropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            if (Design.IsDesignMode) return;
            var image = arg.Sender as Image;
            
            if (!image.IsLoaded )
            {
                image.Loaded += Image_Loaded;
            }
            else
            {
                ApplySource(image);
            }
        }

        private static void Image_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var image = sender as Image;
            image.Loaded -= Image_Loaded;
            ApplySource(image);
        }

        private static async void ApplySource(Image image)
        {
            var src = GetSrc(image);
            if (src == null)
            {
                var placeholderSrc = GetPlaceholderSrc(image);
                if (placeholderSrc != null)
                {
                    src = placeholderSrc;
                }
                else
                {
                    image.Source = null;
                    return;
                }
            }

            if (src.Scheme == Uri.UriSchemeHttp || src.Scheme == Uri.UriSchemeHttps)
            {
                var cache = GetCache(image);
                var headers = GetHeaders(image);
                image.Source = await LoadFromWebAsync(src, cache, headers);
            }
            else
            {
                image.Source = new Bitmap(AssetLoader.Open(src));
            }
        }

        private static async Task<Bitmap> LoadFromWebAsync(Uri uri, IBufferCache cache = null, IDictionary<string, string> headers = null)
        {
            cache ??= Ioc.Default.GetService<IBufferCache>();
            using var httpClient = cache switch
            {
                not null => new HttpClient(new HttpCachingHandler(cache)),
                _ => new HttpClient()
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62");

            try
            {
                if (headers != null && headers.Count > 0)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Add header error:{ex.Message}");
            }

            try
            {
                using var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return new Bitmap(await response.Content.ReadAsStreamAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Downloading image error '{uri}' : {ex.Message}");
            }
            return null;
        }
    }
}
