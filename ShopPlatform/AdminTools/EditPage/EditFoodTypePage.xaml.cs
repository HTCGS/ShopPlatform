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
    /// Interaction logic for EditFoodTypePage.xaml
    /// </summary>
    public partial class EditFoodTypePage : Page
    {
        private string MainLanguage;

        private List<string> Languages;
        private List<FoodPropertie> Types;

        private List<FoodName> TypeNameList;
        private int LanguageNum;
        private int index;

        public EditFoodTypePage()
        {
            InitializeComponent();
            LanguageNum = 0;
            index = 0;
        }

        public EditFoodTypePage(string language) :this()
        {
            this.MainLanguage = language;

            Languages = MainFunctions.GetLanguages();
            LanguageBox.ItemsSource = Languages.ToArray();

            RefreshList();
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string lang = LanguageBox.SelectedItem.ToString();
            RefreshList(lang);
        }

        private void AddLanguage(object sender, RoutedEventArgs e)
        {
            if (LanguageNum < Languages.Count)
            {
                TypeNameList.Add(new FoodName { Languages = Languages, Index = index });
                TypeNames.ItemsSource = TypeNameList.ToArray();
                LanguageNum++;
                index++;
            }
        }

        private void DeleteTypeName(object sender, RoutedEventArgs e)
        {
            int elementindex = (int)(sender as FrameworkElement).Tag;
            foreach (var item in TypeNameList)
            {
                if (item.Index == elementindex)
                {
                    TypeNameList.Remove(item);
                    TypeNames.ItemsSource = TypeNameList.ToArray();
                    break;
                }
            }
            LanguageNum--;
        }

        private void DisplayTypes(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int typeId = (e.AddedItems[0] as FoodPropertie).FoodId;
                ShopContext shopContext = new ShopContext();
                TypeNameList = shopContext.FoodTypeDictionary
                    .Where(t => t.FoodTypeID == typeId)
                    .Select(t => new { TypeName = t.Name, LanguageId = t.LanguageID })
                    .Join(shopContext.Language, t => t.LanguageId, l => l.LanguageID,
                    (t, l) => new FoodName { Name = t.TypeName, NameLanguage = l.Name, Languages = Languages }).ToList();
                foreach (var item in TypeNameList)
                {
                    item.Index = index;
                    index++;
                }
                LanguageNum = TypeNameList.Count;
                TypeNames.ItemsSource = TypeNameList.ToArray();
            }
            NamePanel.Visibility = Visibility.Visible;
        }

        private void SaveType(object sender, RoutedEventArgs e)
        {
            if (TypeList.SelectedItem != null)
            {
                int typeId = (TypeList.SelectedItem as FoodPropertie).FoodId;
                ShopContext shopContext = new ShopContext();
                List<FoodTypeDictionary> DBNames = shopContext.FoodTypeDictionary
                    .Where(d => d.FoodTypeID == typeId).ToList();

                List<FoodTypeDictionary> dictionaryList = new List<FoodTypeDictionary>();
                for (int i = 0; i < TypeNames.Items.Count; i++)
                {
                    ContentPresenter c = (ContentPresenter)TypeNames.ItemContainerGenerator.ContainerFromItem(TypeNames.Items[i]);
                    ComboBox comboBox = c.ContentTemplate.FindName("TypeLanguage", c) as ComboBox;
                    TextBox text = c.ContentTemplate.FindName("TypeName", c) as TextBox;

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
                        string userLang = comboBox.SelectedItem.ToString();
                        int userLangId = shopContext.Language.Where(l => l.Name == userLang).Select(l => l.LanguageID).First();
                        dictionary.LanguageID = userLangId;
                        dictionary.FoodTypeID = typeId;
                    }
                    dictionaryList.Add(dictionary);
                }

                List<FoodTypeDictionary> duplicates = dictionaryList.GroupBy(d => d.LanguageID).SelectMany(d => d.Skip(1)).ToList();
                if (duplicates.Count != 0)
                {
                    MessageBox.Show("Duplicated languages!");
                    return;
                }

                if (dictionaryList.Count > 0)
                {
                    shopContext.FoodTypeDictionary.RemoveRange(DBNames);
                    shopContext.FoodTypeDictionary.AddRange(dictionaryList);
                    shopContext.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Some fields is empty!");
                    return;
                }
                RefreshList();
            }
        }

        private void RefreshList(string language = null)
        {
            ShopContext shopContext = new ShopContext();
            if (language == null)
            {
                if (LanguageBox.SelectedItem != null)
                {
                    language = LanguageBox.SelectedItem.ToString();
                }
                else language = MainLanguage;
            }

            Types = shopContext.FoodTypeDictionary
                .Join(shopContext.Language, t => t.LanguageID, l => l.LanguageID,
                (t, l) => new { TypeName = t.Name, TypeId = t.FoodTypeID, Language = l.Name })
                .Where(l => l.Language == language)
                .Select(t => new FoodPropertie { FoodName = t.TypeName, FoodId = t.TypeId }).ToList();
            TypeList.ItemsSource = Types.ToArray();

            TypeNames.ItemsSource = null;
            NamePanel.Visibility = Visibility.Collapsed;
        }
    }
}
