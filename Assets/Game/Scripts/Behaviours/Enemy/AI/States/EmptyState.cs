using System;
using Game.Behaviours.Enemy.Movement;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class EmptyState : BaseState
	{
		private Transform         _target;
		private MovementBehaviour _movement;

		private Vector3 _initialPosition;
		
		public EmptyState(
			Transform target,
			MovementBehaviour movement)
		{
			_target   = target;
			_movement = movement;
		}
		
		public override void Enter()
		{
			_initialPosition = _target.position;
			_target.position   = _target.forward * 2000f;
			
			_movement.Deactivate();
		}

		public override Type Tick()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				return typeof(IdleState);
			}
			else
			{
				return GetType();
			}
		}

		public override void Exit()
		{
			_target.position = _initialPosition;
			
			_movement.Activate();
		}
	}
}