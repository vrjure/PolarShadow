using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PolarShadow.Core
{
    internal class TextXmlRepairReader : StreamReader
    {
        private static ReadOnlySpan<byte> _regular => new[]
        {
            (byte)'='
        };

        private int _last = 0;
        private bool _inTag;

        public TextXmlRepairReader(Stream stream) : base(stream)
        {
        }

        public TextXmlRepairReader(string path) : base(path)
        {
        }

        public TextXmlRepairReader(Stream stream, bool detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks)
        {
        }

        public TextXmlRepairReader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public TextXmlRepairReader(string path, bool detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks)
        {
        }

        public TextXmlRepairReader(string path, Encoding encoding) : base(path, encoding)
        {
        }

        public TextXmlRepairReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public TextXmlRepairReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public TextXmlRepairReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public TextXmlRepairReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public TextXmlRepairReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        {
        }

        public override int Read()
        {
            var c = base.Read();
            switch (_last)
            {
                case (int)'=':
                    c = '\"';
                    break;
                case (int)'<':
                    _inTag = true;
                    break;
                default:
                    break;
            }
            _last = c;
            return c;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            return base.Read(buffer, index, count);
        }
    }
}
