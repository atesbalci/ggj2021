using System;

namespace Game.Behaviours.Enemy.AI.States
{
	public class CatchState : BaseState
	{
		public override Type Tick()
		{
			return GetType();
		}
	}
}