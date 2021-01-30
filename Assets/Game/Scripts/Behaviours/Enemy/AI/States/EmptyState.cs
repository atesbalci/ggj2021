using System;
using Game.Behaviours.Enemy.Movement;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class EmptyState : BaseState
	{
		private readonly Transform         _owner;
		private readonly Transform         _player;
		private readonly MovementBehaviour _movement;

		public EmptyState(
			Transform owner,
			Transform player,
			MovementBehaviour movement)
		{
			_owner    = owner;
			_player   = player;
			_movement = movement;
		}
		
		public override void Enter()
		{
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
			_owner.position = -_player.forward * AiSettings.SPAWN_DISTANCE_TO_PLAYER;
			_owner.forward  = (_player.position - _owner.position).normalized;
			_movement.Activate();
		}
	}
}