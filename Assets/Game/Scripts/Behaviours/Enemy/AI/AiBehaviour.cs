using System;
using System.Collections.Generic;
using System.Linq;
using Game.Behaviours.Enemy.AI.States;
using Game.Behaviours.Enemy.Animation;
using Game.Behaviours.Enemy.Movement;
using Game.Behaviours.Enemy.Sensor;
using UnityEngine;

namespace Game.Behaviours.Enemy.AI
{
	[RequireComponent(typeof(MovementBehaviour), typeof(AnimationBehaviour))]
	public class AiBehaviour : MonoBehaviour
	{
		public static Action OnPlayerCatched;
		
		private Transform          _player;
		private MovementBehaviour  _movementBehaviour;
		private AnimationBehaviour _animationBehaviour;
		private VisionSensor       _visionSensor;
		
		private BaseState       currentState;
		private List<BaseState> _states; 
		
		private void Awake()
		{
			_player             = GameObject.FindGameObjectWithTag(Tags.PLAYER).transform;
			_movementBehaviour  = GetComponent<MovementBehaviour>();
			_animationBehaviour = GetComponent<AnimationBehaviour>();
			_visionSensor       = GetComponent<VisionSensor>();
			
			CreateStages();
		}

		private void Update()
		{
			Type stageType = currentState.Tick();
			
			if (stageType != currentState.GetType())
			{
				EnterState(GetState(stageType));	
			}
		}

		private void CreateStages()
		{
			_states = new List<BaseState>()
			{
				new EmptyState(transform, _player, _movementBehaviour),
				new IdleState(_movementBehaviour, _animationBehaviour),
				new StrafeState(transform, _player, _movementBehaviour, _visionSensor),
				new ChaseState(_player, _movementBehaviour, _visionSensor),
				new CatchState(transform, _player, _animationBehaviour, _movementBehaviour)
			};

			EnterState(_states.First());
		}

		private void EnterState(BaseState state)
		{
			currentState?.Exit();
			
			currentState = state;
			currentState.Enter();
			
			Debug.Log($"[AiBehaviour] => Entered: {currentState.GetType()}");
		}

		private BaseState GetState(Type type)
		{
			return _states.FirstOrDefault(item => item.GetType() == type);
		}
	}
}