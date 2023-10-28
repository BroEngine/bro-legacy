using System.Runtime.CompilerServices;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public class EcsWorldComparer
    {
        private int[] _bufferA = new int[1024*10];
        private int[] _bufferB = new int[1024*10];
        
        private readonly int[] _checkedEntitiesA = new int[1024*10];
        private readonly int[] _checkedEntitiesB = new int[1024*10];
        private int _checkedIndex = 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CompareOwners(byte ownerId, EcsWorld worldA, EcsWorld worldB, bool debug = false)
        {
            return false; // невозможно сравнить две стурктуры без аллокации памяти
            
            var entitiesA = worldA.Filter<OwnerComponent>().End();
            var entitiesB = worldB.Filter<OwnerComponent>().End(); 
            
            var poolA = worldA.GetPool<OwnerComponent>();
            var poolB = worldB.GetPool<OwnerComponent>();

            var sizeA = 0;
            var sizeB = 0;

            foreach (var a in entitiesA)
            {
                ref var owner = ref poolA.Get(a);
                if (owner.OwnerId == ownerId)
                {
                    _bufferA[sizeA] = a;
                    ++sizeA;
                }
            }
            
            foreach (var b in entitiesB)
            {
                ref var owner = ref poolB.Get(b);
                if (owner.OwnerId == ownerId)
                {
                    _bufferB[sizeB] = b;
                    ++sizeB;
                }
            }

            var result = HardCompare(worldA, ref _bufferA, sizeA, worldB, ref _bufferB, sizeB, debug);
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HardCompare(EcsWorld worldA, ref int[] entitiesA, int sizeA, EcsWorld worldB, ref int[] entitiesB, int sizeB, bool debug)
        {
            _checkedIndex = 0;
            
            if (sizeA != sizeB)
            {
                if (debug)
                {
                    Bro.Log.Error("entities count are not the same " + sizeA + " != " + sizeB);
                }
                return false;
            }
            
            var isEqual = true;
            
            object[] componentsA = null;
            for(var i = 0; i < sizeA; ++ i)
            {
                var entityA = entitiesA[i];
                var componentsCount = worldA.GetComponents(entityA, ref componentsA);
               
                if (!Search(worldB, ref entitiesB, sizeB, ref componentsA, componentsCount, entityA))
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Search(EcsWorld worldB, ref int[] entitiesB, int sizeB, ref object[] searchComponents, int searchComponentsCount, int entityA)
        {
            object[] components = null;
            for (var i = 0; i < sizeB; ++ i)
            {
                var entityB = entitiesB[i];
                var componentsCount = worldB.GetComponents(entityB, ref components);
                if (componentsCount == searchComponentsCount)
                {
                    if (FastArrayComparer.Compare(components, searchComponents, componentsCount))
                    {
                        if (Register(entityA, entityB))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Register(int entityA, int entityB)
        {
            for (var i = 0; i < _checkedIndex; ++i) // first B
            {
                if (_checkedEntitiesB[i] == entityB)
                {
                    return false;
                }
            }   
            
            for (var i = 0; i < _checkedIndex; ++i)
            {
                if (_checkedEntitiesA[i] == entityA)
                {
                    return false;
                }
            }
            
            _checkedEntitiesA[_checkedIndex] = entityA;
            _checkedEntitiesB[_checkedIndex] = entityB;
            ++_checkedIndex;
            return true;
        }
    }
}