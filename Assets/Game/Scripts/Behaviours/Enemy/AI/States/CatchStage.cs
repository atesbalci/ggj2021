using System;

namespace Game.Behaviours.Enemy.AI.States
{
	public class CatchStage : BaseStage
	{
		public override Type Tick()
		{
			return GetType();
		}
	}
}