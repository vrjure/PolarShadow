using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
            SourceProperty.Changed.Subscribe(SourcePropertyChanged);
        }
        public static readonly AttachedProperty<Uri> SourceProperty = AvaloniaProperty.RegisterAttached<ImageAttached, Image, Uri>("Source");
        public static Uri GetSource(Image image)
        {
            return image.GetValue(SourceProperty);
        }
        public static void SetSource(Image image, Uri value)
        {
            image.SetValue(SourceProperty, value);
        }

        private static void SourcePropertyChanged(AvaloniaPropertyChangedEventArgs<Uri> arg)
        {
            if (!arg.NewValue.HasValue) return;

            var image = arg.Sender as Image;
            if (!image.IsLoaded)
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
            var source = GetSource(image);
            if (source == null) return;

            if (source.Scheme == Uri.UriSchemeHttp || source.Scheme == Uri.UriSchemeHttps)
            {
                image.Source = await LoadFromWebAsync(source);
            }
            else
            {
                image.Source = new Bitmap(AssetLoader.Open(source));
            }
        }

        private static async Task<Bitmap> LoadFromWebAsync(Uri uri)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62");
            try
            {
                var response = await httpClient.GetAsync(uri);
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
