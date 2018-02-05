using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopPlatform.UserPages.Main
{
    [Serializable]
    public class StoreItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public string Unit { get; set; }
        public int UnitId { get; set; }
        public float UnitStep { get; set; }

        public float Amount { get; set; }
        public string AmountString
        {
            get
            {
                string mainLang = MainFunctions.Main.MainLanguage;
                string atStore = string.Empty;
                switch (mainLang)
                {
                    case "English":
                        atStore = "At store";
                        break;

                    case "Русский":
                        atStore = "На складе";
                        break;
                            
                    default:
                        break;
                }

                return string.Format("{0}: {1} {2}", atStore, Amount, Unit);
            }
        }
    }
}
