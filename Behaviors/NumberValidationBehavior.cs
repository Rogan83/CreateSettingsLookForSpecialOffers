using CreateSettingsLookForSpecialOffers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.Behaviors
{
    public static class NumberValidationBehavior
    {
        public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(NumberValidationBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static bool GetAttachBehavior(BindableObject view)
        {
            return (bool)view.GetValue(AttachBehaviorProperty);
        }

        public static void SetAttachBehavior(BindableObject view, bool value)
        {
            view.SetValue(AttachBehaviorProperty, value);
        }

        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            Entry? entry = view as Entry;
            if (entry == null) {return;}

            bool? attachBehavior = null;
            try
            {
                attachBehavior = (bool)newValue;
            }
            catch {  }

            if (attachBehavior == null) { return; }

            if (attachBehavior == true)
            {
                entry.TextChanged += OnEntryTextChanged;
            }
            else
            {
                entry.TextChanged -= OnEntryTextChanged;
            }
        }

        public static ValidationState? CheckInputValidity(string? input)
        {
            if (input == null) { return null; }

            input = input.Replace('.', ',');

            decimal result = 0;
            bool isValid = decimal.TryParse(input, out result);

            if (isValid)
            {
                return ValidationState.Valid;
            }
            else if (input.Length == 0)
            {
                return ValidationState.Empty;
            }
            else
            {
                return ValidationState.Invalid;
            }
        }

        static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            Entry? entry = sender as Entry;
            string? input = args.NewTextValue;
            ValidationState? state = CheckInputValidity(input);
            if (state == null) { return; }
            if (state == ValidationState.Valid)
            {
                VisualStateManager.GoToState(entry, state.ToString());
            }
            else if (state == ValidationState.Empty)
            {
                VisualStateManager.GoToState(entry, state.ToString());
            }
            else if (state == ValidationState.Invalid)
            {
                VisualStateManager.GoToState(entry, state.ToString());
            }
        }
    }
}
