using System;
using MemoFramework;

namespace TapTap2025
{
    public class OnRequireEnterGame : MFEventArgs
    {
        public static OnRequireEnterGame Create()
        {
            OnRequireEnterGame args = MFRefPool.Acquire<OnRequireEnterGame>();
            return args;
        }
        public override void Clear()
        {
            
        }
    }
}