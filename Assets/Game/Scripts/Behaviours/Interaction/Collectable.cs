using System;
using UnityEngine;

namespace Game.Behaviours.Interactable
{
	public class Collectable : MonoBehaviour, IInteractable
	{
		public static event Action<Collectable> OnCollected;

		public Sprite Sprite => _sprite;
		
		[SerializeField] private Sprite _sprite;
		
		public void Interact()
		{
			OnCollected?.Invoke(this);
			
			Destroy(gameObject);
		}
	}
}