using ShopPlatform.AdminTools.AddPage;
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
    /// Interaction logic for EditUnitPage.xaml
    /// </summary>
    public partial class EditUnitPage : Page
    {
        private string MainLanguage;

        private List<string> Languages;
        private List<UnitPropertis> Units;

        private List<FoodName> UnitNameList;
        private int LanguageNum;
        private int index;

        public EditUnitPage()
        {
            InitializeComponent();
            LanguageNum = 0;
            index = 0;
        }

        public EditUnitPage(string language) : this()
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
                UnitNameList.Add(new FoodName { Languages = Languages, Index = index });
                UnitNames.ItemsSource = UnitNameList.ToArray();
                LanguageNum++;
                index++;
            }
        }

        private void DisplayUnits(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int unitId = (e.AddedItems[0] as UnitPropertis).UnitId;
                float unitStep = (e.AddedItems[0] as UnitPropertis).Step;

                ShopContext shopContext = new ShopContext();
                UnitNameList = shopContext.UnitDictionary
                    .Where(u => u.UnitID == unitId)
                    .Select(u => new { UnitName = u.Name, LanguageId = u.LanguageID })
                    .Join(shopContext.Language, u => u.LanguageId, l => l.LanguageID,
                    (u, l) => new FoodName { Name = u.UnitName, NameLanguage = l.Name, Languages = Languages }).ToList();
                foreach (var item in UnitNameList)
                {
                    item.Index = index;
                    index++;
                }
                LanguageNum = UnitNameList.Count;
                UnitNames.ItemsSource = UnitNameList.ToArray();
                UnitStep.Text = unitStep.ToString() ;
            }
            NamePanel.Visibility = Visibility.Visible;
        }

        private void DeleteUnitName(object sender, RoutedEventArgs e)
        {
            int elementindex = (int)(sender as FrameworkElement).Tag;
            foreach (var item in UnitNameList)
            {
                if (item.Index == elementindex)
                {
                    UnitNameList.Remove(item);
                    UnitNames.ItemsSource = UnitNameList.ToArray();
                    break;
                }
            }
            LanguageNum--;
        }

        private void SaveUnit(object sender, RoutedEventArgs e)
        {
            if (UnitList.SelectedItem != null)
            {
                int unitId = (UnitList.SelectedItem as UnitPropertis).UnitId;
                ShopContext shopContext = new ShopContext();

                float userStep = Convert.ToSingle(UnitStep.Text);
                float step = (float)shopContext.Unit.Where(u => u.UnitID == unitId).Select(u => u.UnitStep).First();
                if(userStep != step)
                {
                    Unit editedUnit = shopContext.Unit.Where(u => u.UnitID == unitId).First();
                    editedUnit.UnitStep = userStep;
                }


                List<UnitDictionary> DBNames = shopContext.UnitDictionary
                    .Where(d => d.UnitID == unitId).ToList();

                List<UnitDictionary> dictionaryList = new List<UnitDictionary>();
                for (int i = 0; i < UnitNames.Items.Count; i++)
                {
                    ContentPresenter c = (ContentPresenter)UnitNames.ItemContainerGenerator.ContainerFromItem(UnitNames.Items[i]);
                    ComboBox comboBox = c.ContentTemplate.FindName("UnitLanguage", c) as ComboBox;
                    TextBox text = c.ContentTemplate.FindName("UnitName", c) as TextBox;

                    UnitDictionary dictionary = new UnitDictionary();
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
                        dictionary.UnitID = unitId;
                    }
                    dictionaryList.Add(dictionary);
                }

                List<UnitDictionary> duplicates = dictionaryList.GroupBy(d => d.LanguageID).SelectMany(d => d.Skip(1)).ToList();
                if (duplicates.Count != 0)
                {
                    MessageBox.Show("Duplicated languages!");
                    return;
                }

                if (dictionaryList.Count > 0)
                {
                    shopContext.UnitDictionary.RemoveRange(DBNames);
                    shopContext.UnitDictionary.AddRange(dictionaryList);
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

            Units = shopContext.Unit
                .Join(shopContext.UnitDictionary, u => u.UnitID, d => d.UnitID, 
                (u, d) => new { UnitName = d.Name, UnitId = u.UnitID, UnitStep = u.UnitStep, LanguageId = d.LanguageID})
                .Join(shopContext.Language, u => u.LanguageId, l => l.LanguageID,
                (u, l) => new { UnitName = u.UnitName, UnitId = u.UnitId, UnitStep = u.UnitStep, Language = l.Name })
                .Where(l => l.Language == language)
                .Select(u => new UnitPropertis { Name = u.UnitName, UnitId = u.UnitId, Step = (float)u.UnitStep }).ToList();
            UnitList.ItemsSource = Units.ToArray();

            UnitNames.ItemsSource = null;
            NamePanel.Visibility = Visibility.Collapsed;
        }
    }
}
