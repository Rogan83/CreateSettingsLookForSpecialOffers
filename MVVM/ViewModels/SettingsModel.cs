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
using static Microsoft.Maui.Controls.Behavior;

namespace CreateSettingsLookForSpecialOffers.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class SettingsModel
    {
        public string EmailPattern { get; set; } = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])+";
        public string PathPattern { get; set; } = @"^([a-zA-Z]):[\\\/]((?:[^<>:""\\\/\|\?\*]+[\\\/])*)([^<>:""\\\/\|\?\*]+)\.([^<>:""\\\/\|\?\*\s]+)$";
        public string ProduktNamePattern { get; set; } = @"^([a-zA-Z])";

        public string InfoTextPfad { get; set; } = "Hier können Sie den Pfad für die Excel Datei angeben, wo alle Produkte gespeichert werden. Wenn kein Pfad angegeben wird, dann wird die Tabelle in dem Pfad gespeichert, wo sich das Programm befindet.";
        public string InfoTextEmail { get; set; } = "Damit Sie per E-Mail benachrichtigt werden können, wenn ein oder mehrere Produkte günstig genug sind.";
        public string InfoTextProdukte { get; set; } = "Die Preisgrenze pro Produkt ist nur dann relevant, wenn der Preis pro Kg vom Programm nicht ermittelt werden konnte (z.B. bei einzelnen Früchten oder Taschentücher).";

        public ObservableCollection<FavoriteProduct> FavoriteProducts { get; set; } = new ObservableCollection<FavoriteProduct>{
            new FavoriteProduct("Speisequark", 2.60m),
            new FavoriteProduct("Thunfisch", 5.08m),
            new FavoriteProduct("Tomate", 2.00m),
            new FavoriteProduct("Orange", 0.99m),
            new FavoriteProduct("Buttermilch", 0.99m),
            new FavoriteProduct("Äpfel", 1.99m),
            new FavoriteProduct("Hackfleisch", 5.99m)
        };

        ObservableCollection<MarketName> marketNames = [];
        public ObservableCollection<MarketName> MarketNames
        {
            get { return marketNames; }
            set 
            {
                bool isValid = value.Any(x => x.IsSelected);
                if (isValid)
                    marketNames = value; 
            }
        }

        public SettingsModel()
        {
            MarketNames = new ObservableCollection<MarketName>
            {
                new MarketName("Penny", true),
                new MarketName("Lidl", false),
                new MarketName("Aldi", false),
                new MarketName("Netto", false),
                new MarketName("Kaufland", false),
            };
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


    }
}
