using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Aria2
{
    public class Aria2GlobalOption : Aria2InputFileOption
    {
        public string Bt_max_open_files { get; set; }
        public string Download_result { get; set; }
        public string Keep_unfinished_download_result { get; set; }
        public string Log { get; set; }
        public string Log_level { get; set; }
        public string Max_concurrent_downloads { get; set; }
        public string Max_download_result { get; set; }
        public string Max_overall_download_limit { get; set; }
        public string Max_overall_upload_limit { get; set; }
        public string Optimize_concurrent_downloads { get; set; }
        public string Save_cookies { get; set; }
        public string Save_session { get; set; }
        public string Server_stat_of { get; set; }

        private new string Checksum { get; set; }
        private new string Index_out { get; set; }
        private new string Out { get; set; }
        private new string Pause { get; set; }
        private new string Select_file { get; set; }
    }
}
