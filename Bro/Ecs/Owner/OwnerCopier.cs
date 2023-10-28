using System;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    // todo
    // ввести новый тип компонент - local simulation например
    // класть его на те сущности - которые можно ресимулировать на клиенте
    // не учитывать мов инпут тут вообще
    public class OwnerCopier
    {
        private Type[] _buffer = new Type[128];
        
        public void CopyComponents(EcsWorld fromWorld, EcsWorld toWorld, short ownerId)
        {
            if (fromWorld == null || toWorld == null)
            {
                return;
            }

            var fromComponents = fromWorld.Filter<OwnerComponent>().End();
            var toComponents = toWorld.Filter<OwnerComponent>().End();
            
            var fromPool = fromWorld.GetPool<OwnerComponent>();
            var toPool = toWorld.GetPool<OwnerComponent>();
            
            // 1. clear toWorld
            foreach (var entity in toComponents)
            {
                if (toPool.Get(entity).OwnerId == ownerId)
                {
                    toWorld.DelEntity(entity);
                }
            }

            foreach (var entity in fromComponents)
            {
                if (fromPool.Get(entity).OwnerId == ownerId)
                {
                    CopyEntity(fromWorld, toWorld, entity);
                }
            }
        }
        
        public void CopyColliders(EcsWorld fromWorld, EcsWorld toWorld)
        {
            if (fromWorld == null || toWorld == null)
            {
                return;
            }

            // var fromComponents = fromWorld.Filter<ColliderComponent>().End();
            // var toComponents = toWorld.Filter<ColliderComponent>().End();
       
            // foreach (var entity in toComponents)
            // {
            //     toWorld.DelEntity(entity);
            // }
            //
            // foreach (var entity in fromComponents)
            // {
            //     CopyEntity(fromWorld, toWorld, entity);
            // }
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CopyEntity(EcsWorld fromWorld, EcsWorld toWorld, int fromEntity)
        {
            var newEntity = toWorld.NewEntity();
            var size = fromWorld.GetComponentTypes(fromEntity, ref _buffer);
            for (var i = 0; i < size; i++)
            {
                var type = _buffer[i];
                var fromPool = fromWorld.GetPool(type);
                var toPool = toWorld.GetPool(type);
                toPool.Copy(newEntity, fromPool, fromEntity);
            }
        }
    }
}