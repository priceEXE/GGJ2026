using MemoFramework;
using MemoFramework.Extension;

namespace GGJ2026
{
    public class OnRequireEnterEnd : MFEventArgs
    {
        public static OnRequireEnterEnd Create()
        {
            OnRequireEnterEnd args = MFRefPool.Acquire<OnRequireEnterEnd>();
            return args;
        }
        
        public override void Clear()
        {
        }
    }
}