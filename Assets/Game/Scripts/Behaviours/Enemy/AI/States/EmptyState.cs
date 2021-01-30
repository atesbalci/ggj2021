using System;
using Game.Behaviours.Enemy.Movement;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class EmptyState : BaseState
	{
		private Transform         _owner;
		private MovementBehaviour _movement;

		private Vector3 _initialPosition;
		
		public EmptyState(
			Transform owner,
			MovementBehaviour movement)
		{
			_owner   = owner;
			_movement = movement;
		}
		
		public override void Enter()
		{
			_initialPosition = _owner.position;
			_owner.position   = _owner.forward * 2000f;
			
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
			Transform player = GameObject.FindGameObjectWithTag("Player").transform;
			_owner.position = -player.forward * AiSettings.SPAWN_DISTANCE_TO_PLAYER;
			_owner.forward  = (player.position - _owner.position).normalized;
			_movement.Activate();
		}
	}
}