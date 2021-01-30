using System;
using Game.Behaviours.Enemy.AI.States;
using Game.Behaviours.Enemy.Movement;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI
{
	public class IdleStage : BaseStage
	{
		private readonly MovementBehaviour _movement;
		
		public IdleStage(MovementBehaviour movement)
		{
			_movement = movement;
		}

		public override Type Tick()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out RaycastHit hit))
				{
					_movement.SetDestination(hit.point, AiSettings.WALK_SPEED);
					// return typeof(ChaseStage);
				}
			}
			
			return GetType();
		}
	}
}