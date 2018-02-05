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

namespace ShopPlatform.AdminTools
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private List<string> Languages;

        public SettingsPage()
        {
            InitializeComponent();

            Languages = MainFunctions.GetLanguages();
            AppLanguage.ItemsSource = Languages.ToArray();
            AppLanguage.Text = MainFunctions.Main.MainLanguage;
            this.Resources = MainFunctions.GetLanguageResource();
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            if (AppLanguage.SelectedItem != null)
            {
                string lang = AppLanguage.SelectedItem.ToString();
                MainFunctions.Main.MainLanguage = lang;
                MainFunctions.Main.Resources = MainFunctions.GetLanguageResource();
                this.Resources = MainFunctions.GetLanguageResource();
            }
        }

        private void StyleChange(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CheckAdminKey(object sender, RoutedEventArgs e)
        {
            if (AdminKey.Text != "")
            {
                string key = AdminKey.Text;
                ShopContext shopContext = new ShopContext();
                Shop adminShop = shopContext.Shop.Where(s => s.AdministationKey.ToString() == key).FirstOrDefault();
                if (adminShop == null)
                {
                    MessageBox.Show("Wrong key!");
                }
                else
                {
                    MainFunctions.AdminShopId = adminShop.ShopID;
                    MessageBox.Show("Admin mode is enabled!");
                    MainFunctions.Main.OnAdminButtons();
                }
            }
        }
    }
}
