using ShopPlatform.UserPages.Main;
using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using ShopPlatform.AdminTools;

namespace ShopPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int ShopId { get; set; }
        public string MainLanguage { get; set; }

        private List<StoreItem> UserItems;

        private string fileName;
        private bool isChanged;

        public MainWindow()
        {
            InitializeComponent();
            MainLanguage = "Русский";
            fileName = string.Empty;
            isChanged = false;
            UserItems = new List<StoreItem>();

            ShopContext shopContext = new ShopContext();
            List<string> shops = shopContext.Shop.Select(s => s.Name).ToList();
            ShopList.ItemsSource = shops.ToArray();
            MainFunctions.Main = this;
            DBItems.Source = new Uri(@"UserPages\Main\MainFoodTypePage.xaml", UriKind.RelativeOrAbsolute);
            this.Resources = MainFunctions.GetLanguageResource();
        }

        private void OpenAddWindow(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow(AdminAction.Add, MainLanguage);
            adminWindow.Show();
        }

        private void OpenDeleteWindow(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow(AdminAction.Delete, MainLanguage);
            adminWindow.Show();
        }

        private void OpenEditWindow(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow(AdminAction.Edit, MainLanguage);
            adminWindow.Show();
        }

        private void ClosingApplication(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void AddItemToList(StoreItem item)
        {
            if (UserItems.Any(i => i.Id == item.Id && i.UnitId == item.UnitId))
            {
                ShopContext shopContext = new ShopContext();
                float shopAmount = (float)shopContext.ShopStore.Where(s => s.ShopID == ShopId && s.FoodID == item.Id).Select(s => s.Amount).First();
                StoreItem fined = UserItems.Find(i => i.Id == item.Id && i.UnitId == item.UnitId);
                if (fined.Amount + fined.UnitStep <= shopAmount)
                {
                    fined.Amount += fined.UnitStep;
                }
            }
            else
            {
                StoreItem newItem = new StoreItem();
                newItem.Name = item.Name;
                newItem.Id = item.Id;
                newItem.Image = item.Image;
                newItem.Unit = item.Unit;
                newItem.UnitId = item.UnitId;
                newItem.UnitStep = item.UnitStep;
                newItem.Amount = item.UnitStep;
                UserItems.Add(newItem);
            }
            UserItemList.ItemsSource = null;
            UserItemList.Items.Clear();
            UserItemList.ItemsSource = UserItems.ToArray();
            isChanged = true;
        }

        public void RemoveItemFromList(StoreItem item)
        {
            if (UserItems.Any(i => i.Id == item.Id && i.UnitId == item.UnitId))
            {
                StoreItem fined = UserItems.Find(i => i.Id == item.Id && i.UnitId == item.UnitId);
                if (fined.Amount - fined.UnitStep > 0)
                {
                    fined.Amount -= fined.UnitStep;
                }
                else
                {
                    UserItems.Remove(fined);
                }
            }
            UserItemList.ItemsSource = UserItems.ToArray();
            isChanged = true;
        }

        public void NavigateItemPage(Page nextPage)
        {
            DBItems.NavigationService.Navigate(nextPage);
        }

        private void ChangeShop(object sender, SelectionChangedEventArgs e)
        {
            if (ShopList.SelectedItem != null)
            {
                string shopName = ShopList.SelectedItem.ToString();
                ShopContext shopContext = new ShopContext();
                ShopId = shopContext.Shop.Where(s => s.Name == shopName).Select(s => s.ShopID).First();
                DBItems.NavigationService.Navigate(new MainFoodTypePage());
                OnAdminButtons();
            }
        }

        private void CreateNewList(object sender, RoutedEventArgs e)
        {
            if (UserItems.Count != 0)
            {
                if (isChanged)
                {
                    if (MessageBox.Show("Do you want to save list?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (listName.Text == "List name")
                        {
                            SaveToFile();
                        }
                        else SaveToFile(listName.Text);
                    }
                }
                UserItems.Clear();
                UserItemList.ItemsSource = null;
                UserItemList.Items.Clear();
                fileName = string.Empty;
                listName.Text = "List name";
            }
        }

        private void OpenList(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".fd";
            bool? result = fileDialog.ShowDialog();
            if (result == true)
            {
                using (FileStream fs = File.OpenRead(fileDialog.FileName))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    UserItems = (List<StoreItem>)formatter.Deserialize(fs);
                    UserItemList.ItemsSource = UserItems.ToArray();
                }
            }
            fileName = fileDialog.FileName;
            isChanged = false;
            listName.Text = "List name";
        }

        private void SaveList(object sender, RoutedEventArgs e)
        {
            if (listName.Text == "List name")
            {
                SaveToFile();
            }
            else SaveToFile(listName.Text);
        }

        private void SaveAsList(object sender, RoutedEventArgs e)
        {
            fileName = string.Empty;
            if (listName.Text == "List name")
            {
                SaveToFile();
            }
            else SaveToFile(listName.Text);
        }

        private void OpenSettingsPage(object sender, RoutedEventArgs e)
        {
            DBItems.NavigationService.Navigate(new SettingsPage());
        }

        private void ClearList(object sender, RoutedEventArgs e)
        {
            UserItems.Clear();
            UserItemList.ItemsSource = null;
            UserItemList.Items.Clear();
        }

        private void SaveToFile(string listName = null)
        {
            bool? result;
            if (fileName == string.Empty)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                if (listName != null)
                {
                    fileDialog.FileName = listName;
                }
                fileDialog.DefaultExt = ".fd";
                result = fileDialog.ShowDialog();
                fileName = fileDialog.FileName;
            }
            else result = true;
            if (result == true)
            {
                using (FileStream fs = File.OpenWrite(fileName))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, UserItems);
                }
            }
            isChanged = false;
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            NavigateItemPage(new MainFoodTypePage());
        }

        public void OnAdminButtons()
        {
            if (ShopId == MainFunctions.AdminShopId)
            {
                AdminPanel.Visibility = Visibility.Visible;
            }
            else AdminPanel.Visibility = Visibility.Collapsed;
        }

        private void PlusItem(object sender, RoutedEventArgs e)
        {
            StoreItem selectedItem = (sender as Button).DataContext as StoreItem;
            AddItemToList(selectedItem);
        }

        private void MinusItem(object sender, RoutedEventArgs e)
        {
            StoreItem selectedItem = (sender as Button).DataContext as StoreItem;
            RemoveItemFromList(selectedItem);
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            StoreItem selectedItem = (sender as Button).DataContext as StoreItem;
            StoreItem fined = UserItems.Find(i => i.Id == selectedItem.Id && i.UnitId == selectedItem.UnitId);
            UserItems.Remove(fined);
            UserItemList.ItemsSource = UserItems.ToArray();
        }
    }

    public enum AdminAction
    {
        Add, Delete, Edit
    }
}
