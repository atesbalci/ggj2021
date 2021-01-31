using System;
using DG.Tweening;
using Game.Behaviours.Enemy.AI.States;
using Game.Behaviours.Enemy.Animation;
using Game.Behaviours.Enemy.Movement;
using Game.Helpers;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI
{
	public class IdleState : BaseState
	{
		private readonly MovementBehaviour  _movementBehaviour;
		private readonly AnimationBehaviour _animationBehaviour;

		private float _stateEnterTime;
		
		public IdleState(
			MovementBehaviour movementBehaviour,
			AnimationBehaviour animationBehaviour)
		{
			_movementBehaviour  = movementBehaviour;
			_animationBehaviour = animationBehaviour;
		}

		public override void Enter()
		{
			base.Enter();

			_stateEnterTime = Time.time;
			
			_animationBehaviour.PlayIdle();

			DOVirtual.DelayedCall(UnityEngine.Random.Range(3f, 5f), () =>
            {
				_animationBehaviour.PlayAgony();
            });
		}

		public override Type Tick()
		{
			if (Time.time - _stateEnterTime > 5f)
			{
				// Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				// if (Physics.Raycast(ray, out RaycastHit hit))
				// {
				// 	_movementBehaviour.SetDestination(hit.point, AiSettings.WALK_SPEED);
				// 	// return typeof(ChaseStage);
				// }

				return typeof(StrafeState);
			}
			
			return GetType();
		}
	}
}