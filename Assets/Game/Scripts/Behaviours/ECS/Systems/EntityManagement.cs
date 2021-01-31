using Unity.Entities;

namespace Game.Behaviours.ECS.Systems
{
    public static class EntityManagement
    {
        public static void ClearEntities()  //TODO quick fix for restarting entities on scene load, refactor later
        {
            var entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.DestroyEntity(entityManager.UniversalQuery);
        }
    }
}