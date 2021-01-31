using System;
using DG.Tweening;
using Game.Behaviours.Enemy.Animation;
using Game.Behaviours.Enemy.Movement;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class CatchState : BaseState
	{
		private readonly Transform          _owner;
		private readonly Transform          _player;
		private readonly AnimationBehaviour _animationBehaviour;
		private readonly MovementBehaviour  _movementBehaviour;
		
		public CatchState(
			Transform owner,
			Transform player,
			AnimationBehaviour animationBehaviour,
			MovementBehaviour movementBehaviour)
		{
			_owner              = owner;
			_player             = player;
			_animationBehaviour = animationBehaviour;
			_movementBehaviour  = movementBehaviour;
		}

		public override void Enter()
		{
			base.Enter();
			
			_movementBehaviour.Deactivate();

			_animationBehaviour.PlayCatch();

			CharacterInputBehaviour characterInputBehaviour = _player.GetComponent<CharacterInputBehaviour>();
			characterInputBehaviour.enabled = false;

			Vector3 targetPosition = _player.position + _player.forward * 0.625f;
			targetPosition = new Vector3(targetPosition.x, 0f, targetPosition.z);
			
			float moveStartTime = Time.time;
			Vector3 startForward = _owner.forward;
			_owner.DOMove(targetPosition, 0.5f).OnUpdate(() =>
	         {
	             _owner.forward = Vector3.Lerp(startForward, -_player.forward, (Time.time - moveStartTime) / 0.5f);
	         }).OnComplete(() =>
			{
				_owner.GetComponentInChildren<Light>(true).gameObject.SetActive(true);
				_owner.forward = -_player.GetComponentInChildren<Camera>().transform.forward;
				_animationBehaviour.PlayAgony();
				DOVirtual.DelayedCall(4f, () => AiBehaviour.OnPlayerCatched?.Invoke());
			});
		}

		public override Type Tick()
		{
			// _owner.forward      = -_player.GetComponentInChildren<Camera>().transform.forward;
			// _owner.transform.up = Vector3.up;
			
			return GetType();
		}
	}
}