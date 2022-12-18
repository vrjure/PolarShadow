using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public class Aria2Peer
    {
        /// <summary>
        /// Percent-encoded peer ID.
        /// </summary>
        public string PeerId { get; set; }
        /// <summary>
        /// IP address of the peer.
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// Port number of the peer.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Hexadecimal representation of the download progress of the peer. 
        /// The highest bit corresponds to the piece at index 0. 
        /// Set bits indicate the piece is available and unset bits indicate the piece is missing. 
        /// Any spare bits at the end are set to zero.
        /// </summary>
        public string Bitfield { get; set; }
        /// <summary>
        /// true if aria2 is choking the peer. Otherwise false.
        /// </summary>
        public bool AmChoking { get; set; }
        /// <summary>
        /// true if the peer is choking aria2. Otherwise false.
        /// </summary>
        public bool PeerChoking { get; set; }
        /// <summary>
        /// Download speed (byte/sec) that this client obtains from the peer.
        /// </summary>
        public int DownloadSpeed { get; set; }
        /// <summary>
        /// Upload speed(byte/sec) that this client uploads to the peer.
        /// </summary>
        public int UploadSpeed { get; set; }
        /// <summary>
        /// true if this peer is a seeder. Otherwise false.
        /// </summary>
        public bool Seeder { get; set; }
    }
}
