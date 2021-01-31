using System;

namespace Game.Helpers.DistanceZones
{
    public class DistanceZoneProperty
    {
        public event Action<DistanceZone> ZoneChange; 
        
        private readonly float _reachDistance;
        private readonly float _closeDistance;
        private readonly float _midDistance;
        private readonly float _farDistance;
        
        public DistanceZone CurrentZone { get; private set; }

        public DistanceZoneProperty(float reachDistance, float closeDistance, float midDistance, float farDistance)
        {
            _reachDistance = reachDistance;
            _closeDistance = closeDistance;
            _midDistance = midDistance;
            _farDistance = farDistance;
        }

        public void UpdateDistance(float distance)
        {
            var newZone = DistanceZone.None;
            if (distance < _reachDistance)
            {
                newZone = DistanceZone.Close;
            }
            else if (distance < _closeDistance)
            {
                newZone = DistanceZone.Close;
            }
            else if (distance < _midDistance)
            {
                newZone = DistanceZone.Mid;
            }
            else if (distance < _farDistance)
            {
                newZone = DistanceZone.Far;
            }

            if (newZone != CurrentZone)
            {
                CurrentZone = newZone;
                ZoneChange?.Invoke(CurrentZone);
            }
        }
    }
        
    public enum DistanceZone
    {
        None = 0,
        Far,
        Mid,
        Close,
        Reach
    }
}