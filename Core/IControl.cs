using lib_iis.Core.MVVM;
using System;

namespace lib_iis.Core
{
    public interface IControl
    {
        Action<BaseViewModel, string> Close { get; set; }
    }
}
