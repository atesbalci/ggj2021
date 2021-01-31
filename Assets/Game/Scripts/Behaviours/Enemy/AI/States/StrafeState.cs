using System;
using System.Collections.Generic;
using Game.Behaviours.Enemy.Movement;
using Game.Behaviours.Enemy.Sensor;
using Game.Helpers;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class StrafeState : BaseState
	{
		private readonly Transform         _owner;
		private readonly Transform         _player;
		private readonly MovementBehaviour _movementBehaviour;
		private readonly VisionSensor      _visionSensor;
		
		private int           _currentWaypointIndex;
		private float         _currentWaypointSetDistance;
		private List<Vector3> _relativeWaypoints;

		public StrafeState(
			Transform owner,
			Transform player,
			MovementBehaviour movementBehaviour,
			VisionSensor visionSensor)
		{
			_owner             = owner;
			_player            = player;
			_movementBehaviour = movementBehaviour;
			_visionSensor      = visionSensor;
		}
		
		public override void Enter()
		{
			base.Enter();

			CreateWaypoints();
			SetWaypoint(0);
		}

		public override void Exit()
		{
			base.Exit();
			
		}

		public override Type Tick()
		{
			float distanceToWaypoint = Vector3.Distance(_owner.position, _player.position + _relativeWaypoints[_currentWaypointIndex]);
			
			if (distanceToWaypoint < 5f)
			{
				SetNextWaypoint();
			}

			if (Mathf.Abs(distanceToWaypoint - _currentWaypointSetDistance)  > 10f)
			{
				_owner.transform.position += (_player.position - _owner.position).normalized * 10f;
				SetNextWaypoint();
			}

			if (_visionSensor.IsPlayerInSight())
			{
				return typeof(ChaseState);
			}
			
			return GetType();
		}

		private void CreateWaypoints()
		{
			_relativeWaypoints = new List<Vector3>();
			
			for (int i = 0; i < 4; i++)
			{
				Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * 15f;
				_relativeWaypoints.Add(new Vector3(randomPoint.x, 0f, randomPoint.y));
			}

			for (int i = 0; i < _relativeWaypoints.Count; i++)
			{
				GizmosHelper.DrawSphere(_player.position + _relativeWaypoints[i], 1f, Color.red, 4f);
			}
		}

		private void SetWaypoint(int index)
		{
			_currentWaypointIndex = index;
			_movementBehaviour.SetDestination(_player.position + _relativeWaypoints[_currentWaypointIndex], AiSettings.WALK_SPEED);
			_currentWaypointSetDistance = Vector3.Distance(_owner.position, _player.position + _relativeWaypoints[_currentWaypointIndex]);
			
			GizmosHelper.DrawSphere(_player.position + _relativeWaypoints[_currentWaypointIndex], 1f, Color.green, 5f);
		}

		private void SetNextWaypoint()
		{
			SetWaypoint((_currentWaypointIndex + 1) % _relativeWaypoints.Count);
		}
	}
}