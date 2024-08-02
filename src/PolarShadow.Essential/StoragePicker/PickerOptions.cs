using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public class PickerOptions
    {
        public PickerOptions() { }
        public PickerOptions(PickerType pickerType)
        {
            this.PickerType = pickerType;
        }
        public PickerType PickerType { get; }
        /// <summary>
        /// dialog title
        /// </summary>
        public string Title { get; set; }
        public bool AllowMultiple { get; set; }
        public string DefaultLocation { get; set; }
        public IReadOnlyList<FilePickType> FileTypeFilter { get; set; }

    }
}
