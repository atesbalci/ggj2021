using System;

namespace Game.Behaviours.Enemy.AI.States
{
	public abstract class BaseStage
	{
		public virtual void Enter() {}
		public virtual void Exit()  {}

		public abstract Type Tick();
	}
}