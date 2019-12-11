using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Config
{
    public class Settings
    {
        public string HostAddress { get; private set; }
        public string SNMPCommunity { get; private set; }
        public int SNMPVersion { get; private set; }
        public int SNMPRetry { get; private set; }
        public int SNMPTimeout { get; private set; }
        public int DNP3MasterAddress { get; private set; }
        public int DNP3ConcurrentThreads { get; private set; }
    }
}
