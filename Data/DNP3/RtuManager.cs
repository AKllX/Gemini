using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatak.DNP3.Interface;

using Gemini.Database;
using Gemini.Database.Adapter;
using Gemini.Database.Interfaces;


namespace Gemini.Data.DNP3
{
    public class RtuManager
    {
        public List<Rtu> Rtus;

        public RtuManager()
        {
            Rtus = new List<Rtu>();
            LoadAll();
        }

        public void LoadAll()
        {
            using (IQueryAdapter adapter = Core.DatabaseManager.GetQueryReactor())
            {
                adapter.SetQuery("SELECT * FROM equipment;");
                using (DataTable dt = adapter.getTable())
                {
                    Core.Log.InfoFormat("{0} RTUs encontradas", dt.Rows.Count);
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            LoadRtu(dr);
                        }
                        catch(Exception ex)
                        {
                            Core.Log.Error(ex.ToString());
                        }
                    }
                }
            }
        }

        void LoadRtu(DataRow equipmentRow)
        {
            Rtu rtu = null;
            int id, snmpPort, type, dnp3port, rtuDnp3Addres, scadaDnp3Address;
            string name, clientAddress;
            ushort maxBin = 0, maxAnalog = 0, maxCounter = 0;

            DatabaseTemplate databaseTemplate;
            GeminiMap GMap = new GeminiMap();
            DataTable DNP3Map = null, Bin = null, Analog = null, Counter = null;
            Dictionary<int, int> bins = new Dictionary<int, int>();
            Dictionary<int, int> analogs = new Dictionary<int, int>();
            Dictionary<int, int> counters = new Dictionary<int, int>();

            id = Convert.ToInt32(equipmentRow["id"]);
            snmpPort = Convert.ToInt32(equipmentRow["snmp_port"]);
            type = Convert.ToInt32(equipmentRow["type"]);
            dnp3port = Convert.ToInt32(equipmentRow["dnp3_port"]);
            name = Convert.ToString(equipmentRow["nome"]);
            clientAddress = Convert.ToString(equipmentRow["ip"]);
            rtuDnp3Addres = Convert.ToInt32(equipmentRow["dnp3_address"]);
            scadaDnp3Address = Convert.ToInt32(equipmentRow["dnp3_master_address"]);

            using (IQueryAdapter adapter = Core.DatabaseManager.GetQueryReactor())
            {
                adapter.SetQuery("SELECT * FROM dnp3_map WHERE equipment_type_id = @tid;");
                adapter.AddParameter("tid", type);
                DNP3Map = adapter.getTable();
            }

            try
            {
                foreach(DataRow dr in DNP3Map.Rows)
                {
                    GMap.Add(new GeminiPoint
                    {
                        Id = Convert.ToInt32(dr["id"]),
                        EquipmentType = Convert.ToInt32(dr["equipment_type_id"]),
                        PointType = Convert.ToChar(dr["point_type"]),
                        Class = Convert.ToInt32(dr["class"]),
                        SnmpObjectId = Convert.ToInt32(dr["snmp_object_id"]),
                        Dnp3Addr = ushort.Parse(dr["dnp3_addr"].ToString())
                    });
                }

                Bin = DNP3Map.AsEnumerable().Where(r => r.Field<String>("point_type") == "D").CopyToDataTable();
                Analog = DNP3Map.AsEnumerable().Where(r => r.Field<String>("point_type") == "A").CopyToDataTable();
                Counter = DNP3Map.AsEnumerable().Where(r => r.Field<String>("point_type") == "C").CopyToDataTable();
            }
            catch (Exception) { }

            foreach(DataRow dr in Bin.Rows)
            {
                maxBin = (ushort)Math.Max(maxBin, Convert.ToInt32(dr["dnp3_addr"]));
            }
            foreach (DataRow dr in Analog.Rows)
            {
                maxAnalog = (ushort)Math.Max(maxAnalog, Convert.ToInt32(dr["dnp3_addr"]));
            }
            foreach (DataRow dr in Bin.Rows)
            {
                maxCounter = (ushort)Math.Max(maxCounter, Convert.ToInt32(dr["dnp3_addr"]));
            }

            maxBin++;
            maxAnalog++;
            maxCounter++;

            databaseTemplate = new DatabaseTemplate(maxBin, 0, maxAnalog, maxCounter, 0, 0, 0, 0, 0);

            if (Bin != null)
            {
                foreach (DataRow dr in Bin.Rows)
                {
                    int cls = Convert.ToInt32(dr["class"]);
                    int dnp3Addr = Convert.ToInt32(dr["dnp3_addr"]);
                    PointClass ptCls;
                    switch (cls)
                    {
                        case 0:
                            ptCls = PointClass.Class0;
                            break;
                        case 1:
                            ptCls = PointClass.Class1;
                            break;
                        case 2:
                            ptCls = PointClass.Class2;
                            break;
                        case 3:
                            ptCls = PointClass.Class3;
                            break;
                        default:
                            ptCls = PointClass.Class0;
                            break;
                    }
                    databaseTemplate.binaries[dnp3Addr].clazz = ptCls;
                    bins.Add(dnp3Addr, Convert.ToInt32(dr["snmp_object_id"]));
                }
            }

            if (Analog != null)
            {
                foreach (DataRow dr in Analog.Rows)
                {
                    int cls = Convert.ToInt32(dr["class"]);
                    int dnp3Addr = Convert.ToInt32(dr["dnp3_addr"]);
                    PointClass ptCls;
                    switch (cls)
                    {
                        case 0:
                            ptCls = PointClass.Class0;
                            break;
                        case 1:
                            ptCls = PointClass.Class1;
                            break;
                        case 2:
                            ptCls = PointClass.Class2;
                            break;
                        case 3:
                            ptCls = PointClass.Class3;
                            break;
                        default:
                            ptCls = PointClass.Class0;
                            break;
                    }
                    databaseTemplate.analogs[dnp3Addr].clazz = ptCls;
                    analogs.Add(dnp3Addr, Convert.ToInt32(dr["snmp_object_id"]));
                }
            }

            if (Counter != null)
            {
                foreach (DataRow dr in Counter.Rows)
                {
                    int cls = Convert.ToInt32(dr["class"]);
                    int dnp3Addr = Convert.ToInt32(dr["dnp3_addr"]);
                    PointClass ptCls;
                    switch (cls)
                    {
                        case 0:
                            ptCls = PointClass.Class0;
                            break;
                        case 1:
                            ptCls = PointClass.Class1;
                            break;
                        case 2:
                            ptCls = PointClass.Class2;
                            break;
                        case 3:
                            ptCls = PointClass.Class3;
                            break;
                        default:
                            ptCls = PointClass.Class0;
                            break;
                    }
                    databaseTemplate.counters[dnp3Addr].clazz = ptCls;
                    counters.Add(dnp3Addr, Convert.ToInt32(dr["snmp_object_id"]));
                }
            }

            rtu = new Rtu(id, name, new IPEndpoint(clientAddress,(ushort)snmpPort), new IPEndpoint("0.0.0.0", (ushort)dnp3port), (ushort)rtuDnp3Addres, (ushort)scadaDnp3Address, bins, analogs, counters, databaseTemplate, GMap );
        }
    }
}
