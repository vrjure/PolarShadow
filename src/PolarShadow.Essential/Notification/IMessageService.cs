using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IMessageService
    {
        void Show(IMessage message);
        Task<MessageResult> ShowDialog(IMessage message, DialogType dialogType);
    }
}
