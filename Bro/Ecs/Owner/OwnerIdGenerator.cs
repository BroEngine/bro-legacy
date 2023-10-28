namespace Bro.Ecs
{
    public class OwnerIdGenerator
    {        
        private readonly byte[] _dataByte = new byte[256];
        private readonly ushort[] _dataShort = new ushort[256];
        
        private readonly byte[] _dataByteOwner = new byte[256];
        private readonly int[] _dataByteFrame = new int[256];

        
        public ushort GetNextShort(byte owner) /* dimension for one owner is 255, ushort = 65536 (0...65535) => 256x256 */
        {
            return (ushort) (owner * 256 + (_dataByte[owner]++) );
        } 
        
        public int GetNextInt(byte owner)  /* dimension for one owner is 65535 */
        {
            return (owner * 65536 + (_dataShort[owner]++) );
        } 
        
        /* 16 уникальных айдишников на кадр для каждого овнера */
        /* 2147483647 /   256  /  16   */
        /* dimension  / owners / size  */
        /* 254287 frames -> 8500 seconds -> 140 minutes */
        public int GetNextByFrame(byte owner, int frame)
        {
            var previous = _dataByteFrame[owner];
            if (previous != frame)
            {
                _dataByteOwner[owner] = 0;
            }

            _dataByteFrame[owner] = frame;
            return owner * 32 + (_dataByteOwner[owner]++) + (frame * 256 * 32);
            // return owner * 16 + (_dataByteOwner[owner]++) + (frame * 256 * 16);
        }

        public void ResetFrame(int frame)
        {
            for(var i = 0; i < _dataByteOwner.Length; ++i)
            {
                _dataByteOwner[i] = 0;
            }
        }
    }
}