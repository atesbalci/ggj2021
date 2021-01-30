using System;
using System.Collections.Generic;
using Game.Helpers;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI.States
{
	public class StrafeState : BaseState
	{
		private Vector3 _destinationPosition;

		private Transform _player;

		private List<Vector3> _waypoints;
		
		public override void Enter()
		{
			base.Enter();

			_player = GameObject.FindGameObjectWithTag("Player").transform;
			
			_waypoints = new List<Vector3>();
			
			for (int i = 0; i < 4; i++)
			{
				Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * 15f;
				Vector3 waypoint = _player.position + new Vector3(randomPoint.x, 0f, randomPoint.y);
				_waypoints.Add(waypoint);
			}

			for (int i = 0; i < _waypoints.Count; i++)
			{
				GizmosHelper.DrawSphere(_waypoints[i], 1f, Color.red, 4f);
			}
		}

		public override Type Tick()
		{
			
			
			return GetType();
		}
	}
}