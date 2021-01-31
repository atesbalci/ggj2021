using Game.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Behaviours.Game
{
    public class GameBehaviour : ITickable
    {
        public GameBehaviour(Transform playerTransform, GameStateData gameStateData)
        {
            var spawnPoint = gameStateData.LastCheckpoint;
            if (spawnPoint.HasValue)
            {
                playerTransform.position = spawnPoint.Value;
            }
        }
        
        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}