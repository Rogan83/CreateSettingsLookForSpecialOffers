﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.Behaviors
{
    public  class ValidationBehavior : Behavior<Entry>
    {
        //public static readonly BindableProperty PatternProperty =
        //BindableProperty.Create(nameof(Pattern), typeof(string), typeof(ValidationBehavior), default(string));

        //public string Pattern
        //{
        //    get => (string)GetValue(PatternProperty);
        //    set => SetValue(PatternProperty, value);
        //}
        public string Pattern { get; set; }



        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);
            entry.TextChanged += Bindable_TextChanged;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);
            entry.TextChanged -= Bindable_TextChanged;
        }

        private void Bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            string emailPattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])+";
            //string pathPattern = @"^([a-zA-Z]):[\\\/]((?:[^<>:""\\\/\|\?\*]+[\\\/])*)([^<>:""\\\/\|\?\*]+)\.([^<>:""\\\/\|\?\*\s]+)$";
            //string pattern = @"\w+@\w+\.(de|com)";

            //Pattern = emailPattern;
            if (Pattern == null) { return; }

            Regex regex = new Regex(Pattern);

            var entry = sender as Entry;

            if (entry == null) { return; }

            Match match = regex.Match(entry.Text);

            if (match.Success)
            {
                VisualStateManager.GoToState(entry, "Valid");
            }
            else if (entry.Text.Length == 0)
            {
                VisualStateManager.GoToState(entry, "Empty");
            }
            else
            {
                VisualStateManager.GoToState(entry, "Invalid");
            }
        }
    }
}
