using MemoFramework;
using MemoFramework.Extension;

namespace GGJ2026
{
    public class OnRequireEnterMenu : MFEventArgs
    {
        public static OnRequireEnterMenu Create()
        {
            OnRequireEnterMenu args = MFRefPool.Acquire<OnRequireEnterMenu>();
            return args;
        }
        public override void Clear()
        {
        }
    }
}