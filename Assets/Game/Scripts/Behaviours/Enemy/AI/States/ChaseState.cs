using System;

namespace Game.Behaviours.Enemy.AI.States
{
	public class ChaseState : BaseState
	{
		public override Type Tick()
		{
			return GetType();
		}
	}
}