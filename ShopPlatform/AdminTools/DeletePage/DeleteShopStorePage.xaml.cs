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
    /// Interaction logic for DeleteShopStorePage.xaml
    /// </summary>
    public partial class DeleteShopStorePage : Page
    {
        private string MainLanguage;
        private int ShopId;

        private List<string> Languages;
        private List<string> Types;
        private List<ShopStorePropertie> StoreItems;

        public DeleteShopStorePage()
        {
            InitializeComponent();
        }

        public DeleteShopStorePage(string language, int shopId) : this()
        {
            this.MainLanguage = language;
            this.ShopId = shopId;

            Languages = MainFunctions.GetLanguages();
            StoreLanguageBox.ItemsSource = Languages.ToArray();

            Types = MainFunctions.GetFoodTypes(MainLanguage);
            FoodTypeBox.ItemsSource = Types.ToArray();

            RefreshList(MainLanguage);
        }

        private void StoreItemLanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string foodLanguage = (sender as ComboBox).SelectedItem.ToString();
            if (FoodTypeBox.SelectedItem != null)
            {
                string foodType = FoodTypeBox.SelectedItem.ToString();
                RefreshList(foodLanguage, foodType);
            }
            else RefreshList(foodLanguage);
        }

        private void FoodTypeChange(object sender, SelectionChangedEventArgs e)
        {
            string foodType = (sender as ComboBox).SelectedItem.ToString();
            if (StoreLanguageBox.SelectedItem != null)
            {
                string foodLanguage = StoreLanguageBox.SelectedItem.ToString();
                RefreshList(foodLanguage, foodType);
            }
            else RefreshList(MainLanguage, foodType);
        }


        private void DeleteStoreItem(object sender, RoutedEventArgs e)
        {
            ShopContext shopContext = new ShopContext();
            if (StoreList.SelectedItem != null)
            {
                ShopStorePropertie storeItem = StoreList.SelectedItem as ShopStorePropertie;
                ShopStore deletedItem = shopContext.ShopStore
                    .Where(s => s.ShopID == ShopId && s.FoodID == storeItem.FoodId && s.UnitID == storeItem.Unit.UnitId).First();
                shopContext.ShopStore.Remove(deletedItem);
                shopContext.SaveChanges();

                string lang;
                string type = null;
                if (StoreLanguageBox.SelectedItem != null) lang = StoreLanguageBox.SelectedItem.ToString();
                else lang = MainLanguage;
                if (FoodTypeBox.SelectedItem != null) type = FoodTypeBox.SelectedItem.ToString();
                if (type != null) RefreshList(lang, type);
                else RefreshList(lang);
            }
        }

        private void RefreshList(string foodLanguage, string foodType = null)
        {
            ShopContext shopContext = new ShopContext();
            var items = shopContext.ShopStore
                .Join(shopContext.Unit, s => s.UnitID, u => u.UnitID,
                (s, u) => new { FoodId = s.FoodID, Amount = s.Amount, UnitId = s.UnitID, UnitStep = u.UnitStep })
                .Join(shopContext.UnitDictionary, s => s.UnitId, ud => ud.UnitID,
                (s, ud) => new { FoodId = s.FoodId, Amount = s.Amount, UnitId = s.UnitId, UnitStep = s.UnitStep, UnitName = ud.Name })
                .Join(shopContext.Food, s => s.FoodId, f => f.FoodID,
                (s, f) => new { ShopItem = s, FoodTypeId = f.FoodTypeID })
                .Join(shopContext.FoodDictionary, s => s.ShopItem.FoodId, f => f.FoodID,
                (s, f) => new { ShopItem = s.ShopItem, FoodTypeId = s.FoodTypeId, FoodName = f.Name, LanguageId = f.LanguageID })
                .Join(shopContext.Language, s => s.LanguageId, l => l.LanguageID,
                (s, l) => new { ShopItem = s.ShopItem, FoodTypeId = s.FoodTypeId, FoodName = s.FoodName, Language = l.Name })
                .Where(s => s.Language == foodLanguage);

            if (foodType != null)
            {
                int typeId = shopContext.FoodTypeDictionary.Where(t => t.Name == foodType).Select(t => t.FoodTypeID).First();
                StoreItems = items
                    .Where(s => s.FoodTypeId == typeId)
                    .Select(s => new ShopStorePropertie
                    {
                        FoodId = s.ShopItem.FoodId,
                        FoodName = s.FoodName,
                        Amount = (float)s.ShopItem.Amount,
                        Unit = new UnitPropertis
                        {
                            UnitId = s.ShopItem.UnitId,
                            Name = s.ShopItem.UnitName,
                            Step = (float)s.ShopItem.UnitStep
                        }
                    }).ToList();
            }
            else
            {
                StoreItems = items
                    .Select(s => new ShopStorePropertie
                    {
                        FoodId = s.ShopItem.FoodId,
                        FoodName = s.FoodName,
                        Amount = (float)s.ShopItem.Amount,
                        Unit = new UnitPropertis
                        {
                            UnitId = s.ShopItem.UnitId,
                            Name = s.ShopItem.UnitName,
                            Step = (float)s.ShopItem.UnitStep
                        }
                    }).ToList();
            }
            StoreList.ItemsSource = StoreItems.ToArray();
        }
    }
}
