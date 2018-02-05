using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopPlatform.AdminTools.AddPage
{
    class UnitPropertis
    {
        public int UnitId { get; set; }
        public float Step { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Step, Name);
        }
    }
}
