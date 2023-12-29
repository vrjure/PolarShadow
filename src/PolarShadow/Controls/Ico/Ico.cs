using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class Ico
    {
        private IcoFile _file;
        
        public Ico(Stream stream)
        {
            Read(stream);
        }

        public int Count => _file.Count;
        public ImageFormat GetFormat(int index) => _file.Infos[index].Format;

        public Stream GetStream(int index)
        {
            var info = _file.Infos[index];
            return info.GetStream();
        }

        private void Read(Stream stream)
        {
            _file = new IcoFile();
            using (var br = new BinaryReader(stream))
            {
                br.ReadInt16();
                _file.Type = br.ReadInt16();
                _file.Count = br.ReadInt16();
                _file.Entries = new IcoEntry[_file.Count];
                for (int i = 0; i < _file.Count; i++)
                {
                    _file.Entries[i] = ReadEntry(br);
                }

                _file.Infos = new IcoInfo[_file.Count];
                for (int i = 0; i < _file.Count; i++)
                {
                    var entry = _file.Entries[i];
                    _file.Infos[i] = ReadInfo(br, entry);
                }
            }
        }

        private IcoEntry ReadEntry(BinaryReader reader)
        {
            var entry = new IcoEntry();
            entry.Width = reader.ReadByte();
            entry.Height = reader.ReadByte();
            entry.Colors = reader.ReadByte();
            reader.ReadByte();
            entry.Planes = reader.ReadInt16();
            entry.BitsPrePixel = reader.ReadInt16();
            entry.Length = reader.ReadInt32();
            entry.Offset = reader.ReadInt32();
            return entry;
        }

        private IcoInfo ReadInfo(BinaryReader reader, IcoEntry entry)
        {
            return new IcoInfo(reader.ReadBytes(entry.Length));
        }

        public override string ToString()
        {
            if (_file != null)
            {
                var s = new StringBuilder();
                s.AppendLine($"{nameof(IcoFile.Type)}:{_file.Type}");
                s.AppendLine($"{nameof(IcoFile.Count)}:{_file.Count}");
                s.AppendLine("------------------------------------------");
                for (int i = 0; i < _file.Count; i++)
                {
                    
                    s.AppendLine($"Index:{i}");
                    var entry = _file.Entries[i];
                    var info = _file.Infos[i];

                    s.AppendLine($"Size:{entry.Width}*{entry.Height}");
                    s.AppendLine($"Bit:{entry.BitsPrePixel}");
                    s.AppendLine($"Format:{info.Format}");
                    s.AppendLine("-----------------------------------------");
                }

                return s.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
