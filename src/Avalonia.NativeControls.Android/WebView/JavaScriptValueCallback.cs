using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls.Android
{
    internal class JavaScriptValueCallback : Java.Lang.Object, IValueCallback
    {
        private Action<Java.Lang.Object> _receiveValueCallback;

        public JavaScriptValueCallback(Action<Java.Lang.Object> receiveValueCallback)
        {
            _receiveValueCallback = receiveValueCallback;
        }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            _receiveValueCallback?.Invoke(value);
        }
    }
}
