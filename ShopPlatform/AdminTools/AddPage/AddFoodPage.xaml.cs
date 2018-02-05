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
using System.IO;
using Microsoft.Win32;

namespace ShopPlatform.AdminTools.AddPage
{
    /// <summary>
    /// Interaction logic for AddFoodPage.xaml
    /// </summary>
    public partial class AddFoodPage : Page
    {
        private string MainLanguage;
        private List<string> Types;
        private List<string> Languages;

        private byte[] Image;

        private bool AddedImage;
        private int LanguageNum;

        public AddFoodPage()
        {
            InitializeComponent();
        }

        public AddFoodPage(string language) : this()
        {
            this.MainLanguage = language;
            AddedImage = false;
            LanguageNum = 1;

            Types = MainFunctions.GetFoodTypes(MainLanguage);
            FoodType.ItemsSource = Types.ToArray();

            Languages = MainFunctions.GetLanguages();
            LanguageBox.ItemsSource = Languages.ToArray();
            FoodNames.Items.Add(new NewLanguage { Languages = Languages });
        }

        private void AddImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "*.png; *.jpeg; *.JPG";
            fileDialog.Filter = "Image files (*.png;*.jpeg;*.JPG)|*.png;*.jpeg;*.JPG|All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                Image = File.ReadAllBytes(fileDialog.FileName);
                FoodImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
                AddedImage = true;
            }
        }

        private void AddLanguage(object sender, RoutedEventArgs e)
        {
            if (LanguageNum < Languages.Count)
            {
                FoodNames.Items.Add(new NewLanguage { Languages = Languages });
                LanguageNum++;
            }
        }

        private void SaveFood(object sender, RoutedEventArgs e)
        {
            if (FoodType.SelectedItem == null)
            {
                if (MessageBox.Show(@"Food type is not selected!", "Do you want to choose it?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    return;
                }
            }
            if (!AddedImage)
            {
                if (MessageBox.Show("Image is not selected!", "Do you want to choose it?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    return;
                }
            }
            ShopContext shopContext = new ShopContext();
            Food food = new Food();
            if (AddedImage) food.Image = Image;
            if (FoodType.SelectedItem != null)
            {
                string type = FoodType.SelectedItem.ToString();
                int typeId = shopContext.FoodTypeDictionary.Where(t => t.Name == type).Select(t => t.FoodTypeID).First();
                food.FoodTypeID = typeId;
            }
            shopContext.Food.Add(food);

            List<FoodDictionary> dictionaryList = new List<FoodDictionary>();
            for (int i = 0; i < FoodNames.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter)FoodNames.ItemContainerGenerator.ContainerFromItem(FoodNames.Items[i]);
                ComboBox comboBox = c.ContentTemplate.FindName("NewFoodLanguage", c) as ComboBox;
                TextBox text = c.ContentTemplate.FindName("NewFoodName", c) as TextBox;

                FoodDictionary dictionary = new FoodDictionary();
                if (text.Text == "")
                {
                    continue;
                }
                else
                {
                    dictionary.Name = text.Text;
                }

                if (comboBox.SelectedItem == null)
                {
                    continue;
                }
                else
                {
                    string lang = comboBox.SelectedItem.ToString();
                    int langId = shopContext.Language.Where(l => l.Name == lang).Select(l => l.LanguageID).First();
                    dictionary.LanguageID = langId;
                }
                dictionaryList.Add(dictionary);
            }

            List<FoodDictionary> duplicatesLanguages = dictionaryList.GroupBy(d => d.LanguageID).SelectMany(d => d.Skip(1)).ToList();
            if (duplicatesLanguages.Count != 0)
            {
                MessageBox.Show("Duplicated languages!");
                return;
            }

            foreach (var item in dictionaryList)
            {
                if (shopContext.FoodDictionary.Where(f => f.Name == item.Name && f.LanguageID == item.LanguageID).Count() > 0)
                {
                    MessageBox.Show("Food is existed!");
                    return;
                }
            }

            if (dictionaryList.Count > 0)
            {
                shopContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Some fields is empty!");
                return;
            }

            List<int> foodIdList = shopContext.Food.Where(f => f.FoodTypeID == food.FoodTypeID && f.Image == food.Image).Select(f => f.FoodID).ToList();
            foreach (var id in foodIdList)
            {
                int idCount = shopContext.FoodDictionary.Where(fd => fd.FoodID == id).Count();
                if (idCount == 0)
                {
                    foreach (var item in dictionaryList)
                    {
                        item.FoodID = id;
                        shopContext.FoodDictionary.Add(item);
                    }
                    break;
                }
            }
            shopContext.SaveChanges();
            FoodType.SelectedItem = null;
            FoodType.Text = "Type";
            FoodNames.Items.Clear();
            FoodNames.Items.Add(new NewLanguage { Languages = Languages });
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string lang = (sender as ComboBox).SelectedItem.ToString();
            ShopContext shopContext = new ShopContext();
            Types = MainFunctions.GetFoodTypes(lang);
            FoodType.ItemsSource = Types.ToArray();
        }
    }
}
