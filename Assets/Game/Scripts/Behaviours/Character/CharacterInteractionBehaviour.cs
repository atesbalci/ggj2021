using System;
using Game.Behaviours.Interactable;
using UnityEngine;

namespace Game.Behaviours.Character
{
	public class CharacterInteractionBehaviour : MonoBehaviour
	{
		public static event Action OnInteractionAvailable;
		public static event Action OnInteractionUnavailable;
		
		[SerializeField] private LayerMask _interactableLayer;
		[SerializeField] private float _maxRayDistance = 5f;
		
		private Camera _mainCamera;

		private Ray        _ray;
		private RaycastHit _hit;

		private bool _isReadyToInteract;
		
		private void Awake()
		{
			_mainCamera = Camera.main;
		}
		
		private void Update()
		{
			_ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			if (Physics.Raycast(_ray, out _hit, _maxRayDistance, _interactableLayer))
			{
				if (Input.GetKeyDown(KeyCode.E))
				{
					_hit.collider.GetComponent<IInteractable>().Interact();
					
					_isReadyToInteract = false;
					OnInteractionUnavailable?.Invoke();
				}
				else if (!_isReadyToInteract)
				{
					_isReadyToInteract = true;
					OnInteractionAvailable?.Invoke();
				}
			}
			else
			{
				if (_isReadyToInteract)
				{
					_isReadyToInteract = false;
					OnInteractionUnavailable?.Invoke();
				}
			}
		}
	}
}