using System;
using UnityEngine;

namespace Game.Behaviours.Boundaries
{
    [RequireComponent(typeof(Collider))]
    public class BoundsBehaviour : MonoBehaviour
    {
        public event Action Death;

        [SerializeField] private Transform _playerTransform;

        private Bounds _bounds;

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
            var playerPoints = new[]
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
            if (minDist < Constants.BoundDeathDistance)
            {
                Death?.Invoke();
            }
        }
    }
}
