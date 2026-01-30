using System;
using MemoFramework;

namespace GGJ2026
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