using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bro.Network.TransmitProtocol
{   
    public class ParamsCollection : BaseParam
    {
        //CheckInitialized();
        public List<BaseParam> Params => _params;

        private readonly List<BaseParam> _params = new List<BaseParam>();

        private static readonly byte[] ParamExistingFlag = 
        {
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128
        };

        private string _ownerData;
        public override string OwnerData
        {
            protected get { return _ownerData;}
            set
            {
                _ownerData = value;
                foreach (var p in _params)
                {
                    p.OwnerData = _ownerData;
                }
            }
        }
        
        public ParamsCollection(bool isOptional = false) : base(isOptional)
        {
            #if UNITY_EDITOR
            _ownerData = GetType().ToString();
            #endif
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInvalidParam(BaseParam p)
        {
            Bro.Log.Error($"Param is invalid {p.GetType()} owner = {OwnerData} data = {(p == null ? "" : p.DebugData)}");
        }

        public override bool IsInitialized
        {
            get
            {
                for (int i = 0, max = _params.Count; i < max; ++i)
                {
                    if (!_params[i].IsOptional && !_params[i].IsInitialized)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override bool IsValid
        {
            get
            {
                if (IsOptional)
                {
                    return true;
                }

                for (int i = 0, max = _params.Count; i < max; ++i)
                {
                    if (!_params[i].IsValid)
                    {
                        LogInvalidParam(_params[i]);
                        return false;
                    }
                }

                return true;
            }
        }


        private int OptionalParamsCount
        {
            get
            {
                int optionalParametersCount = 0;
                for (int i = 0, max = _params.Count; i < max; ++i)
                {
                    if (_params[i].IsOptional)
                    {
                        ++optionalParametersCount;
                    }
                }

                return optionalParametersCount;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddParam(BaseParam param)
        {
            param.OwnerData = OwnerData;
            _params.Add(param);
        }

        public override void Write(IWriter writer)
        {
            var optionalParametersCount = OptionalParamsCount;
            if (optionalParametersCount > 0)
            {
                int bytesToStoreParamsExistingData = (optionalParametersCount - 1) / 8 + 1;
                byte[] paramsExistingData = new byte[bytesToStoreParamsExistingData];

                int optParamIndex = 0;
                for (int i = 0, max = _params.Count; i < max; ++i)
                {
                    if (_params[i].IsOptional)
                    {
                        if (_params[i].IsInitialized)
                        {
                            paramsExistingData[optParamIndex / 8] |= ParamExistingFlag[optParamIndex % 8];
                        }

                        ++optParamIndex;
                    }
                }

                writer.Write(paramsExistingData);
            }

            BaseParam p;
            for (int i = 0, max = _params.Count; i < max; ++i)
            {
                p = _params[i];
                if (p.IsOptional)
                {
                    if (p.IsInitialized)
                    {
                        _params[i].Write(writer);
                    }
                }
                else
                {
                    if (!p.IsInitialized)
                    {
                        var dataString = (p != null) ? (string.IsNullOrEmpty(p.DebugData) ? "<unknown>" : p.DebugData) : "<null>";
                        Bro.Log.Error($"Param is not initialized in {OwnerData}; type = {p.GetType()}; data = {dataString}");
                        //throw new System.ArgumentException();
                    }

                    p.Write(writer);
                }
            }
        }

        public override void Read(IReader reader)
        {
            var optionalParametersCount = OptionalParamsCount;
            byte[] paramsExistingData = null;
            if (optionalParametersCount > 0)
            {
                int bytesToStoreParamsExistingData = (optionalParametersCount - 1) / 8 + 1;
                reader.Read(out paramsExistingData, bytesToStoreParamsExistingData);
            }

            var optParamIndex = 0;
            for (int i = 0, max = _params.Count; i < max; ++i)
            {
                
                if (_params[i].IsOptional)
                {
                    var isParamExist = (paramsExistingData[optParamIndex / 8] & ParamExistingFlag[optParamIndex % 8]) != 0;
                    
                    if (isParamExist)
                    {
                        _params[i].Read(reader);
                    }

                 
                    ++optParamIndex;
                }
                else
                {
                    _params[i].Read(reader);
                }
            }
        }

        public override void Cleanup()
        {
            for (int i = 0; i < _params.Count; i++)
            {
                _params[i].Cleanup();
            }
            
            base.Cleanup();
        }
    }
}
