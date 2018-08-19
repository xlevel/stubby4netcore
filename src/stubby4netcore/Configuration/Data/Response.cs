using System.Collections.Generic;

namespace stubby4netcore.Configuration.Data
{
    public class Response
    {
        public int Status { get; set; }

        public IDictionary<string, string> Headers { get; set; }
    }
}