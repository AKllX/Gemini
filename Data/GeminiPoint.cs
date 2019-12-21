using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Data
{
    public class GeminiPoint
    {
        public int Id { get; set; }
        public int EquipmentType { get; set; }
        public char PointType { get; set; }
        public int Class { get; set; }
        public int SnmpObjectId { get; set; }
        public ushort Dnp3Addr { get; set; }
    }
}
