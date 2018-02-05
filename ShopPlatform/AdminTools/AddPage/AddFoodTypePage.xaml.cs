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
    /// Interaction logic for AddFoodTypePage.xaml
    /// </summary>
    public partial class AddFoodTypePage : Page
    {
        private string MainLanguage;
        private List<string> Languages;

        private byte[] Image;

        private bool AddedImage;
        private int LanguageNum;

        public AddFoodTypePage()
        {
            InitializeComponent();
        }

        public AddFoodTypePage(string language) :this()
        {
            this.MainLanguage = language;
            AddedImage = false;
            LanguageNum = 1;

            Languages = MainFunctions.GetLanguages();
            FoodTypeNames.Items.Add(new NewLanguage { Languages = Languages });
        }

        private void AddImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "*.png; *.jpeg; *.JPG";
            fileDialog.Filter = "Image files (*.png;*.jpeg;*.JPG)|*.png;*.jpeg;*.JPG|All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                Image = File.ReadAllBytes(fileDialog.FileName);
                FoodTypeImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
                AddedImage = true;
            }
        }

        private void AddLanguage(object sender, RoutedEventArgs e)
        {
            if (LanguageNum < Languages.Count)
            {
                FoodTypeNames.Items.Add(new NewLanguage { Languages = Languages });
                LanguageNum++;
            }
        }

        private void SaveFood(object sender, RoutedEventArgs e)
        {
            if (!AddedImage)
            {
                if (MessageBox.Show("Image is not selected!", "Do you want to choose it?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    return;
                }
            }

            ShopContext shopContext = new ShopContext();
            FoodType foodType = new FoodType();
            if (AddedImage) foodType.Image = Image;
            shopContext.FoodType.Add(foodType);

            List<FoodTypeDictionary> dictionaryList = new List<FoodTypeDictionary>();
            for (int i = 0; i < FoodTypeNames.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter)FoodTypeNames.ItemContainerGenerator.ContainerFromItem(FoodTypeNames.Items[i]);
                ComboBox comboBox = c.ContentTemplate.FindName("NewTypeLanguage", c) as ComboBox;
                TextBox text = c.ContentTemplate.FindName("NewTypeName", c) as TextBox;

                FoodTypeDictionary dictionary = new FoodTypeDictionary();
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

            List<FoodTypeDictionary> duplicates = dictionaryList.GroupBy(d => d.LanguageID).SelectMany(d => d.Skip(1)).ToList();
            if (duplicates.Count != 0)
            {
                MessageBox.Show("Duplicated languages!");
                return;
            }

            foreach (var item in dictionaryList)
            {
                if (shopContext.FoodTypeDictionary.Where(f => f.Name == item.Name && f.LanguageID == item.LanguageID).Count() > 0)
                {
                    MessageBox.Show("Type is existed!");
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

            List<int> foodTypeIdList = shopContext.FoodType.Where(f => f.Image == foodType.Image).Select(f => f.FoodTypeID).ToList();
            foreach (var id in foodTypeIdList)
            {
                int idCount = shopContext.FoodTypeDictionary.Where(fd => fd.FoodTypeID == id).Count();
                if (idCount == 0)
                {
                    foreach (var item in dictionaryList)
                    {
                        item.FoodTypeID = id;
                        shopContext.FoodTypeDictionary.Add(item);
                    }
                    break;
                }
            }
            shopContext.SaveChanges();
            FoodTypeNames.Items.Clear();
            FoodTypeNames.Items.Add(new NewLanguage { Languages = Languages });
        }
    }
}
