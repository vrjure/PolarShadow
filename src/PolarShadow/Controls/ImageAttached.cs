using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
        static ImageAttached()
        {
            SrcProperty.Changed.Subscribe(SrcPropertyChanged);
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

        public static readonly AttachedProperty<IBufferCache> CacheProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, IBufferCache>("Cache");
        public static IBufferCache GetCache(Image image)
        {
            return image.GetValue(CacheProperty);
        }
        public static void SetCache(Image image, IBufferCache value)
        {
            image.SetValue(CacheProperty, value);
        }

        private static void SrcPropertyChanged(AvaloniaPropertyChangedEventArgs<Uri> arg)
        {
            if (Design.IsDesignMode || !arg.NewValue.HasValue) return;
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
                image.Source = null;
                return;
            }

            if (src.Scheme == Uri.UriSchemeHttp || src.Scheme == Uri.UriSchemeHttps)
            {
                var cache = GetCache(image);
                image.Source = await LoadFromWebAsync(src, cache);
            }
            else
            {
                image.Source = new Bitmap(AssetLoader.Open(src));
            }
        }

        private static async Task<Bitmap> LoadFromWebAsync(Uri uri, IBufferCache cache = null)
        {

            using var httpClient = cache switch
            {
                not null => new HttpClient(new HttpCachingHandler(cache)),
                _ => new HttpClient()
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62");
            try
            {
                using var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return new Bitmap(await response.Content.ReadAsStreamAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while downloading image '{uri}' : {ex.Message}");
            }
            return null;
        }
    }
}
