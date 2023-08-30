using PolarShadow.Core;
using PolarShadow.Navigations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class BookSourceDetailViewModel : ViewModelBase, IParameterObtain
    {
        private ISite _site;
        public ISite Site
        {
            get => _site;
            set => SetProperty(ref _site, value);
        }

        private string _selectedRequest;
        public string SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                SetProperty(_selectedRequest, value, newValue =>
                {
                    _selectedRequest = newValue;
                    OnReqeustSelected();
                });
            }
        }

        private string _formatedRequest;
        public string FormatedRequest
        {
            get => _formatedRequest;
            set => SetProperty(ref _formatedRequest, value);
        }

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if(parameters.TryGetValue(nameof(Site), out ISite value)) Site = value;
        }

        private void OnReqeustSelected()
        {
            FormatedRequest = Site.Requests[SelectedRequest].GetRequestJson(JsonOption.FormatWriteOption);
        }
    }
}
