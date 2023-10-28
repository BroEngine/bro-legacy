using Bro.Network.TransmitProtocol;
using Bro.Sketch;
using UnityEngine;

namespace Bro.Sketch.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(Vector2), UniversalParamTypeIndex.Vector2)]
    public class Vector2Param : BaseParam, IObjectParam
    {
        private readonly FloatParam _x;
        private readonly FloatParam _y;

        object IObjectParam.Value
        {
            get { return Value; }
            set { Value = (Vector2) value; }
        }

        public Vector2 Value
        {
            get
            {
                CheckInitialized();
                return new Vector2(_x.Value, _y.Value);
            }
            set
            {
                _x.Value = value.x;
                _y.Value = value.y;
                IsInitialized = true;
                if (!IsValid)
                {
                    Bro.Log.Error("Parameter is not valid");
                }
            }
        }
        
        System.Type IObjectParam.ValueType => typeof(Vector2);

        public override bool IsValid
        {
            get { return _x.IsValid && _y.IsValid; }
        }

        public override bool IsInitialized
        {
            get { return _x.IsInitialized && _y.IsInitialized; }
        }

        public Vector2Param() : base(false)
        {
            _x = new FloatParam(false);
            _y = new FloatParam(false);
        }

        public Vector2Param(bool isOptional = false) : base(isOptional)
        {
            _x = new FloatParam(isOptional);
            _y = new FloatParam(isOptional);
        }

        public override void Write(IWriter writer)
        {
            _x.Write(writer);
            _y.Write(writer);
        }

        public override void Read(IReader reader)
        {
            _x.Read(reader);
            _y.Read(reader);
        }

        public override void Cleanup()
        {
            _x.Cleanup();
            _y.Cleanup();
            
            base.Cleanup();
        }
    }
}