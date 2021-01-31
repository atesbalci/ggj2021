using System;
using UnityEngine;

namespace Game.Behaviours.Boundaries
{
    [RequireComponent(typeof(Collider))]
    public class BoundsBehaviour : MonoBehaviour
    {
        public event Action<BoundDistanceZone> DistanceZoneChange;
        
        [SerializeField] private Transform _playerTransform;
        
        private Bounds _bounds;
        private BoundDistanceZone _currentZone;
        
        private void Awake()
        {
            _bounds = GetComponent<Collider>().bounds;
            var size = _bounds.size;
            size.y = float.MaxValue;
            _bounds.size = size;
        }

        private void Update()
        {
            var size = _bounds.size;
            var playerPos = _playerTransform.position;
            var playerPoints = new []
            {
                new Vector3(-size.x, playerPos.y, playerPos.z),
                new Vector3(size.x, playerPos.y, playerPos.z),
                new Vector3(playerPos.x, playerPos.y, size.z),
                new Vector3(playerPos.x, playerPos.y, -size.z)
            };

            var minDistSq = float.MaxValue;
            foreach (var point in playerPoints)
            {
                var distSq = (_bounds.ClosestPoint(point) - playerPos).sqrMagnitude;
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                }
            }

            var minDist = Mathf.Sqrt(minDistSq);
            UpdateDistanceToBounds(minDist);
        }

        private void UpdateDistanceToBounds(float distance)
        {
            var newZone = BoundDistanceZone.None;
            if (distance < Constants.BoundDeathDistance)
            {
                newZone = BoundDistanceZone.Death;
            }
            else if (distance < Constants.BoundCloseDistance)
            {
                newZone = BoundDistanceZone.Close;
            }
            else if (distance < Constants.BoundMidDistance)
            {
                newZone = BoundDistanceZone.Mid;
            }
            else if (distance < Constants.BoundFarDistance)
            {
                newZone = BoundDistanceZone.Far;
            }

            if (newZone != _currentZone)
            {
                _currentZone = newZone;
                DistanceZoneChange?.Invoke(_currentZone);
            }
        }
    }
        
    public enum BoundDistanceZone
    {
        None = 0,
        Far,
        Mid,
        Close,
        Death
    }
}
