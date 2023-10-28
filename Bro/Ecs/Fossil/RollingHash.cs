namespace Bro.Fossil
{
	/* смотреть тут: https://fossil-scm.org/home/doc/tip/www/delta_encoder_algorithm.wiki#rollhash */
	public class RollingHash
	{
		private ushort _a;
		private ushort _b;
		private ushort _i;
		private readonly byte[] _z = new byte[Delta.HashSize];
		
		public uint Value => (uint)(_a & 0xffff) | ((uint)(_b & 0xffff) << 16);
		
		public void Init (byte[] z, int pos)
		{
			ushort a = 0, b = 0;
			for(var i = 0; i < Delta.HashSize; i++)
			{
				var x = z[pos + i];
				a = (ushort) ((a + x) & 0xffff);
				b = (ushort) ((b + (Delta.HashSize - i) * x) & 0xffff);
				_z[i] = x;
			}
			
			_a = (ushort) (a & 0xffff);
			_b = (ushort) (b & 0xffff);
			_i = 0;
		}

		public void Next (byte c) 
		{
			ushort old = _z[_i];
			_z[_i] = c;
			_i = (ushort) ((_i + 1) & (Delta.HashSize - 1));
			_a = (ushort) (_a - old + c);
			_b = (ushort) (_b - Delta.HashSize * old + _a);
		}
	}
}