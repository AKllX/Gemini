using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Data.SNMP
{
    public class SnmpObject
    {
        public int Id { get; set; }
        public int EquipmentTypeId { get; set; }
        public string Label { get; set; }
        public string ObjectId { get; set; }
        public string Desc { get; set; }

        public SnmpObject()
        {

        }

        public SnmpObject(int id, int equipId, string lbl, string oid, string desc)
        {
            Id = id;
            EquipmentTypeId = equipId;
            Label = lbl;
            ObjectId = oid;
            Desc = desc;
        }
    }
}
