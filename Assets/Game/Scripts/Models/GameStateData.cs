using UnityEngine;

namespace Game.Models
{
    public class GameStateData
    {
        private CheckPoint _lastReachedCheckpoint;

        public void ReachCheckPoint(int no, Vector3 spawnPoint)
        {
            _lastReachedCheckpoint = new CheckPoint(no, spawnPoint);
        }

        public Vector3? LastCheckpoint =>
            _lastReachedCheckpoint.No > 0 ? _lastReachedCheckpoint.SpawnPoint : (Vector3?) null;

        private struct CheckPoint
        {
            public readonly int No;
            public readonly Vector3 SpawnPoint;

            public CheckPoint(int no, Vector3 spawnPoint)
            {
                No = no;
                SpawnPoint = spawnPoint;
            }
        }
    }
}