﻿using CreateSettingsLookForSpecialOffers.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public ObservableCollection<MarketName> MarketNames { get; set; } = new ObservableCollection<MarketName>
        {
            new MarketName("Penny", true),
            new MarketName("Lidl", false),
            new MarketName("Aldi", false),
            new MarketName("Netto", false),
            new MarketName("Kaufland", false),
        };

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

                    Regex regex = new Regex(ProduktNamePattern);

                    Match match = regex.Match(inputText);

                    if (inputText != string.Empty)
                    {
                        if (match.Success && inputText.Length >=2)
                        {
                            productName = inputText;
                        }
                        else
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
                }

                decimal priceCapPerKg = 0;
                if (entryPriceCapPerKg != null)
                {
                    if (entryPriceCapPerKg.Text != null)
                    {
                        if (!decimal.TryParse(entryPriceCapPerKg.Text.ToString(), out priceCapPerKg))
                        {
                            await App.Current.MainPage.DisplayAlert("", "Ungültige Eingabe im Eingabefeld für die Preisgrenze pro Kg!", "OK");
                            return;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("", "Das Eingabefeld für die Preisgrenze pro Kg darf nicht leer sein!", "OK");
                        return;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "Das Eingabefeld für die Preisgrenze pro Kg darf nicht leer sein!", "OK");
                    return;
                }

                decimal priceCapPerProduct = 0;
                if (entryPriceCapPerProduct != null)
                {
                    if (entryPriceCapPerProduct.Text != null)
                    {
                        if (!Decimal.TryParse(entryPriceCapPerProduct.Text.ToString(), out priceCapPerProduct))
                        {
                            await App.Current.MainPage.DisplayAlert("", "Ungültige Eingabe im Eingabefeld für die Preisgrenze pro Produkt!", "OK");
                            return;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("", "Das Eingabefeld für die Preisgrenze pro Produkt darf nicht leer sein!", "OK");
                        return;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "Das Eingabefeld für die Preisgrenze pro Produkt darf nicht leer sein!", "OK");
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
