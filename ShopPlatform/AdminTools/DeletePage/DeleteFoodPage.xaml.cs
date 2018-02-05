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
    /// Interaction logic for DeleteFoodPage.xaml
    /// </summary>
    public partial class DeleteFoodPage : Page
    {
        private string MainLanguage;
        private List<FoodPropertie> Foods;
        private List<string> Types;
        private List<string> Languages;

        public DeleteFoodPage()
        {
            InitializeComponent();
        }

        public DeleteFoodPage(string language) : this()
        {
            this.MainLanguage = language;

            Types = MainFunctions.GetFoodTypes(MainLanguage);
            FoodTypeBox.ItemsSource = Types.ToArray();

            Languages = MainFunctions.GetLanguages();
            FoodLanguageBox.ItemsSource = Languages.ToArray();

            ShopContext shopContext = new ShopContext();
            Foods = shopContext.Food
                .Join(shopContext.FoodDictionary, f => f.FoodID, t => t.FoodID,
                (f, t) => new { FoodId = f.FoodID, FoodName = t.Name, LanguageId = t.LanguageID })
                .Join(shopContext.Language, f => f.LanguageId, l => l.LanguageID,
                (f, l) => new { FoodId = f.FoodId, FoodName = f.FoodName, Language = l.Name })
                .Where(l => l.Language == MainLanguage)
                .Select(ft => new FoodPropertie
                {
                    FoodName = ft.FoodName,
                    FoodId = ft.FoodId
                }).ToList();
            FoodList.ItemsSource = Foods.ToArray();
        }

        private void FoodTypeChange(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                ShopContext shopContext = new ShopContext();
                string foodType = (sender as ComboBox).SelectedItem.ToString();
                int foodTypeId = shopContext.FoodTypeDictionary.Where(fd => fd.Name == foodType).Select(fd => fd.FoodTypeID).First();
                if (FoodLanguageBox.SelectedItem != null)
                {
                    string foodLanguage = FoodLanguageBox.SelectedItem.ToString();
                    Foods = shopContext.Food
                        .Join(shopContext.FoodDictionary, f => f.FoodID, d => d.FoodID,
                        (f, d) => new { FoodId = f.FoodID, FoodTypeId = f.FoodTypeID, FoodName = d.Name, LanguageId = d.LanguageID })
                        .Join(shopContext.Language, f => f.LanguageId, l => l.LanguageID,
                        (f, l) => new { FoodId = f.FoodId, FoodName = f.FoodName, FoodTypeId = f.FoodTypeId, Language = l.Name })
                        .Where(l => l.Language == foodLanguage)
                        .Where(f => f.FoodTypeId == foodTypeId)
                        .Select(ft => new FoodPropertie
                        {
                            FoodName = ft.FoodName,
                            FoodId = ft.FoodId
                        }).ToList();
                }
                else
                {
                    Foods = shopContext.Food
                        .Join(shopContext.FoodDictionary, f => f.FoodID, d => d.FoodID,
                        (f, d) => new { FoodId = f.FoodID, FoodTypeId = f.FoodTypeID, FoodName = d.Name, LanguageId = d.LanguageID })
                        .Join(shopContext.Language, f => f.LanguageId, l => l.LanguageID,
                        (f, l) => new { FoodId = f.FoodId, FoodName = f.FoodName, FoodTypeId = f.FoodTypeId, Language = l.Name })
                        .Where(l => l.Language == MainLanguage)
                        .Where(f => f.FoodTypeId == foodTypeId)
                        .Select(ft => new FoodPropertie
                        {
                            FoodName = ft.FoodName,
                            FoodId = ft.FoodId
                        }).ToList();
                }
                FoodList.ItemsSource = Foods.ToArray();
            }
        }

        private void FoodLanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string foodLanguage = (sender as ComboBox).SelectedItem.ToString();
            RefreshList(foodLanguage);
        }

        private void RefreshList(string foodLanguage = null)
        {
            ShopContext shopContext = new ShopContext();
            if (foodLanguage == null)
            {
                if (FoodLanguageBox.SelectedItem != null)
                {
                    foodLanguage = FoodLanguageBox.SelectedItem.ToString();
                }
                else foodLanguage = MainLanguage;
            }
            if (FoodTypeBox.SelectedItem != null)
            {
                string foodType = FoodTypeBox.SelectedItem.ToString();
                int foodTypeId = shopContext.FoodTypeDictionary.Where(td => td.Name == foodType).Select(td => td.FoodTypeID).First();
                Foods = shopContext.Food
                    .Join(shopContext.FoodDictionary, f => f.FoodID, d => d.FoodID,
                    (f, d) => new { FoodId = f.FoodID, FoodTypeId = f.FoodTypeID, FoodName = d.Name, LanguageId = d.LanguageID })
                    .Join(shopContext.Language, f => f.LanguageId, l => l.LanguageID,
                    (f, l) => new { FoodId = f.FoodId, FoodName = f.FoodName, FoodTypeId = f.FoodTypeId, Language = l.Name })
                    .Where(l => l.Language == foodLanguage)
                    .Where(t => t.FoodTypeId == foodTypeId)
                    .Select(ft => new FoodPropertie
                    {
                        FoodName = ft.FoodName,
                        FoodId = ft.FoodId
                    }).ToList();
            }
            else
            {
                Foods = shopContext.Food
                    .Join(shopContext.FoodDictionary, f => f.FoodID, t => t.FoodID,
                    (f, t) => new { FoodId = f.FoodID, FoodTypeId = f.FoodTypeID, FoodName = t.Name, LanguageId = t.LanguageID })
                    .Join(shopContext.Language, f => f.LanguageId, l => l.LanguageID,
                    (f, l) => new { FoodId = f.FoodId, FoodName = f.FoodName, Language = l.Name })
                    .Where(l => l.Language == foodLanguage)
                    .Select(ft => new FoodPropertie
                    {
                        FoodName = ft.FoodName,
                        FoodId = ft.FoodId
                    }).ToList();
            }
            FoodList.ItemsSource = Foods.ToArray();
        }

        private void DeleteFood(object sender, RoutedEventArgs e)
        {
            if (FoodList.SelectedItem != null)
            {
                ShopContext shopContext = new ShopContext();
                FoodPropertie foodPropertie = (FoodList.SelectedItem as FoodPropertie);
                int foodId = foodPropertie.FoodId;
                List<FoodDictionary> foodDictionary = shopContext.FoodDictionary.Where(d => d.FoodID == foodId).ToList();
                foreach (var item in foodDictionary)
                {
                    shopContext.FoodDictionary.Remove(item);
                }
                Food deletedFood = shopContext.Food.Where(f => f.FoodID == foodId).First();
                shopContext.Food.Remove(deletedFood);
                shopContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Item is not selected!");
                return;
            }
            RefreshList();
        }
    }
}
