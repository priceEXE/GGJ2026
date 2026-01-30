using MemoFramework;
using MemoFramework.Extension;

namespace GGJ2026
{
    public class OnRequireEnterTest : MFEventArgs
    {
        public static OnRequireEnterTest Create()
        {
            OnRequireEnterTest args = MFRefPool.Acquire<OnRequireEnterTest>();
            return args;
        }
        public override void Clear()
        {
        }
    }
}