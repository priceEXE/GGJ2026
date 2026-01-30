using MemoFramework;

namespace GGJ2026
{
    public class OnRequireGameOver : MFEventArgs
    {
        public static OnRequireGameOver Create()
        {
            OnRequireGameOver args = MFRefPool.Acquire<OnRequireGameOver>();
            return args;
        }
        public override void Clear()
        {
            
        }
    }
}