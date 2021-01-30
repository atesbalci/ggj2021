using Unity.Mathematics;
using UnityEngine;

namespace Game.Behaviours.ECS.Systems
{
    public static class ECSPhysics
    {
        public static bool Intersect(AABB box, Ray ray)
        {
            double tx1 = (box.Min.x - ray.origin.x) * (1 / ray.direction.x);
            double tx2 = (box.Max.x - ray.origin.x) * (1 / ray.direction.x);

            double tmin = math.min(tx1, tx2);
            double tmax = math.max(tx1, tx2);

            double ty1 = (box.Min.y - ray.origin.y) * (1 / ray.direction.y);
            double ty2 = (box.Max.y - ray.origin.y) * (1 / ray.direction.y);

            tmin = math.max(tmin, math.min(ty1, ty2));
            tmax = math.min(tmax, math.max(ty1, ty2));

            double tz1 = (box.Min.z - ray.origin.z) * (1 / ray.direction.z);
            double tz2 = (box.Max.z - ray.origin.z) * (1 / ray.direction.z);

            tmin = math.max(tmin, math.min(tz1, tz2));
            tmax = math.min(tmax, math.max(tz1, tz2));

            return tmax >= tmin;
        }

        public static bool Intersect(AABB box1, AABB box2)
        {
            return (box1.Min.x <= box2.Max.x && box1.Max.x >= box2.Min.x) &&
                   (box1.Min.y <= box2.Max.y && box1.Max.y >= box2.Min.y) &&
                   (box1.Min.z <= box2.Max.z && box1.Max.z >= box2.Min.z);
        }
        
        public static bool Intersect(float3 point, AABB box2)
        {
            return (point.x <= box2.Max.x && point.x >= box2.Min.x) &&
                   (point.y <= box2.Max.y && point.y >= box2.Min.y) &&
                   (point.z <= box2.Max.z && point.z >= box2.Min.z);
        }
    }
}