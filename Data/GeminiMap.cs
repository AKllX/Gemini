using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Data
{
    public class GeminiMap
    {
        private List<GeminiPoint> geminiPoints;

        public GeminiMap()
        {
            geminiPoints = new List<GeminiPoint>();
        }

        public void Add(GeminiPoint p)
        {
            geminiPoints.Add(p);
        }

        public GeminiPoint GetPointFromOid(string oid)
        {
            return geminiPoints.FirstOrDefault(x => x.SnmpObjectId == Core.OidLibrary.GetIdFromOid(oid));
        }
    }
}
