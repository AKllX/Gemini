﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

using SnmpSharpNet;

namespace Gemini.Utility.SNMP
{
    public class SNMPTarget
    {
        public IPAddress Address { get; private set; }
        public UdpTarget Target { get; private set; }
        public Pdu Pdu { get; set; }
        public Dictionary<string,string> Database { get; private set; }
        
        public SNMPTarget(string clientAddress)
        {
            Address = IPAddress.Parse(clientAddress);
            Target = new UdpTarget(Address, 161, 2000, 1);
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
                return (SnmpV2Packet)Target.Request(Pdu, Core.SNMPParameters);
        }
    }
}
