using System;

namespace Bro.Fossil
{
	public class Writer
	{
		private static readonly uint[] IntegerСompressionDigits = /* 64, хак для сжатия uint */ 
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D',
			'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
			'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '_', 'a', 'b', 'c', 'd', 'e',
			'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
			't', 'u', 'v', 'w', 'x', 'y', 'z', '~'
		};

		private readonly byte[] _buffer = new byte[Delta.BufferSize];
		private readonly System.IO.MemoryStream _stream;
		private readonly System.IO.BinaryWriter _streamWriter;
		private readonly uint[] _intBuffer = new uint[20]; 
		
		public Writer ()
		{
			_stream = new System.IO.MemoryStream(_buffer);
			_streamWriter = new System.IO.BinaryWriter(_stream);
		}

		public void WriteChar (char c)
		{
			_streamWriter.Write((byte)c);
		}	

		public void WriteInt (uint v)
		{
			int i, j;
			if (v == 0) 
			{
				_streamWriter.Write((byte) 48); /* 0 */
				return;
			}
			for (i = 0; v > 0; i++, v >>= 6) /* shift by 6 ( 64 ) */
			{
				_intBuffer[i] = IntegerСompressionDigits[v & 0x3f]; /* 63 */
			}
			for (j = i - 1; j >= 0; j--) {
				
				_streamWriter.Write((byte) _intBuffer [j]);
			}
		}

		public void WriteArray (byte[] a, int start, int end) 
		{
			for (var i = start; i < end; i++)
			{
				_streamWriter.Write( a[i] );
			}
		}

		public void Reset()
		{
			if (_stream.CanRead)
			{
				_streamWriter.Seek(0, System.IO.SeekOrigin.Begin);
			}
		}
		
		// todo: предусмотреть второй спобос с передачей алоцированного массива
		public int FillResult(byte[] result = null)
		{
			if (_stream.CanRead)
			{
				var dataSize = (int) _stream.Position;
				if (result == null)
				{
					result = new byte[dataSize];
				}
				Buffer.BlockCopy(_buffer, 0, result, 0, dataSize);
				return dataSize;
			}
			return 0;
		}
	}
}