using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public class Aria2File
    {
        /// <summary>
        /// Index of the file, starting at 1, in the same order as files appear in the multi-file torrent.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// File path.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// File size in bytes.
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// Completed length of this file in bytes. 
        /// Please note that it is possible that sum of completedLength is less than the completedLength returned by the aria2.tellStatus() method. 
        /// This is because completedLength in aria2.getFiles() only includes completed pieces. 
        /// On the other hand, completedLength in aria2.tellStatus() also includes partially completed pieces.
        /// </summary>
        public long CompletedLength { get; set; }
        /// <summary>
        /// true if this file is selected by --select-file option. 
        /// If --select-file is not specified or this is single-file torrent or not a torrent download at all, this value is always true. 
        /// Otherwise false.
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// Returns a list of URIs for this file. 
        /// The element type is the same struct used in the aria2.getUris() method.
        /// </summary>
        public ICollection<Aria2Uri> Uris { get; set; }
    }
}
