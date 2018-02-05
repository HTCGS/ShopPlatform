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
    /// Interaction logic for DeleteFoodTypePage.xaml
    /// </summary>
    public partial class DeleteFoodTypePage : Page
    {
        private string MainLanguage;
        private List<string> Types;
        private List<string> Languages;

        public DeleteFoodTypePage()
        {
            InitializeComponent();
        }

        public DeleteFoodTypePage(string language): this()
        {
            this.MainLanguage = language;
            Languages = MainFunctions.GetLanguages();
            FoodTypeLanguageBox.ItemsSource = Languages.ToArray();

            RefreshList(MainLanguage);
        }

        private void TypeLanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string typeLanguage = (sender as ComboBox).SelectedItem.ToString();
            RefreshList(typeLanguage);
        }

        private void RefreshList(string typeLanguage)
        {
            ShopContext shopContext = new ShopContext();
            Types = shopContext.FoodTypeDictionary
                .Join(shopContext.Language, t => t.LanguageID, l => l.LanguageID, (t, l) => new { TypeName = t.Name, Language = l.Name })
                .Where(l => l.Language == typeLanguage)
                .Select(t => t.TypeName).ToList();
            FoodTypeList.ItemsSource = Types.ToArray();
        }

        private void DeleteFoodType(object sender, RoutedEventArgs e)
        {
            if (FoodTypeList.SelectedItem != null)
            {
                ShopContext shopContext = new ShopContext();
                string foodType = FoodTypeList.SelectedItem.ToString();
                int foodTypeId = shopContext.FoodTypeDictionary.Where(t => t.Name == foodType).Select(t => t.FoodTypeID).First();
                List<FoodTypeDictionary> foodTypeDictionary = shopContext.FoodTypeDictionary.Where(d => d.FoodTypeID == foodTypeId).ToList();
                foreach (var item in foodTypeDictionary)
                {
                    shopContext.FoodTypeDictionary.Remove(item);
                }
                FoodType deletedType = shopContext.FoodType.Where(f => f.FoodTypeID == foodTypeId).First();
                shopContext.FoodType.Remove(deletedType);
                shopContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Item is not selected!");
                return;
            }
            if(FoodTypeLanguageBox.SelectedItem != null)
            {
                string typeLanguage = FoodTypeLanguageBox.SelectedItem.ToString();
                RefreshList(typeLanguage);
            }
            else RefreshList(MainLanguage);
        }
    }
}
