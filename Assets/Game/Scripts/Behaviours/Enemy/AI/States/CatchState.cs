using System;
using Game.Behaviours.Enemy.Animation;
using Game.Behaviours.Enemy.Movement;

namespace Game.Behaviours.Enemy.AI.States
{
	public class CatchState : BaseState
	{
		private readonly AnimationBehaviour _animationBehaviour;
		private readonly MovementBehaviour  _movementBehaviour;
		
		public CatchState(
			AnimationBehaviour animationBehaviour,
			MovementBehaviour movementBehaviour)
		{
			_animationBehaviour = animationBehaviour;
			_movementBehaviour  = movementBehaviour;
		}

		public override void Enter()
		{
			base.Enter();
			
			_movementBehaviour.Deactivate();
			_animationBehaviour.PlayCatch();
		}

		public override Type Tick()
		{
			return GetType();
		}
	}
}