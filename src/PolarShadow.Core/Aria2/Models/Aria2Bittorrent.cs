using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    /// <summary>
    /// Struct which contains information retrieved from the .torrent (file). BitTorrent only. It contains following keys.
    /// </summary>
    public class Aria2Bittorrent
    {
        /// <summary>
        /// List of lists of announce URIs.
        /// If the torrent contains announce and no announce-list, announce is converted to the announce-list format.
        /// </summary>
        public ICollection<string> AnnounceList { get; set; }
        /// <summary>
        /// The comment of the torrent. comment.utf-8 is used if available.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// The creation time of the torrent. 
        /// The value is an integer since the epoch, measured in seconds.
        /// </summary>
        public long CreationDate { get; set; }
        /// <summary>
        /// File mode of the torrent. 
        /// The value is either single or multi.
        /// </summary>
        public Aria2FileMode Mode { get; set; }
    }
}
