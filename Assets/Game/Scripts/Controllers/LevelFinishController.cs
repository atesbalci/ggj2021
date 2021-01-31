using Game.Behaviours.Enemy.AI;
using Game.Behaviours.Interactable;
using Game.Models;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
	public class LevelFinishController
	{
		private const int RequiredCollectableCount = 2;

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
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
		
		private void Collectable_OnCollected(Collectable collectable)
		{
			_gameStateData.PickCollectable();
			if (_gameStateData.GetPickedCollectableCount() >= RequiredCollectableCount)
			{
				FinishLevel(true);
			}
		}
		
		private void AiBehaviour_OnPlayerCatched()
		{
			FinishLevel(false);
		}
	}
}