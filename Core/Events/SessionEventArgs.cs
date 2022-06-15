using lib_iis.ViewModels;
using System;

namespace lib_iis.Core.Events
{
    internal class SessionEventArgs : EventArgs
    {
        public HomeViewModel HoveViewModelContext;
    }
}
