using CreateSettingsLookForSpecialOffers.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CreateSettingsLookForSpecialOffers.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class SettingsModel
    {
        public ObservableCollection<FavoriteProduct> Products { get; set; } = new ObservableCollection<FavoriteProduct>{
            new FavoriteProduct("Speisequark", 2.60m),
            new FavoriteProduct("Thunfisch", 5.08m),
            new FavoriteProduct("Tomate", 2.00m),
            new FavoriteProduct("Orange", 0.99m),
            new FavoriteProduct("Buttermilch", 0.99m),
            new FavoriteProduct("Äpfel", 1.99m),
            new FavoriteProduct("Hackfleisch", 5.99m)
        };


        public ICommand AddFavoriteProduct =>
            new Command(() =>
            {
                Products.Add(new FavoriteProduct("quark", 2.99m));
            });

        public ICommand DeleteAllFavoriteProducts =>
            new Command(() =>
            {
                Products.Clear();
            });

        public ICommand DeleteFavoriteProduct =>
            new Command((item) =>
            {
                FavoriteProduct product = (FavoriteProduct)item;
                if (product != null)
                    Products.Remove(product);
            });
    }
}
