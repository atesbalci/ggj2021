using System;
using System.Collections.Generic;
using System.Linq;
using Game.Behaviours.Enemy.AI.States;
using Game.Behaviours.Enemy.Animation;
using Game.Behaviours.Enemy.Movement;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI
{
	[RequireComponent(typeof(MovementBehaviour), typeof(AnimationBehaviour))]
	public class AiBehaviour : MonoBehaviour
	{
		private MovementBehaviour  _movementBehaviour;
		private AnimationBehaviour _animationBehaviour;
		
		private BaseStage       _currentStage;
		private List<BaseStage> _stages; 
		
		private void Awake()
		{
			_movementBehaviour  = GetComponent<MovementBehaviour>();
			_animationBehaviour = GetComponent<AnimationBehaviour>();
			
			CreateStages();
		}

		private void Update()
		{
			Type stageType = _currentStage.Tick();
			
			if (stageType != _currentStage.GetType())
			{
				EnterStage(GetStage(stageType));	
			}
		}

		private void CreateStages()
		{
			_stages = new List<BaseStage>()
			{
				new EmptyStage(transform, _movementBehaviour),
				new IdleStage(_movementBehaviour),
				new StrafeStage(),
				new ChaseStage(),
				new CatchStage()
			};

			EnterStage(_stages.First());
		}

		private void EnterStage(BaseStage stage)
		{
			_currentStage?.Exit();
			
			_currentStage = stage;
			_currentStage.Enter();
			
			Debug.Log($"[AiBehaviour] => Entered: {_currentStage.GetType()}");
		}

		private BaseStage GetStage(Type type)
		{
			return _stages.FirstOrDefault(item => item.GetType() == type);
		}
	}
}