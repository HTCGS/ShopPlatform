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
using System.Windows.Shapes;
using ShopPlatform.AdminTools.AddPage;
using ShopPlatform.AdminTools.DeletePage;
using ShopPlatform.AdminTools.EditPage;

namespace ShopPlatform
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private AdminAction AdminAction { get; set; }
        private string MainLanguage { get; set; }

        public AdminWindow()
        {
            InitializeComponent();
            this.Resources = MainFunctions.GetLanguageResource();
        }

        public AdminWindow(AdminAction action, string language) : this()
        {
            this.AdminAction = action;
            this.Title = AdminAction.ToString();
            actionText.Text = AdminAction.ToString();
            this.MainLanguage = language;
        }

        private void ShopStoreClick(object sender, RoutedEventArgs e)
        {
            if (AdminAction == AdminAction.Add)
            {
                contentFrame.NavigationService.Navigate(new AddShopStorePage(MainLanguage, 1));
            }
            if(AdminAction == AdminAction.Delete)
            {
                contentFrame.NavigationService.Navigate(new DeleteShopStorePage(MainLanguage, 1));
            }
            if (AdminAction == AdminAction.Edit)
            {
                contentFrame.NavigationService.Navigate(new EditShopStorePage(MainLanguage, 1));
            }
        }

        private void FoodClick(object sender, RoutedEventArgs e)
        {
            if (AdminAction == AdminAction.Add)
            {
                contentFrame.NavigationService.Navigate(new AddFoodPage(MainLanguage));
            }
            if (AdminAction == AdminAction.Delete)
            {
                contentFrame.NavigationService.Navigate(new DeleteFoodPage(MainLanguage));
            }
            if (AdminAction == AdminAction.Edit)
            {
                contentFrame.NavigationService.Navigate(new EditFoodPage(MainLanguage));
            }
        }

        private void TypeClick(object sender, RoutedEventArgs e)
        {
            if (AdminAction == AdminAction.Add)
            {
                contentFrame.NavigationService.Navigate(new AddFoodTypePage(MainLanguage));
            }
            if (AdminAction == AdminAction.Delete)
            {
                contentFrame.NavigationService.Navigate(new DeleteFoodTypePage(MainLanguage));
            }
            if (AdminAction == AdminAction.Edit)
            {
                contentFrame.NavigationService.Navigate(new EditFoodTypePage(MainLanguage));
            }
        }

        private void UnitClick(object sender, RoutedEventArgs e)
        {
            if (AdminAction == AdminAction.Add)
            {
                contentFrame.NavigationService.Navigate(new AddUnitPage(MainLanguage));
            }
            if (AdminAction == AdminAction.Delete)
            {
                contentFrame.NavigationService.Navigate(new DeleteUnitPage(MainLanguage));
            }
            if (AdminAction == AdminAction.Edit)
            {
                contentFrame.NavigationService.Navigate(new EditUnitPage(MainLanguage));
            }
        }
    }
}
