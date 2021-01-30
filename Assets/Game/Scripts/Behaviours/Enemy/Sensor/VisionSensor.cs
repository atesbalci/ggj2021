using UnityEngine;

namespace Game.Behaviours.Enemy.Sensor
{
	public class VisionSensor : MonoBehaviour
	{
		[SerializeField] [Range(0,  1f)] 
		private float _sightThreshold = 0.5f;
		[SerializeField] [Range(0f, 50f)]
		private float _distanceThreshold = 10f;
		
		private Transform _player;
		
		private void Awake()
		{
			_player = GameObject.FindGameObjectWithTag(Tags.PLAYER).transform;
		}
		
		public bool IsPlayerInSight()
		{
			float sightToPlayer = Vector3.Dot((_player.position - transform.position).normalized, transform.forward);
			float distanceToPlayer = Vector3.Distance(_player.position, transform.position);

			return sightToPlayer > _sightThreshold && distanceToPlayer < _distanceThreshold;
		}

		public bool IsPlayerInCatchDistance()
		{
			float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
			return distanceToPlayer < 2.5f;
		}
	} 
}