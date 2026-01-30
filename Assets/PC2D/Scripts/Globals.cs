namespace PC2D
{
    public class Input
    {

        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
        public const string JUMP = "Jump";
        public const string FIRE = "Fire1";
        public const string BUILD = "Fire2";
    }

    public class Globals
    {
        // Input threshold in order to take effect. Arbitarily set.
        public const float INPUT_THRESHOLD = 0.02f;
        public const float FAST_FALL_THRESHOLD = 0.5f;
    }
}
