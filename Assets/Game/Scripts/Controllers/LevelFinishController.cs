using DG.Tweening;
using Game.Behaviours.ECS.Systems;
using Game.Behaviours.Enemy.AI;
using Game.Behaviours.Interactable;
using Game.Models;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
	public class LevelFinishController
	{
		public const int REQUIRED_COLLECTABLE_COUNT = 6;

		private readonly GameStateData _gameStateData;

		public LevelFinishController(GameStateData gameStateData)
		{
			_gameStateData = gameStateData;
			
			Collectable.OnCollected     += Collectable_OnCollected;
			AiBehaviour.OnPlayerCatched += AiBehaviour_OnPlayerCatched;
		}
		
		~LevelFinishController()
		{
			Collectable.OnCollected     -= Collectable_OnCollected;
			AiBehaviour.OnPlayerCatched -= AiBehaviour_OnPlayerCatched;
		}

		private void FinishLevel(bool isSuccessful)
		{
			if (isSuccessful)
			{
				
			}
			else
			{
				EntityManagement.ClearEntities();
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
		
		private void Collectable_OnCollected(Collectable collectable)
		{
			_gameStateData.PickCollectable();
			if (_gameStateData.GetPickedCollectableCount() >= REQUIRED_COLLECTABLE_COUNT)
			{
				FinishLevel(true);
			}
		}
		
		private void AiBehaviour_OnPlayerCatched()
		{
			DOVirtual.DelayedCall(4f, () => FinishLevel(false));
		}
	}
}
