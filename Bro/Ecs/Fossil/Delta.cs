using System;
using System.IO;

namespace Bro.Fossil
{
	/* оригинал https://fossil-scm.org/home/artifact/f3002e96cc35f37b  */
	public class Delta
	{
		public const ushort HashSize = 16;
		public const int BufferSize = 128 * 1024;
		
		private const char CharSize = '\n';
		private const char CharCopyStart = '@';
		private const char CharCopyEnd = ',';
		private const char CharInsert = ':';
		private const char CharInsertEnd = ';';
		
		private readonly Writer _writer = new Writer();
		private readonly Reader _reader = new Reader();
		private readonly RollingHash _hash = new RollingHash();
		
		private readonly int[] _collide =  new int[BufferSize];
		private readonly int[] _landmark = new int[BufferSize];
		
		public int Create(byte[] origin, int originSize, byte[] target, int targetSize, byte[] result) 
		{
			_writer.Reset();

			var lengthOut = targetSize; // target.Length;
			var lengthSrc = originSize; // origin.Length;
			int i, lastRead = -1;
			int hv;

			_writer.WriteInt((uint) lengthOut);
			_writer.WriteChar(CharSize);
			
			if (lengthSrc <= HashSize) 
			{
				_writer.WriteInt((uint) lengthOut);
				_writer.WriteChar(CharInsert);
				_writer.WriteArray(target, 0, lengthOut);
				_writer.WriteInt(Tools.GetChecksum(target, targetSize));
				_writer.WriteChar(CharInsertEnd);
				return _writer.FillResult(result);
			}
			
			var nHash = lengthSrc / HashSize;
			
			Assert(nHash < BufferSize, "buffer size is too small, expected = " + nHash);
			
			for (i = 0; i < nHash; i++)
			{
				_collide[i] = -1;
			}

			for (i = 0; i < nHash; i++)
			{
				_landmark[i] = -1;
			}
			
			for (i = 0; i < lengthSrc-HashSize; i += HashSize) 
			{
				_hash.Init(origin, i);
				hv = (int) (_hash.Value % nHash);
				_collide[i / HashSize] = _landmark[hv];
				_landmark[hv] = i / HashSize;
			}

			var foundation = 0;
			while (foundation + HashSize < lengthOut) 
			{
				var bestOffset = 0;
				var bestLiteral = 0;
				_hash.Init(target, foundation);
				i = 0; 
				var bestCnt = 0;
				while (true) 
				{
					var limit = 250;
					hv = (int) (_hash.Value % nHash);
					var iBlock = _landmark[hv];
					while (iBlock >= 0 && limit-- > 0) 
					{
						int j, k, x, y;
						
						var iSrc = iBlock * HashSize;
						for (j = 0, x = iSrc, y = foundation + i; x < lengthSrc && y < lengthOut; j++, x++, y++) 
						{
							if (origin[x] != target[y])
							{
								break;
							}
						}
						j--;
						
						for (k = 1; k < iSrc && k <= i; k++) 
						{
							if (origin[iSrc - k] != target[foundation + i - k])
							{
								break;
							}
						}
						k--;
						
						var offset = iSrc - k;
						var cnt = j + k + 1;
						var literal = i - k;
						var sz = Tools.DigitCount(i - k) + Tools.DigitCount(cnt) + Tools.DigitCount(offset) + 3;
						if (cnt >= sz && cnt > bestCnt) 
						{
							bestCnt = cnt;
							bestOffset = iSrc-k;
							bestLiteral = literal;
						}
						iBlock = _collide[iBlock];
					}

					if (bestCnt > 0) 
					{
						if (bestLiteral > 0) 
						{
							_writer.WriteInt((uint) bestLiteral);
							_writer.WriteChar(CharInsert);
							_writer.WriteArray(target, foundation, foundation+bestLiteral);
							foundation += bestLiteral;
						}
						foundation += bestCnt;
						_writer.WriteInt((uint) bestCnt);
						_writer.WriteChar(CharCopyStart);
						_writer.WriteInt((uint) bestOffset);
						_writer.WriteChar(CharCopyEnd);
						if (bestOffset + bestCnt -1 > lastRead) 
						{
							lastRead = bestOffset + bestCnt - 1;
						}
						break;
					}

					if (foundation + i + HashSize >= lengthOut)
					{
						_writer.WriteInt((uint) (lengthOut-foundation));
						_writer.WriteChar(CharInsert);
						_writer.WriteArray(target, foundation, foundation + lengthOut - foundation);
						foundation = lengthOut;
						break;
					}
					_hash.Next(target[foundation + i + HashSize]);
					i++;
				}
			}
			
			if(foundation < lengthOut) 
			{
				_writer.WriteInt((uint) (lengthOut-foundation));
				_writer.WriteChar(CharInsert);
				_writer.WriteArray(target, foundation, foundation + lengthOut - foundation);
			}
			
			_writer.WriteInt(Tools.GetChecksum(target, targetSize));
			_writer.WriteChar(CharInsertEnd);
			return _writer.FillResult(result);
		}

		public int Apply(byte[] origin, int originSize, byte[] delta, int deltaSize, byte[] result) 
		{
			_writer.Reset();
			_reader.Reset(delta);
			var limit = _reader.ReadInt();
			var lengthSrc = originSize; // (uint) origin.Length;
			var lengthDelta = deltaSize; // (uint) delta.Length;
			uint total = 0;
			Assert(_reader.ReadChar() == CharSize, "size integer not terminated by valid character");
			
			while(_reader.CanRead())
			{
				var count = _reader.ReadInt();
				switch (_reader.ReadChar()) {
				case CharCopyStart:
					var offset = _reader.ReadInt();
					total += count;
					Assert( _reader.CanRead() && _reader.ReadChar() == CharCopyEnd, "copy command not terminated by valid character");
					Assert(total <= limit, "copy exceeds output file size");
					Assert(offset + count <= lengthSrc, "copy extends past end of input");
					_writer.WriteArray(origin, (int) offset, (int) (offset+count));
					break;
				case CharInsert:
					total += count;
					Assert(total <= limit, "insert command gives an output larger than predicted");
					Assert(count <= lengthDelta, "insert count exceeds size of delta");
					_writer.WriteArray(delta, (int) _reader.Position, (int) (_reader.Position + count));
					_reader.Position += count;
					break;
				case CharInsertEnd:
					var size = _writer.FillResult(result);
					Assert(count == Tools.GetChecksum(result, size), "bad checksum");
					Assert(total == limit, "generated size does not match predicted size");
					return size;
				}
			}
			throw new Exception("unterminated delta");
		}
		
		private static void Assert(bool condition, string exception)
		{
			if (!condition)
			{
				throw new Exception(exception);
			}
		}
	
		public uint GetOutputSize(byte[] delta) 
		{
			_reader.Reset(delta);
			return _reader.ReadInt();
		}
	}
}