using Game.Behaviours.Enemy.AI;
using Game.Behaviours.Interactable;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
	public class LevelFinishController
	{
		private readonly int _requiredCollectableCount = 2;

		private int _currentCollectableCount;

		public LevelFinishController()
		{
			_currentCollectableCount = 0;
			
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
			_currentCollectableCount++;

			if (_currentCollectableCount >= _requiredCollectableCount)
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