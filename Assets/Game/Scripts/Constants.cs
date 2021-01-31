namespace Game
{
    public static class Constants
    {
        public const float MaxDist = 10f;
        public const float MinDist = 1f;
        public const float MaxDistSq = MaxDist * MaxDist;
        public const float GrassRadialGap = 7.5f;
        public const int PlayerGrassPoolSize = 1000;
        public const float GrassCloseThresholdSq = 1.25f;
        
        // Bounds
        public const float BoundFarDistance = 20f;
        public const float BoundMidDistance = 15f;
        public const float BoundCloseDistance = 10f;
        public const float BoundDeathDistance = 1f;
        public const float BoundFarClipInterval = 1f;
        public const float BoundMidClipInterval = 0.5f;
        public const float BoundCloseClipInterval = 0.25f;
    }
}
