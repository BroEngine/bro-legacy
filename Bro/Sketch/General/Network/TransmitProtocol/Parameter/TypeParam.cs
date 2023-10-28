using System;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class TypeParam : BaseParam, IObjectParam
    {
        private readonly StringParam _data;
        
        object IObjectParam.Value
        {
            get => Value;
            set => Value = (Type) value;
        }

        public Type Value
        {
            get
            {
                CheckInitialized();
                return Type.GetType(_data.Value);
            }
            set
            {
                _data.Value = value.FullName;
                IsInitialized = true;
                if (!IsValid)
                {
                    Bro.Log.Error("Parameter is not valid");
                }
            }
        }
        
        Type IObjectParam.ValueType => typeof(Type);

        public override bool IsValid => _data.IsValid;

        public override bool IsInitialized => _data.IsInitialized;

        public TypeParam() : base(false)
        {
            _data = new StringParam(false);
        }

        public TypeParam(bool isOptional = false) : base(isOptional)
        {
            _data = new StringParam(isOptional);
        }

        public override void Write(IWriter writer)
        {
            _data.Write(writer);
        }

        public override void Read(IReader reader)
        {
            _data.Read(reader);
        }

        public override void Cleanup()
        {
            _data.Cleanup();
            base.Cleanup();
        }
    }
}