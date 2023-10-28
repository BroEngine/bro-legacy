using Bro.Network.TransmitProtocol;
using UnityEngine;

namespace Bro.Sketch.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(Vector3), UniversalParamTypeIndex.Vector3)]
    public class Vector3Param : BaseParam, IObjectParam
    {
        private readonly FloatParam _x;
        private readonly FloatParam _y;
        private readonly FloatParam _z;

        object IObjectParam.Value
        {
            get => Value;
            set => Value = (Vector3) value;
        }
        
        System.Type IObjectParam.ValueType => typeof(Vector3);

        public Vector3 Value
        {
            get
            {
                CheckInitialized();
                return new Vector3(_x.Value, _y.Value, _z.Value);
            }
            set
            {
                _x.Value = value.x;
                _y.Value = value.y;
                _z.Value = value.z;
                IsInitialized = true;
                if (!IsValid)
                {
                    Bro.Log.Error("Parameter is not valid");
                }
            }
        }

        public override bool IsValid => _x.IsValid && _y.IsValid && _z.IsValid;

        public override bool IsInitialized => _x.IsInitialized && _y.IsInitialized && _z.IsInitialized;

        public Vector3Param() : this(false)
        {
            
        }
        
        public Vector3Param(bool isOptional = false) : base(isOptional)
        {
            _x = new FloatParam(isOptional);
            _y = new FloatParam(isOptional);
            _z = new FloatParam(isOptional);
        }

        public override void Write(IWriter writer)
        {
            _x.Write(writer);
            _y.Write(writer);
            _z.Write(writer);
        }

        public override void Read(IReader reader)
        {
            _x.Read(reader);
            _y.Read(reader);
            _z.Read(reader);
        }

        public override void Cleanup()
        {
            _x.Cleanup();
            _y.Cleanup();
            _z.Cleanup();
            
            base.Cleanup();
        }
    }
}