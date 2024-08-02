using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal class IcoInfo
    {
        private static byte[] JPEGHeader => new byte[] { 0xFF, 0xD8 };
        private static byte[] PNGHeader => new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        private static byte[] BMPHeader => new byte[] { 0x28, 0x00, 0x00, 0x00 };
        private static byte[] BMPFileHeader => new byte[] { 0x42, 0x4D };

        public IcoInfo(byte[] data)
        {
            this.Data = data;
            if (!GetFormat())
            {
                throw new NotSupportedException("Unknown format in ico");
            }
        }

        public byte[] Data { get; }
        public ImageFormat Format { get; private set; }

        private bool IsJPEG => Data != null && Data.Take(2).SequenceEqual(JPEGHeader);
        private bool IsPNG => Data != null && Data.Take(4).SequenceEqual(PNGHeader);
        private bool IsBMP => Data != null && Data.Take(4).SequenceEqual(BMPHeader);

        private bool GetFormat()
        {
            if (IsJPEG)
            {
                Format = ImageFormat.JPEG;
            }
            else if (IsPNG)
            {
                Format = ImageFormat.PNG;
            }
            else if (IsBMP)
            {
                Format = ImageFormat.BMP;
            }

            return Format != ImageFormat.None;
        }

        public Stream GetStream()
        {
            if(Format == ImageFormat.BMP)
            {
                var ms = new MemoryStream();
                ms.Write(BMPFileHeader);
                ms.Write(BitConverter.GetBytes(14 + Data.Length));
                ms.Write(new byte[] { 0, 0, 0, 0 });
                ms.Write(BitConverter.GetBytes(54));
                ms.Write(Data);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
            else
            {
                return new MemoryStream(Data);
            }
        }
    }

    public enum ImageFormat
    {
        None,
        JPEG,
        PNG,
        BMP
    }
}
