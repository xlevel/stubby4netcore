using System.Net.Http;
using System.Collections.Generic;

namespace stubby4netcore.Configuration.Data
{
    public class Request
    {
        public string Url { get; set; }

        //TODO: Support single Method values as a one element list. Might not be possible with YamlDotNet
        //public IEnumerable<string> Method { get; set;} 
        public string Method { get; set; }

        public string Post { get; set; }

        public string File { get; set; }
    }
}