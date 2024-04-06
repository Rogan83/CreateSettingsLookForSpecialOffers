using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class MarketName
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public MarketName(string name, bool isSelected) 
        { 
            Name = name;
            IsSelected = isSelected;
        }
    }
}
