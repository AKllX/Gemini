using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatak.DNP3.Adapter;
using Automatak.DNP3.Interface;

namespace Gemini.Data.DNP3
{
    public class Rtu
    {
        public OutstationStackConfig Config { get; private set; }
        public IOutstation Outstation { get; private set; }
        public IChannel Channel { get; private set; }
        public Rtu(string name, IPEndpoint hostAddres, ushort localDNP3Addres, ushort remoteDNP3Address)
        {
            //Considerando uma RTU por Canal
            Channel = Core.DNP3Manager.AddTCPServer(name, LogLevels.ALL,ServerAcceptMode.CloseExisting, hostAddres.address, hostAddres.port, ChannelListener.Print());

            //Configuração Padrão de RTU é Outstation
            Config = new OutstationStackConfig();
            Config.databaseTemplate = new DatabaseTemplate(4, 0, 1, 1, 1, 1, 1, 0, 0);
            Config.databaseTemplate.analogs[0].clazz = PointClass.Class2;
            Config.outstation.config.allowUnsolicited = false;
            Config.link.remoteAddr = remoteDNP3Address;
            Config.link.localAddr = localDNP3Addres;

            Outstation = Channel.AddOutstation(name, RejectingCommandHandler.Instance, DefaultOutstationApplication.Instance, Config);
            Outstation.Enable();
        }
    }
}
