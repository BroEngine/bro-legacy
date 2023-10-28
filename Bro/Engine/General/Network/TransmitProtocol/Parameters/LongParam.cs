using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(long), UniversalParamTypeIndex.Long)]
    public class LongParam : BaseParam, IObjectParam
    {
        private long _value;
        private readonly long _minValue;
        private readonly long _maxValue;
        private readonly bool _hasBounds;

        public long Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                if (_hasBounds && (_value < _minValue || _value > _maxValue))
                {
                    throw new ArgumentOutOfRangeException();
                }

                _value = value;
                IsInitialized = true;
            }
        }

        public long ValueOrDefault
        {
            get
            {
                if (IsInitialized)
                {
                    return Value;
                }

                return 0L; /* not null */
            }
        }
        
        object IObjectParam.Value
        {
            get => Value;
            set => Value = (long) value;
        }
        
        System.Type IObjectParam.ValueType => typeof(long);

        public LongParam(long minValue, long maxValue, bool isOptional = false) : base(isOptional)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _hasBounds = true;
        }


        public LongParam(bool isOptional = false) : base(isOptional)
        {
            _hasBounds = false;
        }

        public LongParam() : this(false)
        {
            _hasBounds = true;
        }
        
        public override void Write(IWriter writer)
        {
            writer.Write(_value);
        }

        public override void Read(IReader reader)
        {
            reader.Read(out _value);
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = 0;
            base.Cleanup();
        }
    }
}