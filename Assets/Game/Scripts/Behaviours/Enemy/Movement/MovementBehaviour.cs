using System;
using Game.Behaviours.Enemy.AI;
using Game.Behaviours.Enemy.Animation;
using UnityEngine;

namespace Game.Behaviours.Enemy.Movement
{
	[RequireComponent(typeof(Rigidbody), typeof(AnimationBehaviour))]
	public class MovementBehaviour : MonoBehaviour
	{
		private Rigidbody _rigidbody;
		private Rigidbody Rigidbody
		{
			get
			{
				if (_rigidbody != null) return _rigidbody;
				_rigidbody = GetComponent<Rigidbody>();
				return _rigidbody;
			}
		}
		
		private AnimationBehaviour _animationBehaviour;
		private AnimationBehaviour AnimationBehaviour
		{
			get
			{
				if (_animationBehaviour != null) return _animationBehaviour;
				_animationBehaviour = GetComponent<AnimationBehaviour>();
				return _animationBehaviour;
			}
		}

		private bool    _hasTarget;
		private float   _targetSpeed;
		private Vector3 _targetPoint;

		private RigidbodyConstraints _previousContraints;
		
		private void Update()
		{
			if (_hasTarget)
			{
				Vector3 targetVelocity = (_targetPoint - transform.position).normalized * _targetSpeed;
				if (targetVelocity.magnitude * Time.deltaTime > Mathf.Abs((_targetPoint - transform.position).magnitude))
				{
					_hasTarget         = false;
					transform.position = _targetPoint;
					
					Rigidbody.angularVelocity = Vector3.zero;
					SetVelocity(Vector3.zero);

				}
				else
				{
					SetVelocity(targetVelocity);
				}
			}
		}

		public void Activate()
		{
			Rigidbody.constraints = _previousContraints;
		}

		public void Deactivate()
		{
			_previousContraints   = Rigidbody.constraints;
			Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		
		public void SetDestination(Vector3 targetPoint, float speed)
		{
			_hasTarget   = true;
			_targetPoint = targetPoint;
			_targetSpeed = speed;
			
			AnimationBehaviour.SetSpeed(_targetSpeed);
		}
		
		public void SetVelocity(Vector3 velocity)
		{
			Rigidbody.velocity = velocity;
			if (velocity.magnitude > float.Epsilon)
			{
				transform.forward = Vector3.Lerp(transform.forward, velocity.normalized, Time.deltaTime * AiSettings.TURN_SPEED);
			}
			else
			{
				AnimationBehaviour.SetSpeed(0f);	
			}
		}
	}
}