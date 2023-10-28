using System;
using System.Collections.Generic;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(List<string>), UniversalParamTypeIndex.StringList)]
    public class StringListParam : ParamsCollection, IObjectParam
    {
        private readonly ArrayParam<StringParam> _value = new ArrayParam<StringParam>(short.MaxValue);

        public StringListParam()
        {
            AddParam(_value);
        }
        
        public List<string> Value
        {
            get
            {
                CheckInitialized();
                var result = new List<string>();
                foreach (var gameItemParam in _value.Params)
                {
                    result.Add(gameItemParam.Value);
                }

                return result;
            }
            set
            {
                foreach (var item in value)
                {
                    var stringParam = NetworkPool.GetParam<StringParam>();
                    stringParam.Value = item;
                    _value.Add(stringParam);
                }
            }
            
        }
        object IObjectParam.Value
        {
            get => Value;
            set => Value = (List<string>) value;
        }
        Type IObjectParam.ValueType => typeof(List<string>);
    }
}