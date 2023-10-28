using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network.TransmitProtocol
{
    public class QuaternionParam : BaseParam, IObjectParam
    {
        private readonly FloatParam _x;
        private readonly FloatParam _y;
        private readonly FloatParam _z;
        private readonly FloatParam _w;


        object IObjectParam.Value
        {
            get => Value;
            set => Value = (UnityEngine.Quaternion) value;
        }
        
        System.Type IObjectParam.ValueType => typeof( UnityEngine.Quaternion);

        public UnityEngine.Quaternion Value
        {
            get
            {
                CheckInitialized();
                return new UnityEngine.Quaternion(_x.Value, _y.Value, _z.Value, _w.Value);
            }
            set
            {
                _x.Value = value.x;
                _y.Value = value.y;
                _z.Value = value.z;
                _w.Value = value.w;
                IsInitialized = true;
                if (!IsValid)
                {
                    Bro.Log.Error("Parameter is not valid");
                }
            }
        }

        public override bool IsValid
        {
            get { return _x.IsValid && _y.IsValid && _z.IsValid && _w.IsValid; }
        }

        public override bool IsInitialized
        {
            get { return _x.IsInitialized && _y.IsInitialized && _z.IsInitialized && _w.IsInitialized; }
        }

        public QuaternionParam(bool isOptional = false) : base(isOptional)
        {
            _x = new FloatParam(isOptional);
            _y = new FloatParam(isOptional);
            _z = new FloatParam(isOptional);
            _w = new FloatParam(isOptional);
        }

        public override void Write(IWriter writer)
        {
            _x.Write(writer);
            _y.Write(writer);
            _z.Write(writer);
            _w.Write(writer);
        }

        public override void Read(IReader reader)
        {
            _x.Read(reader);
            _y.Read(reader);
            _z.Read(reader);
            _w.Read(reader);
        }

        public override void Cleanup()
        {
            _x.Cleanup();
            _y.Cleanup();
            _z.Cleanup();
            _w.Cleanup();
            
            base.Cleanup();
        }
    }
}