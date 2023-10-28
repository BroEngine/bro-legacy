using System;
using System.Collections.Generic;

namespace Bro.Network.TransmitProtocol
{
    public class UniversalParam : BaseParam
    {
        private IObjectParam _objectParam => _baseParam as IObjectParam;

        private BaseParam _baseParam;
        public bool SupportUnknownTypes = false;

        public UniversalParam() : this(isOptional: false)
        {
        }

        public UniversalParam(bool isOptional) : base(isOptional)
        {
        }

        public override bool IsInitialized => _baseParam != null && _baseParam.IsInitialized;

        public override bool IsValid => IsOptional || _baseParam.IsValid;

        public object Value
        {
            get
            {
                CheckInitialized();
                return _objectParam.Value;
            }
            set
            {
                var valueType = value.GetType();
                var paramData = ParamsRegistry.GetParamData(valueType);
                if (paramData != null)
                {
                    _baseParam = paramData.CreateParam();
                    _objectParam.Value = value;
                }
                else
                {
                    // нужно добавить параметр ( NetworkParam ) с атрибутом [UniversalParamRegistration(typeof(string), UniversalParamTypeIndex.String)]
                    Bro.Log.Error($"cannot set value to [{value.GetType()}];");
                    throw new NotSupportedException($"cannot set value to {value.GetType()}");
                }
            }
        }

        public BaseParam Param
        {
            get => _baseParam;
            set
            {
                #if UNITY_EDITOR
                if (!(value is IObjectParam))
                {
                    Bro.Log.Error($"universal param :: param {value.GetType()} should implement IObjectParam");
                }
                #endif
                _baseParam = value;
            }
        }

        public override void Write(IWriter writer)
        {
            short length = 0;
            long startPos = 0;
            long lengthPos = 0;
            long endPos = 0;
            if (SupportUnknownTypes)
            {
                lengthPos = writer.Position;
                writer.Write(length);
                startPos = writer.Position;
            }
            
            var paramData = ParamsRegistry.GetParamData(_objectParam.ValueType);

            writer.Write(paramData.ParamTypeIndex);

            _baseParam.Write(writer);
            if (SupportUnknownTypes)
            {
                endPos = writer.Position;
                writer.Position = lengthPos;
                length = (short) (endPos - startPos);
                writer.Write(length);
                writer.Position = endPos;
            }
        }

        public override void Read(IReader reader)
        {
            short length = 0;
            if (SupportUnknownTypes)
            {
                reader.Read(out length);
            }

            reader.Read(out byte paramTypeIndex);

            var paramData = ParamsRegistry.GetParamData(paramTypeIndex);
            if (paramData != null)
            {
                _baseParam = paramData.CreateParam();
            }
            else if (SupportUnknownTypes)
            {
                reader.Position = reader.Position - 1;
                _baseParam = new UnknownObjectParam(length);
            }
            else
            {
                Bro.Log.Error("Cannot read param with index " + paramTypeIndex + "; check attribute for corresponding param type.");
            }

            _baseParam.Read(reader);
        }

        public override void Cleanup()
        {
            _baseParam?.Cleanup();
            _baseParam = null;
            base.Cleanup();
        }
    }
}