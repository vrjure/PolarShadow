using CommunityToolkit.Mvvm.DependencyInjection;
using PolarShadow.Essentials;
using PolarShadows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolarShadow.ImageHelper
{
    class ImageAttached
    {
        public static readonly DependencyProperty SrcProperty = DP.RegisterAttached<ImageAttached, string>("Src", PropertyChanged);
        public static string GetSrc(Image image)
        {
            return (string)image.GetValue(SrcProperty);
        }
        public static void SetSrc(Image image, string value)
        {
            image.SetValue(SrcProperty, value);
        }


        public static readonly DependencyProperty PlaceholderSrcProperty = DP.RegisterAttached<ImageAttached, string>("PlaceholderSrc", PropertyChanged);
        public static string GetPlaceholderSrc(Image image)
        {
            return (string)image.GetValue(PlaceholderSrcProperty);
        }
        public static void SetPlaceholderSrc(Image image, string value)
        {
            image.SetValue(PlaceholderSrcProperty, value);
        }

        public static readonly DependencyProperty CacheProperty = DP.RegisterAttached<ImageAttached, IBufferCache>("Cache");
        public static IBufferCache GetCache(Image image)
        {
            return (IBufferCache)image.GetValue(CacheProperty);
        }
        public static void SetCache(Image image, IBufferCache value)
        {
            image.SetValue(CacheProperty, value);
        }

        public static readonly DependencyProperty CacheLocationProperty = DP.RegisterAttached<ImageAttached, BufferLocation>("CacheLocation", BufferLocation.None);
        public static BufferLocation GetCacheLocation(Image image)
        {
            return (BufferLocation)image.GetValue(CacheLocationProperty);
        }
        public static void SetCacheLocation(Image image, BufferLocation location)
        {
            image.SetValue(CacheLocationProperty, location);
        }


        public static readonly DependencyProperty HeadersProperty = DP.RegisterAttached<ImageAttached, IDictionary<string, string>>("Headers");
        public static IDictionary<string, string> GetHeaders(Image image) => (IDictionary<string,string>)image.GetValue(HeadersProperty);
        public static void SetHeaders(Image image, IDictionary<string, string> value) => image.SetValue(HeadersProperty, value);


        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = (Image)d;
            if (!image.IsLoaded)
            {
                image.Loaded += Image_Loaded;
            }
            else
            {
                LoadImage(image);
            }
        }

        private static void Image_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImage(sender as Image);
        }

        private static async void LoadImage(Image image)
        {
            var src = GetSrc(image);
            var placeholderSrc = GetPlaceholderSrc(image);

            if (src == null && placeholderSrc == null)
            {
                return;
            }

            image.ImageFailed += Image_ImageFailed;
            
            if (!string.IsNullOrEmpty(src))
            {
                await LoadImage(image, new Uri(src));
            }

            if (image.Source == null && !string.IsNullOrEmpty(placeholderSrc))
            {
                await LoadImage(image, new Uri(placeholderSrc));
            }
        }

        private static void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            (sender as Image).Source = null;
        }

        private static async Task LoadImage(Image image, Uri src)
        {
            try
            {
                if (src.IsAbsoluteUri && (src.Scheme == Uri.UriSchemeHttp || src.Scheme == Uri.UriSchemeHttps))
                {
                    var cache = GetCache(image);
                    var headers = GetHeaders(image);
                    var cacheLoc = GetCacheLocation(image);
                     
                    image.Source = await LoadFromWebAsync(src, cache, cacheLoc, headers);
                }
                else
                {
                    image.Source = CreateBitmapImage(src);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Load image error: {ex}");
            }
        }

        private static async Task<BitmapImage> LoadFromWebAsync(Uri uri, IBufferCache cache = null, BufferLocation cacheLocation = BufferLocation.Memory, IDictionary<string, string> headers = null)
        {
            cache ??= Ioc.Default.GetService<IBufferCache>();
            if (cacheLocation == BufferLocation.None)
            {
                cacheLocation = BufferLocation.File;
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
                System.Diagnostics.Trace.WriteLine($"Add header error:{ex}");
            }

            try
            {
                using var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return CreateBitmapImage(await response.Content.ReadAsStreamAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Downloading image error '{uri}' : {ex}");
                cache.Remove(SHA.SHA256(uri.ToString()));
            }
            return null;
        }

        private static BitmapImage CreateBitmapImage(Stream stream)
        {
            var bitmapImage = CreateBitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;//release stream. e...
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private static BitmapImage CreateBitmapImage(Uri uri)
        {
            var bitmapImage = CreateBitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private static BitmapImage CreateBitmapImage()
        {
            var bitmapImage = new BitmapImage();
            return bitmapImage;
        }
    }
}
