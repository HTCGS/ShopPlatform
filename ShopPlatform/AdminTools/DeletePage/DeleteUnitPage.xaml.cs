using ShopPlatform.AdminTools.AddPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopPlatform.AdminTools.DeletePage
{
    /// <summary>
    /// Interaction logic for DeleteUnitPage.xaml
    /// </summary>
    public partial class DeleteUnitPage : Page
    {
        private string MainLanguage;
        List<UnitPropertis> Units;
        List<string> Languages;

        public DeleteUnitPage()
        {
            InitializeComponent();
        }

        public DeleteUnitPage(string language) : this()
        {
            this.MainLanguage = language;
            Languages = MainFunctions.GetLanguages();
            UnitLanguageBox.ItemsSource = Languages.ToArray();

            RefreshList(MainLanguage);
        }

        private void UnitLanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string unitLanguage = (sender as ComboBox).SelectedItem.ToString();
            RefreshList(unitLanguage);
        }

        private void DeleteUnit(object sender, RoutedEventArgs e)
        {
            if (UnitList.SelectedItem != null)
            {
                ShopContext shopContext = new ShopContext();
                UnitPropertis unitPropertie = (UnitList.SelectedItem as UnitPropertis);
                List<UnitDictionary> unitDictionary = shopContext.UnitDictionary.Where(u => u.Name == unitPropertie.Name).ToList();
                foreach (var item in unitDictionary)
                {
                    shopContext.UnitDictionary.Remove(item);
                }
                Unit deletedUnit = shopContext.Unit.Where(u => u.UnitID == unitPropertie.UnitId).First();
                shopContext.Unit.Remove(deletedUnit);
                shopContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Item is not selected!");
                return;
            }

            if (UnitLanguageBox.SelectedItem != null)
            {
                RefreshList(UnitLanguageBox.SelectedItem.ToString());
            }
            else RefreshList(MainLanguage);
        }

        private void RefreshList(string unitLanguage)
        {
            ShopContext shopContext = new ShopContext();
            Languages = shopContext.Language.Select(l => l.Name).ToList();
            UnitLanguageBox.ItemsSource = Languages.ToArray();

            Units = shopContext.UnitDictionary
                .Join(shopContext.Language, u => u.LanguageID, l => l.LanguageID, (u, l) => new { UnitId = u.UnitID, UnitName = u.Name, Language = l.Name })
                .Where(l => l.Language == unitLanguage)
                .Select(u => new { UnitId = u.UnitId, UnitName = u.UnitName })
                .Join(shopContext.Unit, d => d.UnitId, u => u.UnitID, (d, u) => new { UnitId = u.UnitID, UnitName = d.UnitName, UnitStep = u.UnitStep })
                .Select(u => new UnitPropertis
                {
                    UnitId = u.UnitId,
                    Name = u.UnitName,
                    Step = (float)u.UnitStep
                }).ToList();
            UnitList.ItemsSource = Units.ToArray();
        }
    }
}
