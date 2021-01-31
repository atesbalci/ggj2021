using System;
using Game.Behaviours.Enemy.Movement;
using Game.Behaviours.Enemy.Sensor;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class ChaseState : BaseState
	{
		public static event Action OnEnter;
		public static event Action OnExit;
		
		private readonly Transform         _player;
		private readonly MovementBehaviour _movementBehaviour;
		private readonly VisionSensor      _visionSensor;

		private float _stateEnterTime;
		
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

			_stateEnterTime = Time.time;
			
			OnEnter?.Invoke();
		}

		public override void Exit()
		{
			base.Exit();
			
			OnExit?.Invoke();
		}

		public override Type Tick()
		{
			_movementBehaviour.SetDestination(_player.position, AiSettings.RUN_SPEED);

			if (_visionSensor.IsPlayerInCatchDistance())
			{
				return typeof(CatchState);
			}

			if (Time.time - _stateEnterTime > 7f)
			{
				return typeof(StrafeState);
			}
			
			return GetType();
		}
	}
}