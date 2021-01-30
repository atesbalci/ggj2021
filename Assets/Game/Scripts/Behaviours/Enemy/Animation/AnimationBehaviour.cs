using Game.Behaviours.Enemy.AI;
using UnityEngine;

namespace Game.Behaviours.Enemy.Animation
{
	public class AnimationBehaviour : MonoBehaviour
	{
		private static readonly int _idle  = Animator.StringToHash("Idle");
		private static readonly int _walk  = Animator.StringToHash("Walk");
		private static readonly int _speed = Animator.StringToHash("Speed");

		private Animator _animator;

		private bool _isInIdle;
		
		private void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
			
			PlayIdle();
		}

		public void PlayIdle()
		{
			_isInIdle = true;
			_animator.SetTrigger(_idle);
		}

		public void SetVelocity(Vector3 velocity)
		{
			if (_isInIdle)
			{
				_animator.SetTrigger(_walk);
				_isInIdle = false;
			}
			
			if (velocity.magnitude >= AiSettings.WALK_SPEED)
			{
				_animator.SetFloat(_speed, 1f);
			}
			else if (velocity.magnitude < AiSettings.RUN_SPEED || velocity.magnitude >= AiSettings.RUN_SPEED)
			{
				_animator.SetFloat(_speed, 2f);
			}
			else
			{
				_animator.SetFloat(_speed, 0f);
			}
		}
	}
}