using MemoFramework;

namespace TapTap2025
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