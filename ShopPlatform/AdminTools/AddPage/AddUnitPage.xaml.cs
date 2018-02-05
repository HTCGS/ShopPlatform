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

namespace ShopPlatform.AdminTools.AddPage
{
    /// <summary>
    /// Interaction logic for AddUnitPage.xaml
    /// </summary>
    public partial class AddUnitPage : Page
    {
        private string MainLanguage;
        private List<string> Languages;

        private int LanguageNum;

        public AddUnitPage()
        {
            InitializeComponent();
        }

        public AddUnitPage(string language) : this()
        {
            this.MainLanguage = language;
            LanguageNum = 1;

            Languages = MainFunctions.GetLanguages();
            UnitNames.Items.Add(new NewLanguage { Languages = Languages });
        }

        private void AddImage(object sender, RoutedEventArgs e)
        {
        }

        private void AddLanguage(object sender, RoutedEventArgs e)
        {
            if (LanguageNum < Languages.Count)
            {
                UnitNames.Items.Add(new NewLanguage { Languages = Languages });
                LanguageNum++;
            }
        }

        private void SaveUnit(object sender, RoutedEventArgs e)
        {
            if (UnitStep.Text == "")
            {
                MessageBox.Show("Unit step is empty!");
                return;
            }

            ShopContext shopContext = new ShopContext();
            Unit unit = new Unit();
            unit.UnitStep = Convert.ToSingle(UnitStep.Text);
            shopContext.Unit.Add(unit);

            List<UnitDictionary> dictionaryList = new List<UnitDictionary>();
            for (int i = 0; i < UnitNames.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter)UnitNames.ItemContainerGenerator.ContainerFromItem(UnitNames.Items[i]);
                ComboBox comboBox = c.ContentTemplate.FindName("NewUnitLanguage", c) as ComboBox;
                TextBox text = c.ContentTemplate.FindName("NewUnitName", c) as TextBox;

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
                    string lang = comboBox.SelectedItem.ToString();
                    int langId = shopContext.Language.Where(l => l.Name == lang).Select(l => l.LanguageID).First();
                    dictionary.LanguageID = langId;
                }
                dictionaryList.Add(dictionary);
            }

            List<UnitDictionary> duplicates = dictionaryList.GroupBy(d => d.LanguageID).SelectMany(d => d.Skip(1)).ToList();
            if (duplicates.Count != 0)
            {
                MessageBox.Show("Duplicated languages!");
                return;
            }

            foreach (var item in dictionaryList)
            {
                if (shopContext.UnitDictionary.Where(f => f.Name == item.Name && f.LanguageID == item.LanguageID).Count() > 0)
                {
                    UnitDictionary unitDup = shopContext.UnitDictionary.Where(f => f.Name == item.Name && f.LanguageID == item.LanguageID).FirstOrDefault();
                    if (shopContext.Unit.Where(u => u.UnitID == unitDup.UnitID && u.UnitStep == unit.UnitStep).FirstOrDefault() != null)
                    {
                        MessageBox.Show("Unit is existed!");
                        return;
                    }
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

            List<int> foodTypeIdList = shopContext.Unit.Where(u => u.UnitStep == unit.UnitStep).Select(u => u.UnitID).ToList();
            foreach (var id in foodTypeIdList)
            {
                int idCount = shopContext.UnitDictionary.Where(u => u.UnitID == id).Count();
                if (idCount == 0)
                {
                    foreach (var item in dictionaryList)
                    {
                        item.UnitID = id;
                        shopContext.UnitDictionary.Add(item);
                    }
                    break;
                }
            }
            shopContext.SaveChanges();
            UnitStep.Text = "";
            UnitNames.Items.Clear();
            UnitNames.Items.Add(new NewLanguage { Languages = Languages});
        }

        private void InputValidation(object sender, TextCompositionEventArgs e)
        {
            float input;
            e.Handled = !(float.TryParse(e.Text.ToString(), out input) || (e.Text.ToString() == "."));
        }
    }
}
