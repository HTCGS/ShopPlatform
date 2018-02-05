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

namespace ShopPlatform.AdminTools.AddPage
{
    /// <summary>
    /// Interaction logic for AddShopStorePage.xaml
    /// </summary>
    public partial class AddShopStorePage : Page
    {
        private string MainLanguage;
        private int ShopId;

        private List<string> Types;
        private List<string> Foods;
        private List<UnitPropertis> Units;
        private List<string> Languages;

        public AddShopStorePage()
        {
            InitializeComponent();
        }

        public AddShopStorePage(string language, int shopId) : this()
        {
            this.MainLanguage = language;
            this.ShopId = shopId;

            Languages = MainFunctions.GetLanguages();
            LanguageBox.ItemsSource = Languages.ToArray();
            RefreshList(MainLanguage);
        }

        private void SaveStoreItem(object sender, RoutedEventArgs e)
        {
            if (FoodBox.SelectedItem == null)
            {
                MessageBox.Show("Food is not selected!");
                return;
            }
            if (FoodUnitBox.SelectedItem == null)
            {
                MessageBox.Show("Unit is not selected!");
                return;
            }
            if (FoodAmount.Text == "")
            {
                MessageBox.Show("Food amount field is empty!");
                return;
            }

            ShopContext shopContext = new ShopContext();
            string foodName = FoodBox.SelectedItem.ToString();
            int foodId = shopContext.FoodDictionary.Where(f => f.Name == foodName).Select(f => f.FoodID).First();
            UnitPropertis unit = FoodUnitBox.SelectedItem as UnitPropertis;
            List<int> unitIdList = shopContext.UnitDictionary.Where(u => u.Name == unit.Name).Select(u => u.UnitID).ToList();
            int unitId = shopContext.Unit.Where(u => unitIdList.Any(d => d == u.UnitID))
                .Where(u => u.UnitStep == unit.Step).Select(u => u.UnitID).First();
            float amount = Convert.ToSingle(FoodAmount.Text);

            ShopStore shopStore = new ShopStore();
            shopStore.ShopID = ShopId;
            shopStore.FoodID = foodId;
            shopStore.Amount = amount;
            shopStore.UnitID = unitId;

            if (shopContext.ShopStore.Where(s => s.FoodID == shopStore.FoodID && s.UnitID == shopStore.UnitID).Count() > 0)
            {
                MessageBox.Show("Element is existed!");
                return;
            }

            shopContext.ShopStore.Add(shopStore);
            shopContext.SaveChanges();
            FoodTypeBox.SelectedItem = null;
            FoodBox.SelectedItem = null;
            FoodUnitBox.SelectedItem = null;
            FoodTypeBox.Text = "Type";
            FoodBox.Text = "Food";
            FoodUnitBox.Text = "Unit";
            FoodAmount.Text = "";
        }

        private void InputValidation(object sender, TextCompositionEventArgs e)
        {
            float input;
            e.Handled = !(float.TryParse(e.Text.ToString(), out input) || (e.Text.ToString() == "."));
        }

        private void TypeSelection(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                string newType = (sender as ComboBox).SelectedItem.ToString();
                ShopContext shopContext = new ShopContext();
                int typeId = shopContext.FoodTypeDictionary.Where(t => t.Name == newType).Select(t => t.FoodTypeID).First();
                List<int> typedFoodId = shopContext.Food.Where(f => f.FoodTypeID == typeId).Select(f => f.FoodID).ToList();

                string lang = string.Empty;
                if (LanguageBox.SelectedItem != null)
                {
                    lang = LanguageBox.SelectedItem.ToString();
                }
                else lang = MainLanguage;

                Foods = shopContext.FoodDictionary
                    .Where(f => typedFoodId.Any(tf => tf == f.FoodID))
                    .Join(shopContext.Language, f => f.LanguageID, l => l.LanguageID, (f, l) => new { foodName = f.Name, Language = l.Name })
                    .Where(l => l.Language == lang).Select(f => f.foodName.ToString()).ToList();
                FoodBox.ItemsSource = Foods.ToArray();
                FoodBox.Text = "Food";
            }
        }

        private void UnitSelection(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                UnitPropertis selectedUnit = (sender as ComboBox).SelectedItem as UnitPropertis;
                FoodUnitBox.Text = selectedUnit.ToString();
            }
        }

        private void LanguageSelection(object sender, SelectionChangedEventArgs e)
        {
            string itemsLanguage = (sender as ComboBox).SelectedItem.ToString();
            RefreshList(itemsLanguage);
        }

        private void RefreshList(string itemLanguage)
        {
            ShopContext shopContext = new ShopContext();
            Types = MainFunctions.GetFoodTypes(itemLanguage);
            FoodTypeBox.ItemsSource = Types.ToArray();

            Foods = shopContext.FoodDictionary
                .Join(shopContext.Language, f => f.LanguageID, l => l.LanguageID, (f, l) => new { foodName = f.Name, Language = l.Name })
                .Where(l => l.Language == itemLanguage).Select(f => f.foodName.ToString()).ToList();
            FoodBox.ItemsSource = Foods.ToArray();

            Units = shopContext.UnitDictionary
                .Join(shopContext.Language, u => u.LanguageID, l => l.LanguageID, (u, l) => new { UnitId = u.UnitID, UnitName = u.Name, Language = l.Name })
                .Where(l => l.Language == itemLanguage).Select(u => new { UnitId = u.UnitId, UnitName = u.UnitName.ToString() })
                .Join(shopContext.Unit, n => n.UnitId, u => u.UnitID, (n, u) => new UnitPropertis { Step = (float)u.UnitStep, Name = n.UnitName }).ToList();
            FoodUnitBox.ItemsSource = Units.ToArray();
        }
    }
}
