using Game.Behaviours.Boundaries;
using Game.Behaviours.ECS.Systems;
using Game.Models;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Behaviours.Game
{
    public class GameBehaviour : ITickable
    {
        public GameBehaviour(Transform playerTransform, GameStateData gameStateData, BoundsBehaviour boundsBehaviour)
        {
            boundsBehaviour.Death += OnDeath;
            var spawnPoint = gameStateData.LastCheckpoint;
            if (spawnPoint.HasValue)
            {
                playerTransform.position = spawnPoint.Value;
            }
        }

        private void OnDeath()
        {
            Restart();
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
            EntityManagement.ClearEntities();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}