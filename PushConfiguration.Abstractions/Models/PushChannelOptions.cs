using System.Collections.Generic;

namespace PushServer.PushConfiguration.Abstractions.Models
{
    public class PushChannelOptions : Dictionary<string,string>
    {
        public PushChannelOptions() : base()
        {

        }
        public PushChannelOptions(IDictionary<string, string> dictionary) : base(dictionary)
        {

        }
    }
}
