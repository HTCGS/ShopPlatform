using ShopPlatform.UserPages.Items;
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

namespace ShopPlatform.UserPages.Main
{
    /// <summary>
    /// Interaction logic for MainFoodTypePage.xaml
    /// </summary>
    public partial class MainFoodTypePage : Page
    {
        public MainFoodTypePage()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            List<StoreItem> items = MainFunctions.GetStoreFoodTypes(MainFunctions.Main.ShopId, MainFunctions.Main.MainLanguage);
            List<StoreItem> duplicated = items.GroupBy(s => s.Name).SelectMany(s => s.Skip(1)).ToList();
            foreach (var item in duplicated)
            {
                items.Remove(item);
            }
            TypeList.ItemsSource = items.ToArray();
        }

        private void TypeClick(object sender, RoutedEventArgs e)
        {
            int selectedItemId = (int)(sender as FrameworkElement).Tag;
            MainFunctions.Main.NavigateItemPage(new MainFoodPage(selectedItemId));
        }
    }
}
