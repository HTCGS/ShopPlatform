using ShopPlatform.UserPages.Main;
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

namespace ShopPlatform.UserPages.Items
{
    /// <summary>
    /// Interaction logic for MainFoodPage.xaml
    /// </summary>
    public partial class MainFoodPage : Page
    {
        private int TypeId;

        private List<StoreItem> Foods;

        public MainFoodPage()
        {
            InitializeComponent();
        }

        public MainFoodPage(int foodTypeId) :this()
        {
            this.TypeId = foodTypeId;

            Foods = MainFunctions.GetStoreFood(MainFunctions.Main.ShopId, MainFunctions.Main.MainLanguage, TypeId);
            FoodList.ItemsSource = Foods.ToArray();
        }

        private void AddFood(object sender, MouseButtonEventArgs e)
        {
            StoreItem selectedItem = (sender as Button).DataContext as StoreItem;
            MainFunctions.Main.AddItemToList(selectedItem);
        }

        private void RemoveFood(object sender, MouseButtonEventArgs e)
        {
            StoreItem selectedItem = (sender as Button).DataContext as StoreItem;
            MainFunctions.Main.RemoveItemFromList(selectedItem);
        }
    }
}
