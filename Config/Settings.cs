using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Config
{
    public class Settings
    {
        public string HostAddress { get; set; }
        public string SNMPCommunity { get; set; }
        public int SNMPVersion { get; set; }
        public int SNMPRetry { get; set; }
        public int SNMPTimeout { get; set; }
        public int DNP3MasterAddress { get; set; }
        public int DNP3ConcurrentThreads { get; set; }
        public int GatewayPoolingTime { get; set; }
    }
}
