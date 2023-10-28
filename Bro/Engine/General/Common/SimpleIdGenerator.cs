

namespace Bro
{
    public class SimpleIdGenerator
    {
        private readonly short _from;
        private short _current;
        private readonly short _to;
        
        public SimpleIdGenerator(short from, short to)
        {
            _from = from;
            _to = to;
            _current = from;
        }

        public short GenerateId()
        {
            _current++;
            if (_current >= _to)
            {
                _current = _from;
            }

            return _current;
        }
    }
}