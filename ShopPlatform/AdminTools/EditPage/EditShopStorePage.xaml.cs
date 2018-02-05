using ShopPlatform.AdminTools.AddPage;
using ShopPlatform.AdminTools.DeletePage;
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

namespace ShopPlatform.AdminTools.EditPage
{
    /// <summary>
    /// Interaction logic for EditShopStorePage.xaml
    /// </summary>
    public partial class EditShopStorePage : Page
    {
        private int ShopId;
        private string MainLanguage;

        private List<string> Languages;
        private List<string> Types;
        private List<ShopStorePropertie> StoreItems;


        public EditShopStorePage()
        {
            InitializeComponent();
        }

        public EditShopStorePage(string language, int shopId) : this()
        {
            this.MainLanguage = language;
            this.ShopId = shopId;

            Languages = MainFunctions.GetLanguages();
            LanguageBox.ItemsSource = Languages.ToArray();

            Types = MainFunctions.GetFoodTypes(MainLanguage);
            TypeBox.ItemsSource = Types.ToArray();
            RefreshList(MainLanguage);
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string foodLanguage = (sender as ComboBox).SelectedItem.ToString();
            if (TypeBox.SelectedItem != null)
            {
                string foodType = TypeBox.SelectedItem.ToString();
                RefreshList(foodLanguage, foodType);
            }
            else RefreshList(foodLanguage);
        }

        private void TypeChange(object sender, SelectionChangedEventArgs e)
        {
            string foodType = (sender as ComboBox).SelectedItem.ToString();
            if (LanguageBox.SelectedItem != null)
            {
                string foodLanguage = LanguageBox.SelectedItem.ToString();
                RefreshList(foodLanguage, foodType);
            }
            else RefreshList(MainLanguage, foodType);
        }

        private void DisplayStore(object sender, SelectionChangedEventArgs e)
        {
            if (StoreList.SelectedItem != null)
            {
                float amount = (StoreList.SelectedItem as ShopStorePropertie).Amount;
                StoreAmount.Text = amount.ToString();
                NamePanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveStore(object sender, RoutedEventArgs e)
        {
            if(StoreList.SelectedItem != null)
            {
                ShopStorePropertie itemProperty = StoreList.SelectedItem as ShopStorePropertie;
                ShopContext shopContext = new ShopContext();
                ShopStore editedItem = shopContext.ShopStore
                    .Where(s => s.ShopID == ShopId && s.FoodID == itemProperty.FoodId && s.UnitID == itemProperty.Unit.UnitId).First();
                float userAmount = Convert.ToSingle(StoreAmount.Text);
                if(userAmount != editedItem.Amount)
                {
                    ShopStore newItem = new ShopStore();
                    newItem.ShopID = editedItem.ShopID;
                    newItem.FoodID = editedItem.FoodID;
                    newItem.Amount = userAmount;
                    newItem.UnitID = editedItem.UnitID;
                    shopContext.ShopStore.Remove(editedItem);
                    shopContext.ShopStore.Add(newItem);
                    shopContext.SaveChanges();
                }
            }
            NamePanel.Visibility = Visibility.Collapsed;
            string lang;
            string type;
            if (LanguageBox.SelectedItem != null)
            {
                lang = LanguageBox.SelectedItem.ToString();
            }
            else lang = MainLanguage;
            if (TypeBox.SelectedItem != null)
            {
                type = TypeBox.SelectedItem.ToString();
                RefreshList(lang, type);
            }
            else RefreshList(lang);
        }

        private void RefreshList(string language, string type = null)
        {
            ShopContext shopContext = new ShopContext();
            var items = shopContext.ShopStore
                .Where(s => s.ShopID == ShopId)
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
                .Where(s => s.Language == language);

            if (type != null)
            {
                int typeId = shopContext.FoodTypeDictionary.Where(t => t.Name == type).Select(t => t.FoodTypeID).First();
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
