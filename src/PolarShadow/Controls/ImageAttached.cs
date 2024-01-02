using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using PolarShadow.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class ImageAttached
    {
        static ImageAttached()
        {
            SrcProperty.Changed.AddClassHandler<Image>((o, e) => SrcPropertyChanged(e));
            Image.LoadedEvent.AddClassHandler<Image>(ImageLoad);
        }
        public static readonly AttachedProperty<Uri> SrcProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, Uri>("Source");
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

        public static readonly AttachedProperty<BufferLocation> CacheLocationProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, BufferLocation>("CacheLocation", BufferLocation.None);
        public static BufferLocation GetCacheLoaction(Image image)
        {
            return image.GetValue(CacheLocationProperty);
        }
        public static void SetCacheLocation(Image image, BufferLocation location)
        {
            image.SetValue(CacheLocationProperty, location);
        }


        public static readonly AttachedProperty<IDictionary<string, string>> HeadersProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, IDictionary<string, string>>("Headers");
        public static IDictionary<string, string> GetHeaders(Image image) => image.GetValue(HeadersProperty);
        public static void SetHeaders(Image image, IDictionary<string, string> value) => image.SetValue(HeadersProperty, value);

        private static void ImageLoad(Image image, RoutedEventArgs e)
        {
            if (Design.IsDesignMode) return;
            if (image.Source != null)
            {
                return;
            }
            ApplySource(image);
        }

        private static void SrcPropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            if (Design.IsDesignMode) return;
            var image = arg.Sender as Image;

            if (image.IsLoaded)
            {
                ApplySource(image);
            }
        }

        private static async void ApplySource(Image image)
        {
            var src = GetSrc(image);
            if (src != null)
            {
                await LoadImage(image, src);
            }

            if (image.Source == null)
            {
                var placeholderSrc = GetPlaceholderSrc(image);
                if (placeholderSrc != null)
                {
                    await LoadImage(image, placeholderSrc);
                }
            }
        }

        private static async Task LoadImage(Image image, Uri src)
        {
            try
            {
                if (src.IsAbsoluteUri && (src.Scheme == Uri.UriSchemeHttp || src.Scheme == Uri.UriSchemeHttps))
                {
                    var cache = GetCache(image);
                    var headers = GetHeaders(image);
                    var cacheLoc = GetCacheLoaction(image);
                    image.Source = await LoadFromWebAsync(src, cache, cacheLoc, headers);
                }
                else
                {
                    if (!src.IsAbsoluteUri)
                    {
                        src = new Uri(new Uri("avares://PolarShadow"), src);
                    }

                    image.Source = TryGetImage(AssetLoader.Open(src), src);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Load image error: {ex.Message}");
            }

        }

        private static async Task<Bitmap> LoadFromWebAsync(Uri uri, IBufferCache cache = null, BufferLocation cacheLocation = BufferLocation.Memory, IDictionary<string, string> headers = null)
        {
            cache ??= Ioc.Default.GetService<IBufferCache>();
            if (cacheLocation == BufferLocation.None)
            {
                cacheLocation = BufferLocation.Memory;
            }
            using var httpClient = cache switch
            {
                not null => new HttpClient(new HttpCachingHandler(cache, cacheLocation)),
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

                return TryGetImage(await response.Content.ReadAsStreamAsync(), uri);
            }
            catch (Exception ex)
            {
                cache.Remove(BufferCache.SHA(uri.ToString()));
                System.Diagnostics.Trace.WriteLine($"Downloading image error '{uri}' : {ex.Message}");
            }
            return null;
        }

        private static Bitmap TryGetImage(Stream stream, Uri uri) => TryGetImage(stream, uri.ToString().EndsWith(".ico", StringComparison.OrdinalIgnoreCase));

        private static Bitmap TryGetImage(Stream stream, bool isIco)
        {
            if (isIco)
            {
                var ico = new Ico(stream);
                return new Bitmap(ico.GetStream(ico.Count - 1));
            }
            else
            {
                return new Bitmap(stream);
            }
        }
    }
}
