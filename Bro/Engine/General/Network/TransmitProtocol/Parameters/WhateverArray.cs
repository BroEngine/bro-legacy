namespace Bro.Network.TransmitProtocol
{
    public class WhateverArray
    {
        public class Sender : ToByteArrayParam
        {
            public Sender(int maxSizeInBytes, bool isOptional) :
                base(new ArrayParam<UniversalParam>(short.MaxValue, isOptional), maxSizeInBytes)
            {
            }

            public object[] Value
            {
                set
                {
                    var arrayParam = (ArrayParam<UniversalParam>) this.Param;
                    for (int i = 0, max = value.Length; i < max; ++i)
                    {
                        arrayParam.Add(new UniversalParam() {Value = value[i]});
                    }
                }
            }
            
            
        }

        public class Receiver : FromByteArrayParam
        {
            public Receiver(bool isOptional) : base(new ArrayParam<UniversalParam>(short.MaxValue, isOptional))
            {
            }

            public object[] Value
            {
                get
                {
                    var parameters = ((ArrayParam<UniversalParam>) this.Param).Params;
                    if (parameters.Count == 0)
                    {
                        return null;
                    }

                    var result = new object[parameters.Count];
                    for (int i = 0, max = parameters.Count; i < max; ++i)
                    {
                        result[i] = parameters[i].Value;
                    }

                    return result;
                }
            }
        }
    }
}