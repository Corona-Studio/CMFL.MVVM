using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CMFL.MVVM.Class.Helper.Other
{
    public static class JsonHelper
    {
        public static string ToJsonString(this object obj, JsonSerializerSettings settings = null)
        {
            if (settings == null)
                settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}