using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public static class IAria2ClientExtensions
    {
        /// <summary>
        /// This method adds a new download. 
        /// uris is an array of HTTP/FTP/SFTP/BitTorrent URIs (strings) pointing to the same resource.
        /// If you mix URIs pointing to different resources, then the download may fail or be corrupted without aria2 complaining. 
        /// When adding BitTorrent Magnet URIs, uris must have only one element and it should be BitTorrent Magnet URI. 
        /// options is a struct and its members are pairs of option name and value. 
        /// See Options below for more details. 
        /// If position is given, it must be an integer starting from 0. 
        /// The new download will be inserted at position in the waiting queue. 
        /// If position is omitted or position is larger than the current size of the queue, the new download is appended to the end of the queue.
        /// This method returns the GID of the newly registered download.
        /// </summary>
        /// <param name="uris"></param>
        /// <param name="options"></param>
        /// <param name="position"></param>
        public static Aria2Request<string> CreateAddUri(this IAria2Client client, ICollection<string> uris, Aria2InputFileOption options = null, int? position = null)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.addUri";
            request.AddParams(uris, options, position);
            return request;
        }

        /// <summary>
        /// This method adds a BitTorrent download by uploading a ".torrent" file. 
        /// If you want to add a BitTorrent Magnet URI, use the aria2.addUri() method instead. 
        /// torrent must be a base64-encoded string containing the contents of the ".torrent" file. 
        /// uris is an array of URIs (string). uris is used for Web-seeding.
        /// For single file torrents, the URI can be a complete URI pointing to the resource; 
        /// if URI ends with /, name in torrent file is added. 
        /// For multi-file torrents, name and path in torrent are added to form a URI for each file. 
        /// options is a struct and its members are pairs of option name and value. 
        /// See Options below for more details. 
        /// If position is given, it must be an integer starting from 0. 
        /// The new download will be inserted at position in the waiting queue.
        /// If position is omitted or position is larger than the current size of the queue, the new download is appended to the end of the queue. 
        /// This method returns the GID of the newly registered download.
        /// If --rpc-save-upload-metadata is true, the uploaded data is saved as a file named as the hex string of SHA-1 hash of data plus ".torrent" in the directory specified by --dir option. 
        /// E.g. a file name might be 0a3893293e27ac0490424c06de4d09242215f0a6.torrent. 
        /// If a file with the same name already exists, it is overwritten! If the file cannot be saved successfully or --rpc-save-upload-metadata is false, the downloads added by this method are not saved by --save-session.
        /// The following examples add local file file.torrent.
        /// </summary>
        /// <param name="base64Torrent"></param>
        /// <param name="uris"></param>
        /// <param name="options"></param>
        /// <param name="position"></param>
        public static Aria2Request<string> CreateAddTorrent(this IAria2Client client, string base64Torrent, ICollection<string> uris = null, Aria2InputFileOption options = null, int? position = null)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.addTorrent";
            request.AddParams(base64Torrent, uris, options, position);
            return request;
        }

        /// <summary>
        /// This method adds a Metalink download by uploading a ".metalink" file. 
        /// metalink is a base64-encoded string which contains the contents of the ".metalink" file. 
        /// options is a struct and its members are pairs of option name and value. 
        /// See Options below for more details. 
        /// If position is given, it must be an integer starting from 0. 
        /// The new download will be inserted at position in the waiting queue.
        /// If position is omitted or position is larger than the current size of the queue, the new download is appended to the end of the queue. 
        /// This method returns an array of GIDs of newly registered downloads. 
        /// If --rpc-save-upload-metadata is true, the uploaded data is saved as a file named hex string of SHA-1 hash of data plus ".metalink" in the directory specified by --dir option.
        /// E.g. a file name might be 0a3893293e27ac0490424c06de4d09242215f0a6.metalink. 
        /// If a file with the same name already exists, it is overwritten! 
        /// If the file cannot be saved successfully or --rpc-save-upload-metadata is false, the downloads added by this method are not saved by --save-session.
        /// </summary>
        /// <param name="base64Metalink"></param>
        /// <param name="options"></param>
        /// <param name="position"></param>
        public static Aria2Request<ICollection<string>> CreateAddMetalink(this IAria2Client client, string base64Metalink, Aria2InputFileOption options = null, int? position = null)
        {
            var request = new Aria2Request<ICollection<string>>(client);
            request.Method = "aria2.addMetalink";
            request.AddParams(base64Metalink, options, position);
            return request;
        }

        /// <summary>
        /// This method removes the download denoted by gid (string). 
        /// If the specified download is in progress, it is first stopped. 
        /// The status of the removed download becomes removed. 
        /// This method returns GID of removed download.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<string> CreateRemove(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.remove";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method removes the download denoted by gid. 
        /// This method behaves just like aria2.remove() except that this method removes the download without performing any actions which take time, such as contacting BitTorrent trackers to unregister the download first.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<string> CreateForceRemove(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.forceRemove";
            request.AddParams(gid);

            return request;
        }

        /// <summary>
        /// This method pauses the download denoted by gid (string). 
        /// The status of paused download becomes paused. 
        /// If the download was active, the download is placed in the front of waiting queue.
        /// While the status is paused, the download is not started.
        /// To change status to waiting, use the aria2.unpause() method.
        /// This method returns GID of paused download.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<string> CreatePause(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.pause";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method is equal to calling aria2.pause() for every active/waiting download.
        /// This methods returns OK.
        /// </summary>
        public static Aria2Request<string> CreatePauseAll(this IAria2Client client)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.pauseAll";
            return request;
        }

        /// <summary>
        /// This method pauses the download denoted by gid. 
        /// This method behaves just like aria2.pause() except that this method pauses downloads without performing any actions which take time, such as contacting BitTorrent trackers to unregister the download first.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<string> CreateForcePause(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.forcePause";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method is equal to calling aria2.forcePause() for every active/waiting download.
        /// This methods returns OK.
        /// </summary>
        public static Aria2Request<string> CreateForcePauseAll(this IAria2Client client)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.forcePauseAll";
            return request;
        }

        /// <summary>
        /// This method changes the status of the download denoted by gid (string) from paused to waiting, making the download eligible to be restarted. 
        /// This method returns the GID of the unpaused download.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<string> CreateUnpause(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.unpause";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method is equal to calling aria2.unpause() for every paused download. 
        /// This methods returns OK.
        /// </summary>
        public static Aria2Request<string> CreateUnpauseAll(this IAria2Client client)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.unpauseAll";
            return request;
        }

        /// <summary>
        /// This method returns the progress of the download denoted by gid (string). 
        /// keys is an array of strings. 
        /// If specified, the response contains only keys in the keys array. 
        /// If keys is empty or omitted, the response contains all keys. 
        /// This is useful when you just want specific keys and avoid unnecessary transfers. 
        /// For example, aria2.tellStatus("2089b05ecca3d829", ["gid", "status"]) returns the gid and status keys only. 
        /// The response is a struct and contains following keys. Values are strings.
        /// </summary>
        public static Aria2Request<Aria2Status> CreateTellStatus(this IAria2Client client, string gid, ICollection<string> keys = null)
        {
            var request = new Aria2Request<Aria2Status>(client);
            request.Method = "aria2.tellStatus";
            request.AddParams(gid, keys);
            return request;
        }

        /// <summary>
        /// This method returns the URIs used in the download denoted by gid (string). 
        /// The response is an array of structs and it contains following keys. 
        /// Values are string.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<ICollection<Aria2Uri>> CreateGetUris(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<ICollection<Aria2Uri>>(client);
            request.Method = "aria2.getUris";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method returns the file list of the download denoted by gid (string). 
        /// The response is an array of structs which contain following keys. Values are strings.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<ICollection<Aria2File>> CreateGetFiles(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<ICollection<Aria2File>>(client);
            request.Method = "aria2.getFiles";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method returns a list peers of the download denoted by gid (string). 
        /// This method is for BitTorrent only. The response is an array of structs and contains the following keys. 
        /// Values are strings.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<ICollection<Aria2Peer>> CreateGetPeers(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<ICollection<Aria2Peer>>(client);
            request.Method = "aria2.getPeers";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method returns currently connected HTTP(S)/FTP/SFTP servers of the download denoted by gid (string). 
        /// The response is an array of structs and contains the following keys. 
        /// Values are strings.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<ICollection<Aria2Server>> CreateGetServers(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<ICollection<Aria2Server>>(client);
            request.Method = "aria2.getServers";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method returns a list of active downloads. 
        /// The response is an array of the same structs as returned by the aria2.tellStatus() method. 
        /// For the keys parameter, please refer to the aria2.tellStatus() method.
        /// </summary>
        /// <param name="keys"></param>
        public static Aria2Request<ICollection<Aria2Status>> CreateTellActive(this IAria2Client client, ICollection<string> keys = null)
        {
            var request = new Aria2Request<ICollection<Aria2Status>>(client);
            request.Method = "aria2.tellActive";
            request.AddParams(keys);
            return request;
        }

        /// <summary>
        /// This method returns a list of waiting downloads, including paused ones. 
        /// offCreate is an integer and specifies the offCreate from the download waiting at the front. 
        /// num is an integer and specifies the max. number of downloads to be returned. 
        /// For the keys parameter, please refer to the aria2.tellStatus() method.
        /// If offCreate is a positive integer, this method returns downloads in the range of [offCreate, offCreate + num).
        /// offCreate can be a negative integer.
        /// offCreate == -1 points last download in the waiting queue and offCreate == -2 points the download before the last download, and so on.Downloads in the response are in reversed order then.
        /// For example, imagine three downloads "A","B" and "C" are waiting in this order.aria2.tellWaiting(0, 1) returns["A"]. 
        /// aria2.tellWaiting(1, 2) returns["B", "C"]. aria2.tellWaiting(-1, 2) returns["C", "B"].
        /// The response is an array of the same structs as returned by aria2.tellStatus() method.
        /// </summary>
        /// <param name="offCreate"></param>
        /// <param name="num"></param>
        /// <param name="keys"></param>
        public static Aria2Request<ICollection<Aria2Status>> CreateTellWaiting(this IAria2Client client, int offCreate, int num, ICollection<string> keys = null)
        {
            var request = new Aria2Request<ICollection<Aria2Status>>(client);
            request.Method = "aria2.tellWaiting";
            request.AddParams(offCreate, num, keys);
            return request;
        }

        /// <summary>
        /// This method returns a list of stopped downloads. offCreate is an integer and specifies the offCreate from the least recently stopped download. 
        /// num is an integer and specifies the max. number of downloads to be returned. 
        /// For the keys parameter, please refer to the aria2.tellStatus() method.
        /// offCreate and num have the same semantics as described in the aria2.tellWaiting() method.
        /// The response is an array of the same structs as returned by the aria2.tellStatus() method.
        /// </summary>
        /// <param name="offCreate"></param>
        /// <param name="num"></param>
        /// <param name="keys"></param>
        public static Aria2Request<ICollection<Aria2Status>> CreateTellStopped(this IAria2Client client, int offCreate, int num, ICollection<string> keys = null)
        {
            var request = new Aria2Request<ICollection<Aria2Status>>(client);
            request.Method = "aria2.tellStopped";
            request.AddParams(offCreate, num, keys);
            return request;
        }

        /// <summary>
        /// This method changes the position of the download denoted by gid in the queue. 
        /// pos is an integer. 
        /// how is a string. 
        /// If how is POS_Create, it moves the download to a position relative to the beginning of the queue. 
        /// If how is POS_CUR, it moves the download to a position relative to the current position. 
        /// If how is POS_END, it moves the download to a position relative to the end of the queue. 
        /// If the destination position is less than 0 or beyond the end of the queue, it moves the download to the beginning or the end of the queue respectively. 
        /// The response is an integer denoting the resulting position.
        /// For example, if GID#2089b05ecca3d829 is currently in position 3, aria2.changePosition('2089b05ecca3d829', -1, 'POS_CUR') will change its position to 2. 
        /// Additionally aria2.changePosition('2089b05ecca3d829', 0, 'POS_Create') will change its position to 0 (the beginning of the queue).
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="pos"></param>
        /// <param name="how"></param>
        public static Aria2Request<int> CreateChangePosition(this IAria2Client client, string gid, int pos, Aria2PosHow how)
        {
            var request = new Aria2Request<int>(client);
            request.Method = "aria2.changePosition";
            request.AddParams(gid, pos, how.ToString());
            return request;
        }

        /// <summary>
        /// This method removes the URIs in delUris from and appends the URIs in addUris to download denoted by gid. 
        /// delUris and addUris are lists of strings. 
        /// A download can contain multiple files and URIs are attached to each file. 
        /// fileIndex is used to select which file to remove/attach given URIs. 
        /// fileIndex is 1-based.
        /// position is used to specify where URIs are inserted in the existing waiting URI list. 
        /// position is 0-based. 
        /// When position is omitted, URIs are appended to the back of the list. 
        /// This method first executes the removal and then the addition. 
        /// position is the position after URIs are removed, not the position when this method is called. 
        /// When removing an URI, if the same URIs exist in download, only one of them is removed for each URI in delUris. 
        /// In other words, if there are three URIs http://example.org/aria2 and you want remove them all, you have to specify (at least) 3 http://example.org/aria2 in delUris. 
        /// This method returns a list which contains two integers. 
        /// The first integer is the number of URIs deleted. The second integer is the number of URIs added.
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="fileIndex"></param>
        /// <param name="delUris"></param>
        /// <param name="addUris"></param>
        /// <param name="position"></param>
        public static Aria2Request<int[]> CreateChangeUri(this IAria2Client client, string gid, int fileIndex, ICollection<string> delUris, ICollection<string> addUris, int? position)
        {
            var request = new Aria2Request<int[]>(client);
            request.Method = "aria2.changeUri";
            request.AddParams(gid, fileIndex, delUris, addUris, position);
            return request;
        }

        /// <summary>
        /// This method returns options of the download denoted by gid. 
        /// The response is a struct where keys are the names of options. 
        /// The values are strings. 
        /// Note that this method does not return options which have no default value and have not been Create on the command-line, in configuration files or RPC methods.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<Aria2InputFileOption> CreateGetOption(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<Aria2InputFileOption>(client);
            request.Method = "aria2.getOption";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method changes options of the download denoted by gid (string) dynamically. options is a struct. The options listed in Input File subsection are available, except for following options:
        /// 
        /// - dry-run
        /// - metalink-base-uri    
        /// - parameterized-uri
        /// - pause
        /// - piece-length
        /// - rpc-save-upload-metadata
        /// 
        /// Except for the following options, changing the other options of active download makes it restart(restart itself is managed by aria2, and no user intervention is required):
        /// 
        /// - bt-max-peers
        /// - bt-request-peer-speed-limit
        /// - bt-remove-unselected-file
        /// - force-save
        /// - max-download-limit
        /// - max-upload-limit
        /// 
        /// This method returns OK for success.
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="options"></param>
        public static Aria2Request<string> CreateChangeOption(this IAria2Client client, string gid, Aria2InputFileOption options)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.changeOption";
            request.AddParams(gid, options);
            return request;
        }

        /// <summary>
        /// This method returns the global options. 
        /// The response is a struct. 
        /// Its keys are the names of options. 
        /// Values are strings. 
        /// Note that this method does not return options which have no default value and have not been Create on the command-line, in configuration files or RPC methods. 
        /// Because global options are used as a template for the options of newly added downloads, the response contains keys returned by the aria2.getOption() method.
        /// </summary>
        public static Aria2Request<Aria2GlobalOption> CreateGetGlobalOption(this IAria2Client client)
        {
            var request = new Aria2Request<Aria2GlobalOption>(client);
            request.Method = "getGlobalOption";
            return request;
        }

        /// <summary>
        /// This method changes global options dynamically. options is a struct. The following options are available:
        /// 
        /// - bt-max-open-files
        /// - download-result
        /// - keep-unfinished-download-result
        /// - log
        /// - log-level
        /// - max-concurrent-downloads
        /// - max-download-result
        /// - max-overall-download-limit
        /// - max-overall-upload-limit
        /// - optimize-concurrent-downloads
        /// - save-cookies
        /// - save-session
        /// - server-stat-of
        ///
        /// In addition, options listed in the Input File subsection are available, except for following options: 
        /// - checksum
        /// - index-out
        /// - out
        /// - pause
        /// - select-file.
        ///
        /// With the log option, you can dynamically start logging or change log file.To stop logging, specify an empty string("") as the parameter value.Note that log file is always opened in append mode.This method returns OK for success.
        /// </summary>
        public static Aria2Request<string> CreateChangeGlobalOption(this IAria2Client client, Aria2GlobalOption options)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.changeGlobalOption";
            request.AddParams(options);
            return request;
        }

        /// <summary>
        /// This method returns global statistics such as the overall download and upload speeds. 
        /// The response is a struct and contains the following keys. Values are strings.
        /// <see cref="Aria2GlobalStat"/>
        /// </summary>
        public static Aria2Request<Aria2GlobalStat> CreateGetGlobalStat(this IAria2Client client)
        {
            var request = new Aria2Request<Aria2GlobalStat>(client);
            request.Method = "aria2.getGlobalStat";
            return request;
        }

        /// <summary>
        /// This method purges completed/error/removed downloads to free memory. This method returns OK.
        /// </summary>
        public static Aria2Request<string> CreatePurgeDownloadResult(this IAria2Client client)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.purgeDownloadResult";
            return request;
        }

        /// <summary>
        /// This method removes a completed/error/removed download denoted by gid from memory. 
        /// This method returns OK for success.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<string> CreateRemoveDownloadResult(this IAria2Client client, string gid)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.removeDownloadResult";
            request.AddParams(gid);
            return request;
        }

        /// <summary>
        /// This method returns the version of aria2 and the list of enabled features. 
        /// The response is a struct and contains following keys.
        /// - version
        /// Version number of aria2 as a string.
        /// - enabledFeatures
        /// List of enabled features.Each feature is given as a string.
        /// </summary>
        /// <param name="gid"></param>
        public static Aria2Request<Aria2VersionInfo> CreateGetVersion(this IAria2Client client)
        {
            var request = new Aria2Request<Aria2VersionInfo>(client);
            request.Method = "aria2.getVersion";
            return request;
        }

        /// <summary>
        /// This method returns session information. The response is a struct and contains following key.
        /// - sessionId
        /// Session ID, which is generated each time when aria2 is invoked.
        /// </summary>
        public static Aria2Request<Aria2SessionInfo> CreateGetSessionInfo(this IAria2Client client)
        {
            var request = new Aria2Request<Aria2SessionInfo>(client);
            request.Method = "aria2.getSessionInfo";
            return request;
        }

        /// <summary>
        /// This method shuts down aria2. This method returns OK.
        /// </summary>
        public static Aria2Request<string> CreateShutdown(this IAria2Client client)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.shutdown";
            return request;
        }

        /// <summary>
        /// This method shuts down aria2(). 
        /// This method behaves like :func:'aria2.shutdown` without performing any actions which take time, such as contacting BitTorrent trackers to unregister downloads first.
        /// This method returns OK.
        /// </summary>
        public static Aria2Request<string> CreateForceShutdown(this IAria2Client client)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.forceShutdown";
            return request;
        }

        /// <summary>
        /// This method saves the current session to a file specified by the --save-session option. 
        /// This method returns OK if it succeeds.
        /// </summary>
        /// <param name="request"></param>
        public static Aria2Request<string> CreateSaveSession(this IAria2Client client)
        {
            var request = new Aria2Request<string>(client);
            request.Method = "aria2.saveSession";
            return request;
        }

        /// <summary>
        /// This methods encapsulates multiple method calls in a single request. 
        /// methods is an array of structs. 
        /// The structs contain two keys: methodName and params. 
        /// methodName is the method name to call and params is array containing parameters to the method call. 
        /// This method returns an array of responses. 
        /// The elements will be either a one-item array containing the return value of the method call or a struct of fault element if an encapsulated method call fails.
        /// </summary>
        public static Aria2Request<ICollection<string>> CreateMulticall(this IAria2Client client, ICollection<Aria2MulticallMethod> methods)
        {
            var request = new Aria2Request<ICollection<string>> (client);
            request.Method = "system.multicall";
            request.ClearParams();
            request.AddParams(methods);
            return request;
        }

        /// <summary>
        /// This method returns all the available RPC methods in an array of string. 
        /// Unlike other methods, this method does not require secret token. 
        /// This is safe because this method just returns the available method names.
        /// </summary>
        public static Aria2Request<ICollection<string>> CreateListMethods(this IAria2Client client)
        {
            var request = new Aria2Request<ICollection<string>>(client);
            request.Method = "system.listMethods";
            request.ClearParams();
            return request;
        }

        /// <summary>
        /// This method returns all the available RPC notifications in an array of string. 
        /// Unlike other methods, this method does not require secret token. 
        /// This is safe because this method just returns the available notifications names.
        /// </summary>
        /// <param name="request"></param>
        public static Aria2Request<ICollection<string>> CreateListNotifications(this IAria2Client client)
        {
            var request = new Aria2Request<ICollection<string>>(client);
            request.Method = "system.listNotifications";
            request.ClearParams();
            return request;
        }
    }
}
