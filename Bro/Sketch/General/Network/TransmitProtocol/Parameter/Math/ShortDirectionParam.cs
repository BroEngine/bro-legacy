using Bro.Network.TransmitProtocol;
using Bro.Sketch;
using UnityEngine;

namespace Bro.Sketch.Network.TransmitProtocol
{
    public class ShortDirectionParam : BaseParam
    {
        private static readonly ShortFloatConverter _angleConverter = new ShortFloatConverter(-180f, 180f);
        private readonly ShortFloatParam _angle;

        public Vector2 Value
        {
            get
            {
                CheckInitialized();
                return new Vector2(0f, 1f).Rotated(_angle.Value);
            }
            set
            {
                if (value.Equals(Vector2.zero))
                {
                    Bro.Log.Error("Value cannot be a zero vector");
                }
                
                _angle.Value = Vector2.SignedAngle(new Vector2(0f, 1f), value);
                if (!IsValid)
                {
                    Bro.Log.Error("Parameter is not valid");
                }
            }
        }
        
        public override bool IsValid { get { return _angle.IsValid; } }

        public override bool IsInitialized { get { return _angle.IsInitialized; } }

        public ShortDirectionParam(bool isOptional = false) : base(isOptional)
        {
            _angle = new ShortFloatParam(_angleConverter, isOptional);
        }

        public ShortDirectionParam() : this(false)
        {
        }

        public override void Write(IWriter writer)
        {
            _angle.Write(writer);
        }

        public override void Read(IReader reader)
        {
            _angle.Read(reader);
        }

        public override void Cleanup()
        {
            _angle.Cleanup();
            base.Cleanup();
        }
    }
}