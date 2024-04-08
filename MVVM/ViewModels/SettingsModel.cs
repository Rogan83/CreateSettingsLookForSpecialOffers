using CreateSettingsLookForSpecialOffers.Behaviors;
using CreateSettingsLookForSpecialOffers.Enums;
using CreateSettingsLookForSpecialOffers.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using static Microsoft.Maui.Controls.Behavior;
using Newtonsoft.Json.Linq;
using System.Net.Mail;

namespace CreateSettingsLookForSpecialOffers.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class SettingsModel
    {
        static string projectPath = AppDomain.CurrentDomain.BaseDirectory;

        static string jsonFilePath = Path.Combine(projectPath, "settings.json");


        public string EmailPattern { get; set; } = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])+"; 
        //public string PathPattern { get; set; } = @"^([a-zA-Z]):[\\\/]((?:[^<>:""\\\/\|\?\*]+[\\\/])*)([^<>:""\\\/\|\?\*]+)\.([^<>:""\\\/\|\?\*\s]+)$";
        public string PathPattern { get; set; } = @"^([a-zA-Z]):[\\\/]((?:[^<>:""\\\/\|\?\*]+[\\\/])*)([^<>:""\\\/\|\?\*]+)$";
        public string ProduktNamePattern { get; set; } = @"^([a-zA-Z])";

        public string InfoTextPfad { get; set; } = "Hier können Sie den Pfad und den Namen für die Excel Datei angeben, wo alle Produkte gespeichert werden (Beispiel: C:\\Users\\Angebote). Wenn kein Pfad angegeben wird, dann wird die Tabelle in dem Pfad gespeichert, wo sich das Programm befindet mit dem Dateinamen \"Angebote\".";
        public string InfoTextEmail { get; set; } = "Damit Sie per E-Mail benachrichtigt werden können, wenn ein oder mehrere Produkte günstig genug sind.";
        public string InfoTextProdukte { get; set; } = "Die Preisgrenze pro Produkt ist nur dann relevant, wenn der Preis pro Kg vom Programm nicht ermittelt werden konnte (z.B. bei einzelnen Früchten oder Taschentücher).";

        public string Email { get; set; }
        public string PathName { get; set; }

        public ObservableCollection<FavoriteProduct> FavoriteProducts { get; set; } = [];
        public ObservableCollection<Market> Markets { get; set; } = [];

        //ObservableCollection<MarketName> marketNames = [];

        //public ObservableCollection<MarketName> MarketNames
        //{
        //    get { return marketNames; }
        //    set 
        //    {
        //        bool isValid = value.Any(x => x.IsSelected);
        //        if (isValid)
        //            marketNames = value; 
        //    }
        //}

        #region newtonsoft Json Examples
        //Product product = new Product();
        //product.Name = "Apple";
        //product.Expiry = new DateTime(2008, 12, 28);
        //product.Sizes = new string[] { "Small" };

        //string json = JsonConvert.SerializeObject(product);
        // {
        //   "Name": "Apple",
        //   "Expiry": "2008-12-28T00:00:00",
        //   "Sizes": [
        //     "Small"
        //   ]
        // }

        //string json = @"{
        //  'Name': 'Bad Boys',
        //  'ReleaseDate': '1995-4-7T00:00:00',
        //  'Genres': [
        //    'Action',
        //    'Comedy'
        //  ]
        //}";

        //Movie m = JsonConvert.DeserializeObject<Movie>(json);

        //string name = m.Name;
        //// Bad Boys


        //JArray array = new JArray();
        //array.Add("Manual text");
        //array.Add(new DateTime(2000, 5, 23));

        //JObject o = new JObject();
        //o["MyArray"] = array;

        //string json = o.ToString();
        // {
        //   "MyArray": [
        //     "Manual text",
        //     "2000-05-23T00:00:00"
        //   ]
        // }
        #endregion


        public SettingsModel()
        {
            if (File.Exists(jsonFilePath))
            {
                LoadData();
            }
            else
            {
                Markets = new ObservableCollection<Market>
                {
                    new Market("Penny", true),
                    new Market("Lidl", false),
                    new Market("Aldi", false),
                    new Market("Netto", false),
                    new Market("Kaufland", false),
                };

                FavoriteProducts  = new ObservableCollection<FavoriteProduct>
                {
                    new FavoriteProduct("Speisequark", 2.60m),
                    new FavoriteProduct("Thunfisch", 5.08m),
                    new FavoriteProduct("Tomate", 2.00m),
                    new FavoriteProduct("Orange", 0.99m),
                    new FavoriteProduct("Buttermilch", 0.99m),
                    new FavoriteProduct("Äpfel", 1.99m),
                    new FavoriteProduct("Hackfleisch", 5.99m)
                };
            }

            void LoadData()
            {
                string jsonStringFromFile = File.ReadAllText(jsonFilePath);

                JObject data = JObject.Parse(jsonStringFromFile);

                JArray loadedProducts = (JArray)data["FavoriteProducts"];
                FavoriteProducts.Clear();
                foreach (JObject loadedProduct in loadedProducts)
                {
                    string name = (string)loadedProduct["Name"];
                    decimal pricePerKg = (decimal)loadedProduct["PriceCapPerKg"];
                    decimal pricePerProduct = (decimal)loadedProduct["PriceCapPerProduct"];

                    FavoriteProducts.Add(new FavoriteProduct(name, pricePerKg, pricePerProduct));
                }

                JArray loadedMarkets = (JArray)data["Markets"];
                foreach (JObject loadedMarket in loadedMarkets)
                {
                    string name = (string)loadedMarket["Name"];
                    bool isSelected = (bool)loadedMarket["IsSelected"];

                    Markets.Add(new Market(name, isSelected));
                }

                string loadedEmail, loadedPath;
                if (data["Email"] != null)
                {
                    Email = (string)data["Email"];
                }

                if (data["Path"] != null)
                {
                    PathName = ((string)data["Path"]).Replace(".xlsx", "");
                }
            }
        }

        public ICommand AddFavoriteProduct =>
            new Command(async(grid) =>
            {
                var elements = (Grid)grid;
                if (elements == null) return;

                var entryProductName = elements.FindByName("productName") as Entry;
                var entryPriceCapPerKg = elements.FindByName("priceCapPerKg") as Entry;
                var entryPriceCapPerProduct = elements.FindByName("priceCapPerProduct") as Entry;

                string productName = string.Empty;
                if (entryProductName != null)
                {
                    if (entryProductName.Text == null)
                    {
                        await App.Current.MainPage.DisplayAlert("", "Das Eingabefeld für den Produktnamen darf nicht leer sein!", "OK");
                        return;
                    }    
                    var inputText = entryProductName.Text.ToString();

                    ValidationState? state = ProductNameValidationBehavior.CheckInputValidity(inputText);
                    if (state == null) { return; }
                    if (state == ValidationState.Valid)
                    {
                        productName = inputText;
                    }
                    else if (state == ValidationState.Empty)
                    {
                        await App.Current.MainPage.DisplayAlert("", "Das Eingabefeld für den Produktnamen darf nicht leer sein!", "OK");
                        return;
                    }
                    else if (state == ValidationState.Invalid)
                    {
                        await App.Current.MainPage.DisplayAlert("", "Der Produktname muss mit einen Buchstaben anfangen und muss mindestens 2 Zeichen beinhalten.", "OK");
                        return;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "Das Eingabefeld für den Produktnamen darf nicht leer sein!", "OK");
                    return;
                }

                decimal priceCapPerKg = 0;
                string errorMsgEmptyEntry = "Das Eingabefeld für die Preisgrenze pro Kg darf nicht leer sein!";
                if (entryPriceCapPerKg != null)
                {
                    if (entryPriceCapPerKg.Text != null)
                    {
                        ValidationState? state = NumberValidationBehavior.CheckInputValidity(entryPriceCapPerKg.Text);
                        if (state == null) { return; }
                        if (state == ValidationState.Valid)
                        {
                            string value = entryPriceCapPerKg.Text.Replace('.', ',');
                            priceCapPerKg = decimal.Parse(value);
                        }
                        else if (state == ValidationState.Invalid)
                        {
                            await App.Current.MainPage.DisplayAlert("", "Ungültige Eingabe im Eingabefeld für die Preisgrenze pro Kg!", "OK");
                            return;
                        }
                        else if (state == ValidationState.Empty)
                        {
                            await App.Current.MainPage.DisplayAlert("", errorMsgEmptyEntry, "OK");
                            return;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("", errorMsgEmptyEntry, "OK");
                        return;
                    }

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", errorMsgEmptyEntry, "OK");
                    return;
                }

                decimal priceCapPerProduct = 0;
                errorMsgEmptyEntry = "Das Eingabefeld für die Preisgrenze pro Produkt darf nicht leer sein!";
                if (entryPriceCapPerProduct != null)
                {
                    if (entryPriceCapPerProduct.Text != null)
                    {
                        ValidationState? state = NumberValidationBehavior.CheckInputValidity(entryPriceCapPerProduct.Text);
                        if (state == null) { return; }
                        if (state == ValidationState.Valid)
                        {
                            string value = entryPriceCapPerProduct.Text.Replace('.', ',');
                            priceCapPerProduct = decimal.Parse(value);
                        }
                        else if (state == ValidationState.Invalid)
                        {
                            await App.Current.MainPage.DisplayAlert("", "Ungültige Eingabe im Eingabefeld für die Preisgrenze pro Produkt!", "OK");
                            return;
                        }
                        else if (state == ValidationState.Empty)
                        {
                            await App.Current.MainPage.DisplayAlert("", errorMsgEmptyEntry, "OK");
                            return;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("", errorMsgEmptyEntry, "OK");
                        return;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", errorMsgEmptyEntry, "OK");
                    return;
                }

                var newFavoriteProduct = new FavoriteProduct(productName, priceCapPerKg, priceCapPerProduct);

                // Wenn der Produkt Name noch nicht in der Liste vorhanden ist, dann füge ihn zu, wenn doch, dann
                // Gebe eine Warnung aus, dass das Produkt schon vorhanden ist
                if (!FavoriteProducts.Any(favoriteProduct => favoriteProduct.Name == newFavoriteProduct.Name))
                {
                    var listView = elements.FindByName("listView") as ListView;
                    FavoriteProducts.Add(newFavoriteProduct);

                    var scrollView = elements.FindByName("scrollView") as ScrollView;
                    if (scrollView != null)
                    {
                        await Task.Delay(100);  // Kurze Verzögerung, damit die UI-Änderungen abgeschlossen werden
                        await scrollView.ScrollToAsync(listView, ScrollToPosition.End, true);
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "Dieses Produkt ist schon in der Liste enthalten.", "OK");
                    return;
                }
            });

        public ICommand DeleteAllFavoriteProducts =>
            new Command(() =>
            {
                FavoriteProducts.Clear();
            });

        public ICommand DeleteFavoriteProduct =>
            new Command((item) =>
            {
                FavoriteProduct product = (FavoriteProduct)item;
                if (product != null)
                    FavoriteProducts.Remove(product);
            });

        //public ICommand ShowInfo =>
        //    new Command(async (infoLabel) =>
        //    {
        //        var label = (Label)infoLabel;
        //        if (label == null) { return; }

        //        label.IsVisible = true;
        //        await Task.Delay(3000);
        //        label.IsVisible = false;
        //    });

        public ICommand ShowInfo =>
            new Command(async (infoText) =>
            {
                var text = infoText as string;
                await App.Current.MainPage.DisplayAlert("", text, "OK");
            });

        public ICommand SaveDataAndStartProgram =>
            new Command(async(grid) =>
            {
                // Email und Datei Pfad von dein Eingabefeldern holen
                var elements = (Grid)grid;
                if (elements == null) return;

                Entry? entryEmail = elements.FindByName("email") as Entry;
                Entry? entryPath = elements.FindByName("path") as Entry;

                string? email = string.Empty, path = string.Empty;

                email = entryEmail?.Text;
                path = entryPath?.Text;

                

                // email und path auf gültigkeit prüfen
                ValidationState? state = EmailPatternValidationBehavior.CheckInputValidity(email);
                if (state == null) { state = ValidationState.Empty; }
                if (state == ValidationState.Invalid)
                {
                    await App.Current.MainPage.DisplayAlert("", "Ungültige Eingabe für die E-Mail Adresse.", "OK");
                    return;
                }

                state = PathPatternValidationBehavior.CheckInputValidity(path);
                if (state == null) { state = ValidationState.Empty; }
                if (state == ValidationState.Invalid)
                {
                    await App.Current.MainPage.DisplayAlert("", "Ungültige Eingabe für den Datei Pfad.", "OK");
                    return;
                }

                //Dateiendung hinzufügen
                if (path != null)
                    path += ".xlsx";

                //Überprüfen, ob mindestens 1 Markt ausgewählt wurde
                bool isValid = Markets.Any(x => x.IsSelected);
                if (!isValid)
                {
                    await App.Current.MainPage.DisplayAlert("", "Es muss mindestens 1 Markt ausgewählt werden.", "OK");
                    return;
                }

                SaveData(email, path);
            });



        void SaveData(string? email, string? path)
        {
            if (email == null) { email = string.Empty; }
            if (path == null) { path = string.Empty; }

            JArray products = new();
            foreach (var favoriteProduct in FavoriteProducts)
            {
                JObject product = new();
                product["Name"] = favoriteProduct.Name;
                product["PriceCapPerKg"] = favoriteProduct.PriceCapPerKg;
                product["PriceCapPerProduct"] = favoriteProduct.PriceCapPerProduct;

                products.Add(product);
            }

            JArray markets = new();
            foreach (var Market in Markets)
            {
                JObject market = new();
                market["Name"] = Market.Name;
                market["IsSelected"] = Market.IsSelected;

                markets.Add(market);
            }

            JObject root = new JObject();
            root["FavoriteProducts"] = products;
            root["Markets"] = markets;
            root["Email"] = email;
            root["Path"] = path;

            string json = JsonConvert.SerializeObject(root, Formatting.Indented);

            File.WriteAllText(jsonFilePath, json);

            // Datei LADEN

            //string jsonStringFromFile = File.ReadAllText(jsonFilePath);

            //JObject data = JObject.Parse(jsonStringFromFile);

            //JArray loadedProducts = (JArray)data["FavoriteProducts"];
            //ObservableCollection<FavoriteProduct> fp = new ObservableCollection<FavoriteProduct>();
            //foreach (JObject loadedProduct in loadedProducts)
            //{
            //    string name = (string)loadedProduct["Name"];
            //    decimal pricePerKg = (decimal)loadedProduct["PriceCapPerKg"];
            //    decimal pricePerProduct = (decimal)loadedProduct["PriceCapPerProduct"];

            //    fp.Add(new FavoriteProduct(name, pricePerKg, pricePerProduct));
            //}

            //JArray loadedMarkets = (JArray)data["Markets"];
            //ObservableCollection<Market> loadedList = new ObservableCollection<Market>();
            //foreach (JObject loadedMarket in loadedMarkets)
            //{
            //    string name = (string)loadedMarket["Name"];
            //    bool isSelected = (bool)loadedMarket["IsSelected"];

            //    loadedList.Add(new Market(name, isSelected));
            //}

            //string loadedEmail, loadedPath;
            //if (data["Email"] != null)
            //{
            //    loadedEmail = (string)data["Email"];
            //}

            //if (data["Path"] != null)
            //{
            //    loadedPath = (string)data["Path"];
            //}

            ////string e = (string)loadedemail["Email"];

            //var a = 1;


        }

        
    }
}
