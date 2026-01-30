namespace TapTap2025
{
    public class ValueSystem
    {
        private static ValueSystem instance = null;
        public static ValueSystem Instance {get{return instance;}}
        
        private int HalfKey;
        private int WholeKey;

        public ValueSystem()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void GetHalfKey()
        {
            HalfKey += 1;
            if (HalfKey == 2)
            {
                HalfKey = 0;
                WholeKey += 1;
            }
        }

        public void GetWholeKey()
        {
            WholeKey += 1;
        }

        public bool ConsumeKey()
        {
            if (WholeKey == 0)
            {
                return false;
            }
            WholeKey -= 1;
            return true;
        }
    }
}