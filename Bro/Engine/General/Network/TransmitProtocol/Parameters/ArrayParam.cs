using System;
using System.Collections.Generic;

namespace Bro.Network.TransmitProtocol
{
    public class ArrayParam<T> : BaseParam where T : BaseParam, new()
    {
        public delegate T CreateDelegate();

        private readonly IIntegerParam _paramsAmount;
        private readonly List<T> _params = new List<T>();
        private readonly CreateDelegate _creator;

        public List<T> Params
        {
            get
            {
                CheckInitialized();
                return _params;
            }
        }

        public void Add(T param)
        {
            if (param == null)
            {
                Bro.Log.Error("array param added param is null");
                throw new ArgumentException("param is null");
            }

            if (!param.IsInitialized)
            {
                Bro.Log.Error("array param added param is not initialized " + param.GetType() + " " + param.DebugData);
                throw new ArgumentException("param is not initialized " + param.GetType() + " " + param.DebugData);
            }

            _params.Add(param);
        }

        public void Add(IEnumerable<T> parameters)
        {
            foreach (var param in parameters)
            {
                Add(param);
            }
        }

        public void Clear()
        {
            _params.Clear();
        }
        
        public bool Contains(T p)
        {
            return _params.Contains(p);
        }

        public ArrayParam(int maxParamsAmount, bool isOptional = false, CreateDelegate creator = null) : base(isOptional)
        {
            if (maxParamsAmount <= byte.MaxValue)
            {
                _paramsAmount = new ByteParam();
            }
            else if (maxParamsAmount <= short.MaxValue)
            {
                _paramsAmount = new ShortParam();
            }
            else
            {
                _paramsAmount = new IntParam();
            }

            _creator = creator;
        }

        public override bool IsInitialized
        {
            get
            {
                foreach (var p in _params)
                {
                    if (!p.IsInitialized)
                    {
                        string dataString = (p != null) ? p.DebugData : "<null>";
                        Bro.Log.Error("DynamicTypedArrayParam: Param is not initialized " + p.GetType() + " data = " +
                                      dataString);
                        return false;
                    }
                }

                return true;
            }
        }

        public override bool IsValid
        {
            get
            {
                foreach (var p in _params)
                {
                    if (!p.IsValid)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override void Read(IReader reader)
        {
            _paramsAmount.Read(reader);
            _params.Clear();
            var paramsAmount = _paramsAmount.Value;
            for (int i = 0; i < paramsAmount; ++i)
            {
                _params.Add(CreateItem());
            }

            foreach (var p in _params)
            {
                p.Read(reader);
            }
        }

        public T CreateItem()
        {
            if (_creator != null)
            {
                return _creator();
            }

            return NetworkPool.GetParam<T>();
        }

        public override void Write(IWriter writer)
        {
            _paramsAmount.Value = _params.Count;
            _paramsAmount.Write(writer);
            foreach (var p in _params)
            {
                p.Write(writer);
            }
        }

        public override void Cleanup()
        {
            for (int i = 0; i < _params.Count; i++)
            {
                _params[i].Cleanup();
                _params[i].Release();
            }

            _params.Clear();

            base.Cleanup();
        }
    }
}