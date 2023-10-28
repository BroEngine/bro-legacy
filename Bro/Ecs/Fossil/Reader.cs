namespace Bro.Fossil
{
	public class Reader
	{
		private static readonly int[] IntegerСompressionValues = {
			-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1,
			0,  1,  2,  3,  4,  5,  6,  7,    8,  9, -1, -1, -1, -1, -1, -1,
			-1, 10, 11, 12, 13, 14, 15, 16,   17, 18, 19, 20, 21, 22, 23, 24,
			25, 26, 27, 28, 29, 30, 31, 32,   33, 34, 35, -1, -1, -1, -1, 36,
			-1, 37, 38, 39, 40, 41, 42, 43,   44, 45, 46, 47, 48, 49, 50, 51,
			52, 53, 54, 55, 56, 57, 58, 59,   60, 61, 62, -1, -1, -1, 63, -1
		};

		public uint Position
		{
			get => _position;
			set => _position = value;
		}

		private byte[] _data;
		private uint _position;

		public void Reset (byte[] array)
		{
			_data = array;
			_position = 0;
		}

		public bool CanRead() 
		{
			return _position < _data.Length;
		}

		private byte ReadByte() 
		{
			var b = _data[_position];
			_position++;
			return b;
		}

		public char ReadChar() 
		{
			return (char) ReadByte();
		}

		public uint ReadInt ()
		{
			uint v = 0;
			int c;
			while(CanRead() && (c = IntegerСompressionValues[0x7f & ReadByte()]) >= 0) 
			{
				v = (uint) (((int) v << 6) + c);
			}
			_position--;
			return v;
		}
	}
}