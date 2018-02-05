using ShopPlatform.UserPages.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopPlatform
{
    static class MainFunctions
    {
        public static MainWindow Main;

        public static int AdminShopId = -1;

        public static List<string> GetLanguages()
        {
            ShopContext shopContext = new ShopContext();
            return shopContext.Language.Select(l => l.Name).ToList();
        }

        public static List<string> GetFoodTypes()
        {
            ShopContext shopContext = new ShopContext();
            return shopContext.FoodTypeDictionary
                .Join(shopContext.Language, t => t.LanguageID, l => l.LanguageID,
                (t, l) => new { TypeName = t.Name, Language = l.Name })
                .Select(t => t.TypeName).ToList();
        }

        public static List<string> GetFoodTypes(string language)
        {
            ShopContext shopContext = new ShopContext();
            return shopContext.FoodTypeDictionary
                .Join(shopContext.Language, t => t.LanguageID, l => l.LanguageID,
                (t, l) => new { TypeName = t.Name, Language = l.Name })
                .Where(t => t.Language == language)
                .Select(t => t.TypeName).ToList();
        }

        public static List<StoreItem> GetStoreFoodTypes(int shopId, string language)
        {
            ShopContext shopContext = new ShopContext();
            return shopContext.ShopStore
                .Where(s => s.ShopID == shopId)
                .Select(s => new { FoodId = s.FoodID })
                .Join(shopContext.Food, s => s.FoodId, f => f.FoodID,
                (s, f) => new { TypeId = f.FoodTypeID })
                .Join(shopContext.FoodType, f => f.TypeId, t => t.FoodTypeID,
                (f, t) => new { TypeId = f.TypeId, Image = t.Image })
                .Join(shopContext.FoodTypeDictionary, t => t.TypeId, d => d.FoodTypeID,
                (t, d) => new { TypeName = d.Name, TypeId = t.TypeId, Image = t.Image, LanguageId = d.LanguageID })
                .Join(shopContext.Language, d => d.LanguageId, l => l.LanguageID,
                (d, l) => new { TypeName = d.TypeName, TypeId = d.TypeId, Image = d.Image, Language = l.Name })
                .Where(l => l.Language == language)
                .Select(t => new StoreItem { Name = t.TypeName, Id = (int)t.TypeId, Image = t.Image}).ToList();
        }

        public static List<StoreItem> GetStoreFood(int shopId, string language, int typeId)
        {
            ShopContext shopContext = new ShopContext();
            return shopContext.ShopStore
                .Where(s => s.ShopID == shopId)
                .Join(shopContext.Food, s => s.FoodID, f => f.FoodID,
                (s, f) => new { FoodId = s.FoodID, FoodTypeId =f.FoodTypeID, Image = f.Image, Amount = s.Amount, UnitId = s.UnitID })
                .Where(t => t.FoodTypeId == typeId)
                .Join(shopContext.FoodDictionary, s => s.FoodId, d => d.FoodID,
                (s, d) => new { Store = s, Name = d.Name, LanguageId = d.LanguageID })
                .Join(shopContext.Language, d => d.LanguageId, l => l.LanguageID,
                (d, l) => new { Store = d, Language = l.Name })
                .Where(l => l.Language == language)
                .Select(s => new
                {
                    FoodId = s.Store.Store.FoodId,
                    FoodName = s.Store.Name,
                    Image = s.Store.Store.Image,
                    Amount = s.Store.Store.Amount,
                    UnitId = s.Store.Store.UnitId
                })
                .Join(shopContext.Unit, s => s.UnitId, u => u.UnitID,
                (s, u) => new { Store = s, UnitStep = u.UnitStep })
                .Join(shopContext.UnitDictionary, s => s.Store.UnitId, d => d.UnitID,
                (s, d) => new { Store = s, UnitStep = s.UnitStep, Unitname = d.Name, languageId = d.LanguageID })
                .Join(shopContext.Language, s => s.languageId, l => l.LanguageID,
                (s, l) => new
                {
                    FoodId = s.Store.Store.FoodId,
                    FoodName = s.Store.Store.FoodName,
                    Amount = s.Store.Store.Amount,
                    Image = s.Store.Store.Image,
                    UnitId = s.Store.Store.UnitId,
                    UnitName = s.Unitname,
                    UnitStep = s.UnitStep,
                    Language = l.Name
                })
                .Where(l => l.Language == language)
                .Select(s => new StoreItem
                {
                    Name = s.FoodName,
                    Id = s.FoodId,
                    Image = s.Image,
                    Amount = (float)s.Amount,
                    Unit = s.UnitName,
                    UnitId = s.UnitId,
                    UnitStep = (float)s.UnitStep
                }).ToList();
        }

        public static ResourceDictionary GetLanguageResource()
        {
            string resString = string.Empty;
            switch (Main.MainLanguage)
            {
                case "Русский":
                    resString = "pack://application:,,,/ShopPlatform;component/Resources/Languages/RussianDictionary.xaml";
                    break;

                case "English":
                    resString = "pack://application:,,,/ShopPlatform;component/Resources/Languages/EnglishDictionary.xaml";
                    break;

                default:
                    break;
            }

            ResourceDictionary dictionary = new ResourceDictionary()
            {
                Source = new Uri(resString)
            };
            return dictionary;
        }
    }
}
