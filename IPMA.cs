using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISI_IPMAForm
{
    public class PrevisaoDia
    {
        public string precipitaProb { get; set; }
        public string tMin { get; set; }
        public string tMax { get; set; }
        public string predWindDir { get; set; }
        public int idWeatherType { get; set; }
        public int classWindSpeed { get; set; }
        public string longitude { get; set; }
        public string forecastDate { get; set; }
        public int classPrecInt { get; set; }
        public string latitude { get; set; }
    }

    class PrevisaoIPMA
    {
        public string owner { get; set; }
        public string country { get; set; }
        public PrevisaoDia[] data { get; set; }
        public int globalIdLocal { get; set; }
        public DateTime dataUpdate { get; set; }

        // ---- 
        public string local { get; set; }
    }
}
