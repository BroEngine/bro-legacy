using System.Runtime.CompilerServices;

namespace Bro.Fossil
{
    public static class Tools
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int DigitCount(int v)
        {
        	int i, x;
            for (i = 1, x = 64; v >= x; i++, x <<= 6)
            {
	            continue;
            }
        	return i;
        }

        /*
		    ** https://fossil-scm.org/home/artifact/f3002e96cc35f37b
			** Compute a 32-bit big-endian checksum on the N-byte buffer.  If the
			** buffer is not a multiple of 4 bytes length, compute the sum that would
			** have occurred if the buffer was padded with zeros to the next multiple
			** of four bytes.
		*/
        internal static uint GetChecksum(byte[] data, int size, int count = 0) 
        {
        	uint sum = 0, sum0 = 0, sum1 = 0, sum2 = 0,
        	z = 0, n = (uint) (count == 0 ? size : count);

        	while(n >= 16){
        		sum0 += (uint) data[z+0] + data[z+4] + data[z+8]  + data[z+12];
        		sum1 += (uint) data[z+1] + data[z+5] + data[z+9]  + data[z+13];
        		sum2 += (uint) data[z+2] + data[z+6] + data[z+10] + data[z+14];
        		sum  += (uint) data[z+3] + data[z+7] + data[z+11] + data[z+15];
        		z += 16;
        		n -= 16;
        	}
        	while(n >= 4){
        		sum0 += data[z+0];
        		sum1 += data[z+1];
        		sum2 += data[z+2];
        		sum  += data[z+3];
        		z += 4;
        		n -= 4;
        	}

        	sum += (sum2 << 8) + (sum1 << 16) + (sum0 << 24);
        	switch (n&3) {
        	case 3:
        		sum += (uint) (data [z + 2] << 8);
        		sum += (uint) (data [z + 1] << 16);
        		sum += (uint) (data [z + 0] << 24);
        		break;
        	case 2:
        		sum += (uint) (data [z + 1] << 16);
        		sum += (uint) (data [z + 0] << 24);
        		break;
        	case 1:
        		sum += (uint) (data [z + 0] << 24);
        		break;
        	}
        	return sum;
        }
    }
}