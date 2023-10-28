using System;
using System.Collections.Generic;
using Bro.Network.TransmitProtocol;

namespace Bro.Toolbox
{
    [Serializable, TypeBinder("localization_storage")]
    public class LocalizationSubStorage : SyncSingleConfigStorage<Dictionary<string,string>>
    {

    }
}