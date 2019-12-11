using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Data.DNP3
{
    public class RtuManager
    {
        public List<Rtu> Rtus;

        public RtuManager()
        {
            Rtus = new List<Rtu>();
            //TODO:Load all from DB
        }
    }
}
