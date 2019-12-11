using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SnmpSharpNet;

using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Mib;

using Automatak.DNP3.Adapter;
using Automatak.DNP3.Interface;

using Gemini.Data.DNP3;
using Gemini.SNMP;



namespace Gemini
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            Core.Start();
            ChangeSet changeSet;
            int currentValue;
            SnmpV2Packet result = null;
            IPEndpoint myEndPoint = new IPEndpoint("0.0.0.0", 20500);
            Rtu rtu = new Rtu("Teste DNP3", myEndPoint, 2, 1);

            SnmpTarget retAbelSantana = new SnmpTarget("10.7.5.150");
            retAbelSantana.InsertObject("batteryTemperaturesValue", ".1.3.6.1.4.1.12148.10.3.4.1.2.2");
            try
            {
                result = retAbelSantana.GetUpdate();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            while (true)
            {
                if (result != null)
                {
                    changeSet = new ChangeSet();
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        Core.Log.ErrorFormat("Error in SNMP reply. Error {0} index {1}",
                            result.Pdu.ErrorStatus,
                            result.Pdu.ErrorIndex);
                    }
                    else
                    {
                        //log.DebugFormat("Tipo de PDU: {0}", result.Pdu.VbList[0].Type);
                        currentValue = Convert.ToInt32(result.Pdu.VbList[0].Value.ToString());
                        changeSet.Update(new Analog(currentValue, 1, DateTime.Now), 0);
                        Core.Log.Info("Atualizando valor da analógica para " + currentValue);
                        rtu.Outstation.Load(changeSet);
                    }
                }
                else
                {
                    Console.WriteLine("Erro SNMP Nulo");
                }
                Thread.Sleep(15000);
            }
        }
    }
}
