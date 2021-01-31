using Game.Behaviours.Boundaries;
using Game.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Behaviours.Game
{
    public class GameBehaviour : ITickable
    {
        public GameBehaviour(Transform playerTransform, GameStateData gameStateData, BoundsBehaviour boundsBehaviour)
        {
            boundsBehaviour.DistanceZoneChange += OnBoundsDistanceZoneChange;
            var spawnPoint = gameStateData.LastCheckpoint;
            if (spawnPoint.HasValue)
            {
                playerTransform.position = spawnPoint.Value;
            }
        }

        private void OnBoundsDistanceZoneChange(BoundDistanceZone zone)
        {
            if (zone == BoundDistanceZone.Death)
            {
                Restart();
            }
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}