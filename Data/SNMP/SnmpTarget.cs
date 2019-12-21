using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

using SnmpSharpNet;

namespace Gemini.Data.SNMP
{
    public class SnmpTarget
    {
        public IPAddress Address { get; private set; }
        public UdpTarget UdpTarget { get; private set; }
        public Pdu Pdu { get; set; }
        public Dictionary<string,string> Database { get; private set; }
        
        public SnmpTarget(string clientAddress, int clientPort, int timeout, int retries)
        {
            Address = IPAddress.Parse(clientAddress);
            UdpTarget = new UdpTarget(Address, clientPort, timeout, retries);
            Pdu = new Pdu(PduType.Get);
            Database = new Dictionary<string, string>();
        }
        
        public void InsertObject(string labelName, string objectId)
        {
            if(!Database.ContainsKey(labelName))
            {
                Database.Add(labelName, objectId);
                Pdu.VbList.Add(objectId);
            }
        }

        public void UpdateVbList()
        {
            Pdu.VbList.Clear();
            foreach(KeyValuePair<string,string> k in Database)
            {
                Pdu.VbList.Add(k.Value);
            }
        }

        public SnmpV2Packet GetUpdate()
        {
            return (SnmpV2Packet)UdpTarget.Request(Pdu, Core.SNMPParameters);
        }
    }
}
