using System;
using DG.Tweening;
using Game.Behaviours.Character;
using Game.Behaviours.Interactable;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CollectableDisplayPanel : MonoBehaviour
	{
		[SerializeField] private Image _image; 
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private GameObject _collectableShowPanel;
		[SerializeField] private GameObject _interactionInputInfoPanel;
		
		private bool    _isVisible;
		private Vector3 _mousePressPosition;

		private void Awake()
		{
			Collectable.OnCollected                                += Collectable_OnCollected;
			CharacterInteractionBehaviour.OnInteractionAvailable   += CharacterInteractionBehaviour_OnInteractionAvailable;
			CharacterInteractionBehaviour.OnInteractionUnavailable += CharacterInteractionBehaviour_OnInteractionUnavailable;

			_collectableShowPanel.SetActive(false);
			_interactionInputInfoPanel.SetActive(false);
		}
		
		private void OnDestroy()
		{
			Collectable.OnCollected                                -= Collectable_OnCollected;
			CharacterInteractionBehaviour.OnInteractionAvailable   -= CharacterInteractionBehaviour_OnInteractionAvailable;
			CharacterInteractionBehaviour.OnInteractionUnavailable -= CharacterInteractionBehaviour_OnInteractionUnavailable;
		}

		private void Update()
		{
			if (!_isVisible) return; 
			
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Hide();
			}
		}

		public void Show(Sprite sprite)
		{
			if (_isVisible) return;
			_isVisible = true;

			_collectableShowPanel.SetActive(true);
			
			_image.sprite      = sprite;
			_canvasGroup.alpha = 0f;
			_canvasGroup.DOFade(1f, 0.3f);
		}

		public void Hide()
		{
			if (!_isVisible) return;
			
			_canvasGroup.alpha = 0f;
			_canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
             {
                 _isVisible           = false;
                 _collectableShowPanel.SetActive(false);
             });
		}
		
		private void Collectable_OnCollected(Collectable collectable)
		{
			Show(collectable.Sprite);
		}
		
		private void CharacterInteractionBehaviour_OnInteractionAvailable()
		{
			_interactionInputInfoPanel.SetActive(true);
		}

		private void CharacterInteractionBehaviour_OnInteractionUnavailable()
		{
			_interactionInputInfoPanel.SetActive(false);
		}
	}
}