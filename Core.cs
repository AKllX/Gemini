using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lextm.SharpSnmpLib;
using Automatak.DNP3.Adapter;
using Automatak.DNP3.Interface;
using SnmpSharpNet;
using log4net;

using Gemini.Config;
using Gemini.Database;
using Gemini.Database.Interfaces;
using Gemini.Data.DNP3;
using Gemini.Data.SNMP;

namespace Gemini
{
    public static class Core
    {
        public static IDNP3Manager DNP3Manager { get; set; }
        public static AgentParameters SNMPParameters { get; private set; }
        public static ILog Log;
        public static DatabaseManager DatabaseManager { get; private set; }
        public static Settings Settings { get; private set; }
        public static RtuManager RtuManager { get; private set; }
        public static OidLibrary OidLibrary { get; private set; }

        private const string db = "gemini";
        private const string host = "10.160.211.99";

        public static void Start()
        {
            startLogging();
            startDatabaseConnection();
            startSettings();
            startSNMP();
            OidLibrary = new OidLibrary();
            startDNP3();
            RtuManager = new RtuManager();
            
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

        private static void startLogging()
        {
            //Servidor de Logs
            Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private static void startDatabaseConnection()
        {
            //Iniciando Conexão ao Banco de Dados
            DatabaseManager = new DatabaseManager(ConnectionStringBuilder.BuildMySQL(db, host));
            if (!DatabaseManager.IsConnected())
            {
                Log.Fatal("Erro ao iniciar banco de dados");
                Console.ReadLine();
                Environment.Exit(1);
            }
            else
            {
                Log.InfoFormat("MYSQL - {0}@{1} Conectado", db, host);
            }
        }

        private static void startSettings()
        {
            //Carregando arquivo de configuração
            using (IQueryAdapter adapter = Core.DatabaseManager.GetQueryReactor())
            {
                adapter.SetQuery("SELECT * FROM config LIMIT 1;");
                using (DataTable dt = adapter.getTable())
                {
                    if (dt != null & dt.Rows.Count > 0)
                    {
                        Settings = new Settings
                        {
                            HostAddress = Convert.ToString(dt.Rows[0]["hostaddress"]),
                            SNMPCommunity = Convert.ToString(dt.Rows[0]["snmp_community"]),
                            SNMPVersion = Convert.ToInt32(dt.Rows[0]["snmp_ver"]),
                            SNMPRetry = Convert.ToInt32(dt.Rows[0]["snmp_retry"]),
                            SNMPTimeout = Convert.ToInt32(dt.Rows[0]["snmp_timeout"]),
                            DNP3MasterAddress = Convert.ToInt32(dt.Rows[0]["dnp3_master_address"]),
                            DNP3ConcurrentThreads = Convert.ToInt32(dt.Rows[0]["dnp3_concurrent_threads"]),
                            GatewayPoolingTime = Convert.ToInt32(dt.Rows[0]["gateway_pooling_time"])
                        };
                    }
                    Log.InfoFormat("Hostaddress> {0}", Settings.HostAddress);
                    Log.InfoFormat("SNMPCommunity> {0}", Settings.SNMPCommunity);
                    Log.InfoFormat("SNMPVersion> {0}", Settings.SNMPVersion);
                    Log.InfoFormat("SNMPRetry> {0}", Settings.SNMPRetry);
                    Log.InfoFormat("SNMPTimeout> {0}", Settings.SNMPTimeout);
                    Log.InfoFormat("DNP3MasterAddress _ Default> {0}", Settings.DNP3MasterAddress);
                    Log.InfoFormat("DNP3ConcurrentThreads> {0}", Settings.DNP3ConcurrentThreads);
                }
            }
        }

        private static void startSNMP()
        {
            //Config. SNMP
            //Communities Comuns: cr e public
            SnmpSharpNet.OctetString community = new SnmpSharpNet.OctetString("public");
            SNMPParameters = new AgentParameters(community)
            {
                Version = SnmpVersion.Ver2
            };
        }

        private static void startDNP3()
        {
            //Config. DNP3
            DNP3Manager = DNP3ManagerFactory.CreateManager(4, new PrintingLogAdapter());
        }
    }
}
