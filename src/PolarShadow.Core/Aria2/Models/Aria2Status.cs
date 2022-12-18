using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public class Aria2Status
    {
        /// <summary>
        /// GID of the download.
        /// </summary>
        public string Gid { get; set; }
        /// <summary>
        /// active for currently downloading/seeding downloads. 
        /// waiting for downloads in the queue; 
        /// download is not started. 
        /// paused for paused downloads. 
        /// error for downloads that were stopped because of error. 
        /// complete for stopped and completed downloads. 
        /// removed for the downloads removed by user.
        /// </summary>
        public Aria2DownloadStatus Status { get; set; }
        /// <summary>
        /// Total length of the download in bytes.
        /// </summary>
        public long TotalLength { get; set; }
        /// <summary>
        /// Completed length of the download in bytes.
        /// </summary>
        public long CompletedLength { get; set; }
        /// <summary>
        /// Uploaded length of the download in bytes.
        /// </summary>
        public long UploadLength { get; set; }
        /// <summary>
        /// Hexadecimal representation of the download progress. 
        /// The highest bit corresponds to the piece at index 0. 
        /// Any set bits indicate loaded pieces, while unset bits indicate not yet loaded and/or missing pieces. 
        /// Any overflow bits at the end are set to zero. 
        /// When the download was not started yet, this key will not be included in the response.
        /// </summary>
        public string Bitfield { get; set; }
        /// <summary>
        /// Download speed of this download measured in bytes/sec.
        /// </summary>
        public int DownloadSpeed { get; set; }
        /// <summary>
        /// Upload speed of this download measured in bytes/sec.
        /// </summary>
        public int UploadSpeed { get; set; }
        /// <summary>
        /// InfoHash. BitTorrent only.
        /// </summary>
        public string InfoHash { get; set; }
        /// <summary>
        /// The number of seeders aria2 has connected to. BitTorrent only.
        /// </summary>
        public int NumSeeders { get; set; }
        /// <summary>
        /// true if the local endpoint is a seeder. Otherwise false. BitTorrent only.
        /// </summary>
        public bool Seeder { get; set; }
        /// <summary>
        /// Piece length in bytes.
        /// </summary>
        public long PieceLength { get; set; }
        /// <summary>
        /// The number of pieces.
        /// </summary>
        public int NumPieces { get; set; }
        /// <summary>
        /// The number of peers/servers aria2 has connected to.
        /// </summary>
        public int Connections { get; set; }
        /// <summary>
        /// The code of the last error for this item, if any. 
        /// The value is a string. 
        /// The error codes are defined in the EXIT STATUS section. 
        /// This value is only available for stopped/completed downloads.
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// The (hopefully) human readable error message associated to errorCode.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// List of GIDs which are generated as the result of this download. 
        /// For example, when aria2 downloads a Metalink file, it generates downloads described in the Metalink (see the --follow-metalink option). 
        /// This value is useful to track auto-generated downloads. 
        /// If there are no such downloads, this key will not be included in the response.
        /// </summary>
        public ICollection<string> FollowedBy { get; set; }
        /// <summary>
        /// The reverse link for followedBy. 
        /// A download included in followedBy has this object's GID in its following value.
        /// </summary>
        public string Following { get; set; }
        /// <summary>
        /// GID of a parent download. 
        /// Some downloads are a part of another download. 
        /// For example, if a file in a Metalink has BitTorrent resources, the downloads of ".torrent" files are parts of that parent. 
        /// If this download has no parent, this key will not be included in the response.
        /// </summary>
        public string BelongsTo { get; set; }
        /// <summary>
        /// Directory to save files.
        /// </summary>
        public string Dir { get; set; }
        /// <summary>
        /// Returns the list of files. 
        /// The elements of this list are the same structs used in aria2.getFiles() method.
        /// </summary>
        public ICollection<Aria2File> Files { get; set; }
        /// <summary>
        /// The number of verified number of bytes while the files are being hash checked. 
        /// This key exists only when this download is being hash checked.
        /// </summary>
        public long? VerifiedLength { get; set; }
        /// <summary>
        /// true if this download is waiting for the hash check in a queue. 
        /// This key exists only when this download is in the queue.
        /// </summary>
        public bool? VerifyIntegrityPending { get; set; }

        /// <summary>
        /// Struct which contains information retrieved from the .torrent (file).
        /// BitTorrent only. It contains following keys.
        /// </summary>
        public Aria2Bittorrent Bittorrent { get; set; }
    }
}
