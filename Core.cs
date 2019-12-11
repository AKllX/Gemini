using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lextm.SharpSnmpLib;
using Automatak.DNP3.Adapter;
using Automatak.DNP3.Interface;
using SnmpSharpNet;
using log4net;

namespace Gemini
{
    public static class Core
    {
        public static IDNP3Manager DNP3Manager { get; set; }
        public static AgentParameters SNMPParameters { get; private set; }
        public static ILog Log;

        public static void Start()
        {
            //INICIANDO SERVIDOR DE LOGS
            Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //INICIANDO CONFIGURAÇÃO BÁSICA DE SNMP
            //Communities Comuns: cr e public
            SnmpSharpNet.OctetString community = new SnmpSharpNet.OctetString("public");
            SNMPParameters = new AgentParameters(community)
            {
                Version = SnmpVersion.Ver2
            };

            //INICIANDO INTERFACE DNP3
            DNP3Manager = DNP3ManagerFactory.CreateManager(4, new PrintingLogAdapter());
        }

        public static AgentParameters GetAgentParameters()
        {
            if(SNMPParameters != null)
            {
                return SNMPParameters;
            }
            else
            {
                throw new NullReferenceException("Parâmetro de SNMP Nulo");
            }
        }
    }
}
