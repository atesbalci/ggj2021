using System;
using Game.Behaviours.Enemy.Movement;
using Game.Behaviours.Enemy.Sensor;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class ChaseState : BaseState
	{
		private readonly Transform         _player;
		private readonly MovementBehaviour _movementBehaviour;
		private readonly VisionSensor      _visionSensor;
		
		public ChaseState(
			Transform player,
			MovementBehaviour movementBehaviour,
			VisionSensor visionSensor)
		{
			_player            = player;
			_movementBehaviour = movementBehaviour;
			_visionSensor      = visionSensor;
		}
		
		public override void Enter()
		{
			base.Enter();
		}

		public override Type Tick()
		{
			_movementBehaviour.SetDestination(_player.position, AiSettings.RUN_SPEED);

			if (_visionSensor.IsPlayerInCatchDistance())
			{
				return typeof(CatchState);
			}
			
			return GetType();
		}
	}
}