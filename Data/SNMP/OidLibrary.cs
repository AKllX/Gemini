using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gemini.Database.Interfaces;

namespace Gemini.Data.SNMP
{
    public class OidLibrary
    {
        public List<SnmpObject> Cache;

        public OidLibrary()
        {
            Cache = new List<SnmpObject>();
            using (IQueryAdapter adapter = Core.DatabaseManager.GetQueryReactor())
            {
                int c = 0;
                adapter.SetQuery("SELECT * FROM snmp_map;");
                using(DataTable map = adapter.getTable())
                {
                    foreach(DataRow row in map.Rows)
                    {
                        c++;
                        Cache.Add(new SnmpObject
                        {
                            Id = Convert.ToInt32(row["id"]),
                            EquipmentTypeId = Convert.ToInt32(row["equipment_type_id"]),
                            Label = Convert.ToString(row["label"]),
                            ObjectId = Convert.ToString(row["object_id"]),
                            Desc = Convert.ToString(row["desc"])
                        });
                    }
                }
                Core.Log.InfoFormat("Biblioteca SNMP: {0} OIDs carregados", c);
            }
        }

        public string GetLabelFromId(int id)
        {
            if(Cache != null)
            {
                return Cache.Single(x => x.Id == id).Label;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public string GetOidFromId(int id)
        {
            if (Cache != null)
            {
                return Cache.Single(x => x.Id == id).ObjectId;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public int GetIdFromOid(string oid)
        {
            if (Cache != null)
            {
                return Cache.Single(x => x.ObjectId == ("." + oid)).Id;
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
