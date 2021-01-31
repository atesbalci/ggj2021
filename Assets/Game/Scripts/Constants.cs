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
        public const float BoundDeathDistance = 1f;
        
        public const float EnemyFarDistance = 20f;
        public const float EnemyMidDistance = 15f;
        public const float EnemyCloseDistance = 10f;
        public const float EnemyDeathDistance = 1f;
        public const float EnemyFarClipInterval = 1f;
        public const float EnemyMidClipInterval = 0.5f;
        public const float EnemyCloseClipInterval = 0.25f;
    }
}
