using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SnmpSharpNet;

using Automatak.DNP3.Adapter;
using Automatak.DNP3.Interface;

using Gemini.Data.SNMP;

namespace Gemini.Data.DNP3
{
    public class Rtu : IDisposable
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public OutstationStackConfig Config { get; private set; }
        public IOutstation Outstation { get; private set; }
        public IChannel Channel { get; private set; }
        public Dictionary<int,int> DNP3ToSNMPBinaries { get; private set; }
        public Dictionary<int, int> DNP3ToSNMPAnalogs { get; private set; }
        public Dictionary<int, int> DNP3ToSNMPCounters { get; private set; }
        public SnmpTarget Target { get; private set; }
        public Timer MonitorThread = null;
        private SnmpPacket lastPacket;
        private GeminiMap geminiMap;



        public Rtu(int id, string name, IPEndpoint clientAddress, IPEndpoint hostAddres, ushort localDNP3Addres, ushort remoteDNP3Address,
            Dictionary<int, int> bins, Dictionary<int, int> analogs, Dictionary<int, int> counters, DatabaseTemplate dt, GeminiMap gmap)
        {
            Id = id;
            Name = name;
            //Considerando uma RTU por Canal
            Channel = Core.DNP3Manager.AddTCPServer(name, LogLevels.ALL,ServerAcceptMode.CloseExisting, hostAddres.address, hostAddres.port, ChannelListener.Print());

            //Configuração Padrão de RTU é Outstation
            Config = new OutstationStackConfig
            {
                databaseTemplate = dt
            };
            Config.link.remoteAddr = remoteDNP3Address;
            Config.link.localAddr = localDNP3Addres;
            DNP3ToSNMPBinaries = bins;
            DNP3ToSNMPAnalogs = analogs;
            DNP3ToSNMPCounters = counters;
            Outstation = Channel.AddOutstation(name, RejectingCommandHandler.Instance, DefaultOutstationApplication.Instance, Config);
            Outstation.Enable();
            geminiMap = gmap;

            Target = new SnmpTarget(clientAddress.address, clientAddress.port, Core.Settings.SNMPTimeout, Core.Settings.SNMPRetry);

            foreach (KeyValuePair<int, int> map in DNP3ToSNMPBinaries)
            {
                Target.InsertObject(Core.OidLibrary.GetLabelFromId(map.Value), Core.OidLibrary.GetOidFromId(map.Value));
            }

            foreach (KeyValuePair<int, int> map in DNP3ToSNMPAnalogs)
            {
                Target.InsertObject(Core.OidLibrary.GetLabelFromId(map.Value), Core.OidLibrary.GetOidFromId(map.Value));
            }

            foreach (KeyValuePair<int, int> map in DNP3ToSNMPCounters)
            {
                Target.InsertObject(Core.OidLibrary.GetLabelFromId(map.Value), Core.OidLibrary.GetOidFromId(map.Value));
            }

            MonitorThread = new Timer(new TimerCallback(Update), null, TimeSpan.FromMilliseconds(10), TimeSpan.FromSeconds(Core.Settings.GatewayPoolingTime));
        }

        void Update(object state)
        {
            SnmpV2Packet result = Target.GetUpdate();

            if(result == null)
            {
                Core.Log.ErrorFormat("Falha GET SNMP RTU {0}", Id);
            }
            else
            {
                if (result.Pdu.ErrorStatus != 0)
                {
                    Core.Log.ErrorFormat("Falha PDU SNMP RTU {0} ErrorStatus {1} Error Code {2}", Id, result.Pdu.ErrorStatus, result.Pdu.ErrorIndex);
                }
                else
                {
                    this.UpdateCache(result);
                }
            }
        }

        void UpdateCache(SnmpPacket result)
        {
            ChangeSet newValues = new ChangeSet();
            GeminiPoint gp;
            bool binValue;
            double analogValue;
            uint counterValue;

            if(lastPacket == null)
            {
                try
                {
                    lastPacket = result;
                    foreach (var x in result.Pdu.VbList)
                    {
                        gp = geminiMap.GetPointFromOid(x.Oid.ToString());
                        if (!x.Value.ToString().Contains("SNMP"))
                        {
                            switch (gp.PointType)
                            {
                                case 'D':
                                    {
                                        if (Convert.ToInt32(x.Value.ToString()) != 1)
                                        {
                                            binValue = false;

                                        }
                                        else
                                        {
                                            binValue = true;
                                        }
                                        newValues.Update(new Binary(binValue, 1, DateTime.Now), gp.Dnp3Addr);
                                        Core.Log.InfoFormat("{2}-> {0} = {1}", Core.OidLibrary.GetLabelFromId(gp.SnmpObjectId), binValue, Name);
                                    }
                                    break;
                                case 'A':
                                    {
                                        analogValue = Convert.ToDouble(x.Value.ToString());
                                        newValues.Update(new Analog(analogValue, 1, DateTime.Now), gp.Dnp3Addr);
                                        Core.Log.InfoFormat("{2}-> {0} = {1}", Core.OidLibrary.GetLabelFromId(gp.SnmpObjectId), analogValue, Name);
                                    }
                                    break;
                                case 'C':
                                    {
                                        counterValue = uint.Parse(x.Value.ToString());
                                        newValues.Update(new Counter(counterValue, 1, DateTime.Now), gp.Dnp3Addr);
                                        Core.Log.InfoFormat("{2}-> {0} = {1}", Core.OidLibrary.GetLabelFromId(gp.SnmpObjectId), counterValue, Name);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Core.Log.Error(ex.ToString());
                }
                finally
                {
                    Outstation.Load(newValues);
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < result.Pdu.VbCount; i++)
                    {
                        var x = result.Pdu.VbList[i];
                        gp = geminiMap.GetPointFromOid(x.Oid.ToString());

                        if (String.Equals(result.Pdu.VbList[i].Value.ToString(),lastPacket.Pdu.VbList[i].Value.ToString()))
                        {
                            Core.Log.InfoFormat("{2}--> {0} = {1}", Core.OidLibrary.GetLabelFromId(gp.SnmpObjectId), result.Pdu.VbList[i].Value.ToString(), Name);
                            continue;
                        }
                        else
                        {
                            switch (gp.PointType)
                            {
                                case 'D':
                                    {
                                        if (Convert.ToInt32(x.Value.ToString()) != 1)
                                        {
                                            binValue = false;
                                        }
                                        else
                                        {
                                            binValue = true;
                                        }
                                        newValues.Update(new Binary(binValue, 1, DateTime.Now), gp.Dnp3Addr);
                                        Core.Log.InfoFormat("{2}--> {0} = {1}", Core.OidLibrary.GetLabelFromId(gp.SnmpObjectId), binValue, Name);
                                    }
                                    break;
                                case 'A':
                                    {
                                        analogValue = Convert.ToDouble(x.Value.ToString());
                                        newValues.Update(new Analog(analogValue, 1, DateTime.Now), gp.Dnp3Addr);
                                        Core.Log.InfoFormat("{2}-> {0} = {1}", Core.OidLibrary.GetLabelFromId(gp.SnmpObjectId), analogValue, Name);
                                    }
                                    break;
                                case 'C':
                                    {
                                        counterValue = uint.Parse(x.Value.ToString());
                                        newValues.Update(new Counter(counterValue, 1, DateTime.Now), gp.Dnp3Addr);
                                        Core.Log.InfoFormat("{2}-> {0} = {1}", Core.OidLibrary.GetLabelFromId(gp.SnmpObjectId), counterValue, Name);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Core.Log.Error(ex.ToString());
                }
            }
        }

        public void Dispose()
        {
            Outstation.Shutdown();
            Channel.Shutdown();
        }
    }
}
