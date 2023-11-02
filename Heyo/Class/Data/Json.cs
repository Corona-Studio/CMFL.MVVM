using System.Runtime.Serialization;
using Heyo.Class.Helper;

namespace Heyo.Class.Data
{
    [DataContract]
    public class Json
    {
        public new string ToString()
        {
            return JsonHelper.JsonSerializerBySingleData(this);
        }
    }
}