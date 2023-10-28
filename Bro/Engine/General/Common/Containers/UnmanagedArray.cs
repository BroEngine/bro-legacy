using System;
using System.Runtime.InteropServices;
using Bro.Json;
using UnityEngine;

namespace Bro
{
    [Serializable]
    public struct UnmanagedArray<T> where T : struct
    {
        private T[] _array;
        private int _length;

        [JsonIgnore]
        public int Length
        {
            get
            {
                return _length;
            }
        }
        
        [JsonIgnore]
        public T this[int i]
        {
            get
            {
                return _array[i];
            }
            set
            {
                _array[i] = value;
            }
        }

        public UnmanagedArray(int length)
        {
            _length = length;
            _array = new T[_length];
        }
        
        public UnmanagedArray(T[] array)
        {
            _length = array.Length;
            _array = array;
        }

        public T[] ToArray()
        {
            return _array;
        }
    }
    
    /*
    public unsafe struct UnmanagedArray<T> : IDisposable where T : unmanaged
    {
        private readonly IntPtr _arrayPtr;

        public T this[int i]
        {
            get
            {
                return *( (T*) _arrayPtr + i);
            }
            set
            {
                *((T*)_arrayPtr + i) = value;
            }
        }
        
        public UnmanagedArray(int length)
        {
            _arrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)) * length);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_arrayPtr);
        }
    }
    */
    
    /*
    public unsafe struct UnmanagedArray<T> : IDisposable where T : struct
    {
        private readonly IntPtr _arrayPtr;
        private readonly int _sizeofT;

        public T this[int i]
        {
            get
            {
                return (T) Marshal.PtrToStructure(_arrayPtr + i * _sizeofT, typeof(T));
            }
            set
            {
                Marshal.StructureToPtr(value, _arrayPtr + i * _sizeofT, false);
            }
        }
        
        public UnmanagedArray(int length)
        {
            _sizeofT = Marshal.SizeOf(typeof(T));
            _arrayPtr = Marshal.AllocHGlobal(_sizeofT * length);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_arrayPtr);
        }
    }
    */
    
}