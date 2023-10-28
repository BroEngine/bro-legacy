using System;

namespace Bro.Network.TransmitProtocol
{
    public class ToByteArrayParam : BaseParam
    {
        private readonly BaseParam _source;
        private readonly int _maxSize;
        private byte[] _data;

        public ToByteArrayParam(BaseParam source, int maxSizeInBytes) : base(source.IsOptional)
        {
            _maxSize = maxSizeInBytes;
            _source = source;
        }

        public override bool IsInitialized
        {
            get { return _source.IsInitialized || (_data != null && _data.Length > 0); }
        }

        public override bool IsValid
        {
            get { return _source.IsValid; }
        }

        private void WriteSize(IWriter writer, int arraySize)
        {
            if (_maxSize <= byte.MaxValue)
            {
                writer.Write((byte) arraySize);
            }
            else if (_maxSize <= short.MaxValue)
            {
                writer.Write((short) arraySize);
            }
            else
            {
                writer.Write((int) arraySize);
            }
        }

        public override void Write(IWriter writer)
        {
            int dataSize = 0;
            var sizePosition = writer.Position;
            WriteSize(writer, dataSize);
            var dataStartPosition = writer.Position;
            _source.Write(writer);
            var dataEndPosition = writer.Position;
            writer.Position = sizePosition;
            dataSize = (int) (dataEndPosition - dataStartPosition);
            WriteSize(writer, dataSize);
            if (dataSize > _maxSize)
            {
                throw new System.ArgumentException("Wrong array size");
            }

            writer.Position = dataEndPosition;
        }

        public override void Read(IReader reader)
        {
            int arraySize;
            if (_maxSize <= byte.MaxValue)
            {
                byte sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }
            else if (_maxSize <= short.MaxValue)
            {
                short sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }
            else
            {
                int sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }

            _data = new byte[arraySize];
            reader.Read(out _data, arraySize);
        }

        public BaseParam Param
        {
            get { return _source; }
        }

        public byte[] Data
        {
            get
            {
                if (_data == null)
                {
                    throw new InvalidOperationException("Didn't read any data before, _data is invalid");
                }

                return _data;
            }
        }

        public override void Cleanup()
        {
            _data = null;
            base.Cleanup();
        }
    }
}


namespace Bro.Network.TransmitProtocol
{
    public class FromByteArrayParam : BaseParam
    {
        private readonly BaseParam _destination;
        private byte[] _data;

        public FromByteArrayParam(BaseParam destination) : base(destination.IsOptional)
        {
            _destination = destination;
        }

        public override bool IsInitialized
        {
            get { return _destination.IsInitialized || (_data != null && _data.Length > 0); }
        }

        public override bool IsValid
        {
            get { return _destination.IsOptional || (_data != null && _data.Length > 0); }
        }

        public override void Write(IWriter writer)
        {
            writer.Write(_data);
        }

        public override void Read(IReader reader)
        {
            _destination.Read(reader);
        }

        public byte[] Data
        {
            set { _data = value; }
        }

        public BaseParam Param
        {
            get
            {
                CheckInitialized();
                return _destination;
            }
        }

        public override void Cleanup()
        {
            _data = null;
            base.Cleanup();
        }
    }
}