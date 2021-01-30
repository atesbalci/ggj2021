using System;
using Game.Behaviours.Interactable;
using UnityEngine;

namespace Game.Controllers
{
	public class LevelFinishController : MonoBehaviour
	{
		[SerializeField] private int _requiredCollectableCount;

		private int _currentCollectableCount;

		private void Awake()
		{
			_currentCollectableCount = 0;
			
			Collectable.OnCollected += Collectable_OnCollected;
		}
		
		private void OnDestroy()
		{
			Collectable.OnCollected -= Collectable_OnCollected;
		}

		private void FinishLevel()
		{
			
		}
		
		private void Collectable_OnCollected(Collectable collectable)
		{
			_currentCollectableCount++;

			if (_currentCollectableCount >= _requiredCollectableCount)
			{
				FinishLevel();
			}
		}
	}
}