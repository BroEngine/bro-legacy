using Bro.Network.TransmitProtocol;
using Bro.Sketch;
using UnityEngine;

namespace Bro.Sketch.Network.TransmitProtocol
{
    public class ShortVector2Param : BaseParam
    {
        private readonly ShortFloatParam _x;
        private readonly ShortFloatParam _y;

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
                if (!IsValid)
                {
                    Bro.Log.Error("Parameter is not valid");
                }
            }
        }

        public override bool IsValid
        {
            get { return _x.IsValid && _y.IsValid; }
        }

        public override bool IsInitialized
        {
            get { return _x.IsInitialized && _y.IsInitialized; }
        }

        public ShortVector2Param(ShortFloatConverter xConverter, ShortFloatConverter yConverter, bool isOptional = false) : base(isOptional)
        {
            _x = new ShortFloatParam(xConverter, isOptional);
            _y = new ShortFloatParam(yConverter, isOptional);
        }

        public ShortVector2Param() : base(false)
        {
            throw new System.NotSupportedException();
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