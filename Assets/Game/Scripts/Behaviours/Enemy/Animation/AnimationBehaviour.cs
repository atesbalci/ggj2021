using Game.Behaviours.Enemy.AI;
using UnityEngine;

namespace Game.Behaviours.Enemy.Animation
{
	public class AnimationBehaviour : MonoBehaviour
	{
		private static readonly int _idle  = Animator.StringToHash("Idle");
		private static readonly int _walk  = Animator.StringToHash("Walk");
		private static readonly int _scare = Animator.StringToHash("Scare");
		private static readonly int _agony = Animator.StringToHash("Agony");
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

		public void PlayScare()
		{
			_animator.SetTrigger(_scare);
		}

		public void PlayAgony()
		{
			_animator.SetTrigger(_agony);
		}
		
		public void SetSpeed(float speed)
		{
			if (_isInIdle)
			{
				_animator.SetTrigger(_walk);
				_isInIdle = false;
			}
			
			if(speed < float.Epsilon)
			{
				_animator.SetFloat(_speed, 0f);
				PlayIdle();
			}
			else if (speed <= AiSettings.WALK_SPEED)
			{
				_animator.SetFloat(_speed, 1f);
			}
			else if ( speed > AiSettings.WALK_SPEED || speed >= AiSettings.RUN_SPEED)
			{
				_animator.SetFloat(_speed, 2f);
			}
		}
	}
}