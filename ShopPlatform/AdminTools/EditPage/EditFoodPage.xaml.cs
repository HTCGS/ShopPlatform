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
    /// Interaction logic for EditFoodPage.xaml
    /// </summary>
    public partial class EditFoodPage : Page
    {
        private string MainLanguage;

        private List<string> Languages;
        private List<string> Types;
        private List<FoodPropertie> Foods;

        private List<FoodName> FoodNameList;
        private int LanguageNum;
        private int index;


        public EditFoodPage()
        {
            InitializeComponent();
            LanguageNum = 0;    
            index = 0;
        }

        public EditFoodPage(string language) : this()
        {
            this.MainLanguage = language;

            ShopContext shopContext = new ShopContext();
            Languages = MainFunctions.GetLanguages();
            FoodLanguageBox.ItemsSource = Languages.ToArray();

            Types = MainFunctions.GetFoodTypes(MainLanguage);
            FoodTypeBox.ItemsSource = Types.ToArray();

            RefreshList();
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string language = (sender as ComboBox).SelectedItem.ToString();
            Types = MainFunctions.GetFoodTypes(language);
            FoodTypeBox.ItemsSource = Types.ToArray();
            FoodTypeBox.Text = "Type";
            RefreshList(language);
        }

        private void FoodTypeSortChange(object sender, SelectionChangedEventArgs e)
        {
            if (FoodTypeBox.SelectedItem != null)
            {
                RefreshList();
            }
        }

        private void DeleteFoodName(object sender, RoutedEventArgs e)
        {
            int elementindex = (int)(sender as FrameworkElement).Tag;
            foreach (var item in FoodNameList)
            {
                if (item.Index == elementindex)
                {
                    FoodNameList.Remove(item);
                    FoodNames.ItemsSource = FoodNameList.ToArray();
                    break;
                }
            }
            LanguageNum--;
        }

        private void AddLanguage(object sender, RoutedEventArgs e)
        {
            if (LanguageNum < Languages.Count)
            {
                FoodNameList.Add(new FoodName { Languages = Languages, Index = index });
                FoodNames.ItemsSource = FoodNameList.ToArray();
                LanguageNum++;
                index++;
            }
        }

        private void SaveFood(object sender, RoutedEventArgs e)
        {
            if (FoodList.SelectedItem != null)
            {
                int foodId = (FoodList.SelectedItem as FoodPropertie).FoodId;
                ShopContext shopContext = new ShopContext();

                string lang;
                int langId;
                if (FoodLanguageBox.SelectedItem != null)
                {
                    lang = FoodLanguageBox.SelectedItem.ToString();
                }
                else lang = MainLanguage;
                langId = shopContext.Language.Where(l => l.Name == lang).Select(l => l.LanguageID).First();

                int? foodTypeId = shopContext.Food.Where(f => f.FoodID == foodId).Select(f => f.FoodTypeID).First();
                string foodTypeName = string.Empty;
                if (foodTypeId.HasValue)
                {
                    foodTypeName = shopContext.FoodTypeDictionary
                        .Where(t => t.FoodTypeID == foodTypeId && t.LanguageID == langId)
                        .Select(t => t.Name).FirstOrDefault();
                }

                List<FoodDictionary> DBNames = shopContext.FoodDictionary
                    .Where(d => d.FoodID == foodId).ToList();

                Food Food = shopContext.Food.Where(f => f.FoodID == foodId).First();
                string DBFoodType = FoodTypeChangeBox.Text;
                if (foodTypeName != DBFoodType)
                {
                    foodTypeId = shopContext.FoodTypeDictionary.Where(t => t.Name == DBFoodType).Select(t => t.FoodTypeID).First();
                    Food.FoodTypeID = foodTypeId;
                }

                List<FoodDictionary> dictionaryList = new List<FoodDictionary>();
                for (int i = 0; i < FoodNames.Items.Count; i++)
                {
                    ContentPresenter c = (ContentPresenter)FoodNames.ItemContainerGenerator.ContainerFromItem(FoodNames.Items[i]);
                    ComboBox comboBox = c.ContentTemplate.FindName("FoodLanguage", c) as ComboBox;
                    TextBox text = c.ContentTemplate.FindName("FoodName", c) as TextBox;

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
                        string userLang = comboBox.SelectedItem.ToString();
                        int userLangId = shopContext.Language.Where(l => l.Name == userLang).Select(l => l.LanguageID).First();
                        dictionary.LanguageID = userLangId;
                        dictionary.FoodID = foodId;
                    }
                    dictionaryList.Add(dictionary);
                }

                List<FoodDictionary> duplicates = dictionaryList.GroupBy(d => d.LanguageID).SelectMany(d => d.Skip(1)).ToList();
                if (duplicates.Count != 0)
                {
                    MessageBox.Show("Duplicated languages!");
                    return;
                }

                if (dictionaryList.Count > 0)
                {
                    shopContext.FoodDictionary.RemoveRange(DBNames);
                    shopContext.FoodDictionary.AddRange(dictionaryList);
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
            FoodNames.ItemsSource = null;
            NamePanel.Visibility = Visibility.Collapsed;
            FoodTypeChangeBox.Text = "Food type";
        }

        private void DisplayFoodProperties(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int foodId = (e.AddedItems[0] as FoodPropertie).FoodId;
                ShopContext shopContext = new ShopContext();
                FoodNameList = shopContext.FoodDictionary
                    .Where(f => f.FoodID == foodId)
                    .Select(f => new { FoodName = f.Name, LanguageId = f.LanguageID })
                    .Join(shopContext.Language, f => f.LanguageId, l => l.LanguageID,
                    (f, l) => new FoodName { Name = f.FoodName, NameLanguage = l.Name, Languages = Languages }).ToList();
                foreach (var item in FoodNameList)
                {
                    item.Index = index;
                    index++;
                }
                FoodNames.ItemsSource = FoodNameList.ToArray();
                LanguageNum = FoodNameList.Count;
                string lang;
                if (FoodLanguageBox.SelectedItem != null)
                {
                    lang = FoodLanguageBox.SelectedItem.ToString();
                }
                else lang = MainLanguage;
                int languageId = shopContext.Language.Where(l => l.Name == lang).Select(l => l.LanguageID).First();
                int? foodTypeId = shopContext.Food.Where(f => f.FoodID == foodId).Select(f => f.FoodTypeID).First();
                if (foodTypeId.HasValue)
                {
                    string foodtype = shopContext.FoodTypeDictionary
                        .Where(t => t.FoodTypeID == foodTypeId)
                        .Where(t => t.LanguageID == languageId)
                        .Select(t => t.Name).FirstOrDefault();
                    FoodTypeChangeBox.ItemsSource = Types.ToArray();
                    if (foodtype != null) FoodTypeChangeBox.Text = foodtype;
                }
                NamePanel.Visibility = Visibility.Visible;
            }
        }
    }
}
