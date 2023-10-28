using System;
using System.Collections.Generic;

namespace Bro
{
    public static class IdGenerator
    {
        public abstract class Base
        {
            protected readonly int _maxId;
            protected readonly int _minId;
            private int _currentId;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromId">including</param>
            /// <param name="toId">excluding</param>
            protected Base(int fromId, int toId)
            {
                _minId = fromId;
                _maxId = toId - 1;
                _currentId = _maxId;
            }
            
            protected abstract void MakeOccupied(int id);
            protected abstract void MakeFree(int id);
            protected abstract bool IsOccupied(int id);

            public virtual void Reset()
            {
                _currentId = _maxId;
            }

            /// <summary>
            /// returns any not ocupated id
            /// </summary>
            /// <returns></returns>
            protected int AcquireIntId()
            {
                int previous = _currentId;
                do
                {
                    ++_currentId;
                    if (_currentId > _maxId)
                    {
                        _currentId = _minId;
                    }

                    if (previous == _currentId)
                    {
                        throw new ArgumentException("All ids are already occupied");
                    }
                } while (IsOccupied(_currentId)); // is occupied

                MakeOccupied(_currentId);
                return _currentId;
            }
            
            /// <summary>
            /// return the incoming id if it's not ocupated, else throws exception
            /// </summary>
            /// <param name="id">id that will be return if not ocupated</param>
            /// <returns></returns>
            protected int AcquireIntId(int id)
            {
                if (IsOccupied(id)) // is occupied
                {
                    throw new ArgumentException("This id (" + id + ") is already occupied ");
                }

                MakeOccupied(id);
                return id;
            }

            /// <summary>
            /// return id from range [fromId, toId) if it's not ocupated, else throws exception
            /// </summary>
            /// <param name="fromId">including</param>
            /// <param name="toId">excluding</param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            protected int AcquireIntId(int fromId, int toId)
            {
                for (int id = fromId; id < toId; ++id)
                {
                    if (!IsOccupied(id))
                    {
                        MakeOccupied(id);
                        return id;
                    }
                }

                throw new ArgumentException("This id range is already occupied");
            }

            /// <summary>
            /// returns any not ocupated id from range, the id will begin after previousId
            /// </summary>
            /// <returns></returns>
            protected int AcquireIntId(int fromId, int toId, int previousId)
            {
                int currentId = previousId;
                do
                {
                    ++currentId;
                    if (currentId >= toId)
                    {
                        currentId = fromId;
                    }

                    if (currentId == previousId)
                    {
                        throw new ArgumentException("This id range is already occupied");
                    }
                } while (IsOccupied(currentId)); // is occupied

                MakeOccupied(currentId);
                return currentId;
            }


            protected void ReleaseId(int id)
            {
                MakeFree(id);
            }
        }

        public class ArrayBased : Base
        {
            private readonly bool[] _ids;

            protected ArrayBased(int fromId, int toId): base(fromId, toId)
            {
                _ids = new bool[toId - fromId];
            }

            protected override void MakeOccupied(int id)
            {
                _ids[id - _minId] = true;
            }

            protected override void MakeFree(int id)
            {
                _ids[id - _minId] = false;
            }

            protected override bool IsOccupied(int id)
            {
                return _ids[id - _minId];
            }

            public override void Reset()
            {
                base.Reset();
                for (var index = 0; index < _ids.Length; index++)
                {
                    _ids[index] = false;
                }
            }
        }
        
        public class DictionaryBased : Base
        {
            private readonly Dictionary<int,object> _ids = new Dictionary<int, object>();

            protected DictionaryBased(int fromId, int toId): base(fromId, toId)
            {
                
            }

            protected override void MakeOccupied(int id)
            {
                _ids[id] = null;
            }

            protected override void MakeFree(int id)
            {
                _ids.Remove(id);
            }

            protected override bool IsOccupied(int id)
            {
                return _ids.ContainsKey(id);
            }

            public override void Reset()
            {
                base.Reset();
                _ids.Clear();
            }
        }
        
        
        public class Int : DictionaryBased
        {
            public Int(int fromId, int toId) : base(fromId, toId)
            {
            }

            public int AcquireId()
            {
                return base.AcquireIntId();
            }

            public int AcquireId(int id)
            {
                return base.AcquireIntId(id);
            }

            public int AcquireId(int fromId, int toId)
            {
                return base.AcquireIntId(fromId, toId);
            }

            public new void ReleaseId(int id)
            {
                base.ReleaseId(id);
            }
        }

        public class Short : ArrayBased
        {
            public Short(short fromId, short toId) : base(fromId, toId)
            {
            }

            public short AcquireId()
            {
                return (short) AcquireIntId();
            }

            public short AcquireId(short id)
            {
                return (short) AcquireIntId(id);
            }

            public short AcquireId(short fromId, short toId)
            {
                return (short) AcquireIntId(fromId, toId);
            }

            public short AcquireId(short fromId, short toId, short current)
            {
                return (short) AcquireIntId(fromId, toId, current);
            }

            public void ReleaseId(short id)
            {
                base.ReleaseId(id);
            }
        }
        
        public class Byte : ArrayBased
        {
            public Byte(int fromId = 0, int toId = 255) : base(fromId, toId)
            {
            }

            public byte AcquireId()
            {
                return (byte) AcquireIntId();
            }

            public byte AcquireId(byte id)
            {
                return (byte) AcquireIntId(id);
            }

            public byte AcquireId(byte fromId, byte toId)
            {
                return (byte) AcquireIntId(fromId, toId);
            }

            public void ReleaseId(byte id)
            {
                base.ReleaseId(id);
            }
        }
    }
}