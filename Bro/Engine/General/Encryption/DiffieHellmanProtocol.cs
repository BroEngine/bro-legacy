namespace Bro.Encryption
{
    public class DiffieHellmanProtocol
    {
        public enum EndpointSide
        {
            Client,
            Server
        }

        public readonly EndpointSide Side;

        private BigInteger _g; // client, public
        private BigInteger _p; // server, public

        private BigInteger _a; // client, secret
        private BigInteger _b; // server, secret

        private BigInteger _A; // client, public
        private BigInteger _B; // server, public

        private const string _salt = "5f+qH(2jvJ+qR;=4>/$QeXf96t)X-X,n";

        public DiffieHellmanProtocol(EndpointSide side)
        {
            Side = side;
        }

        public void Reset()
        {
            _g = null;
            _p = null;
            _a = null;
            _b = null;
            _A = null;
            _B = null;
        }

        public long GenerateSelfPublicPart()
        {
            Reset();
            switch (Side)
            {
                case EndpointSide.Client:
                    _g = Random.Instance.Range(1, 99);
                    return _g.LongValue();
                case EndpointSide.Server:
                    _p = Random.Instance.Range(1, 999999999);
                    return _p.LongValue();
            }

            return 0;
        }

        public void SetForeignPublicPart(long v)
        {
            switch (Side)
            {
                case EndpointSide.Client:
                    _p = new BigInteger(v);
                    break;
                case EndpointSide.Server:
                    _g = new BigInteger(v);
                    break;
            }
        }

        public long GenerateSelfSecterPart()
        {
            if (_g == null)
            {
                Bro.Log.Error("DiffieHellmanProtocol: 'g' factor is not seted " + Side);
                return 0;
            }

            if (_p == null)
            {
                Bro.Log.Error("DiffieHellmanProtocol: 'p' factor is not seted " + Side);
                return 0;
            }

            switch (Side)
            {
                case EndpointSide.Client:
                    _a = Random.Instance.Range(1, 999);
                    _A = Power(_g, _a) % _p;
                    return _A.LongValue();
                case EndpointSide.Server:
                    _b = Random.Instance.Range(1, 999);
                    _B = Power(_g, _b) % _p;
                    return _B.LongValue();
            }

            return 0;
        }

        public void SetForeignSecretPart(long v)
        {
            switch (Side)
            {
                case EndpointSide.Client:
                    _B = new BigInteger(v);
                    break;
                case EndpointSide.Server:
                    _A = new BigInteger(v);
                    break;
            }
        }

        public bool IsValid
        {
            get
            {
                switch (Side)
                {
                    case EndpointSide.Client:
                        return _B != null && _a != null && _p != null;
                    case EndpointSide.Server:
                        return _A != null && _b != null && _p != null;
                }

                return false;
            }
        }

        public long GetSecretKey()
        {
            if (!IsValid)
            {
                Bro.Log.Error("DiffieHellmanProtocol: Can not return key, protocol not valid " + Side);
                return 0;
            }

            switch (Side)
            {
                case EndpointSide.Client:
                    return (Power(_B, _a) % _p).LongValue();
                case EndpointSide.Server:
                    return (Power(_A, _b) % _p).LongValue();
            }

            return 0;
        }

        public string GetAESKey()
        {
            return Tools.MD5(GetSecretKey() + _salt);
        }


        private static BigInteger Power(BigInteger number, BigInteger power)
        {
            if (power == 0) return 1;
            var result = number;
            while (power > 1)
            {
                result *= number;
                power--;
            }

            return result;
        }
    }
}