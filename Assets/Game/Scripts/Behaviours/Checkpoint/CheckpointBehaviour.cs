using System;
using Game.Models;
using UnityEngine;
using Zenject;

namespace Game.Behaviours.Checkpoint
{
    public class CheckpointBehaviour : MonoBehaviour
    {
        private GameStateData _gameStateData;
        
        [Inject]
        public void Initialize(GameStateData gameStateData)
        {
            _gameStateData = gameStateData;
        }

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var pos = transform.position;
                pos.y = other.transform.position.y;
                _gameStateData.ReachCheckPoint(1, pos);
                Debug.Log("Checkpoint!");
            }
        }
    }
}