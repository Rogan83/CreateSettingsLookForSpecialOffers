using CreateSettingsLookForSpecialOffers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.Behaviors
{
    public static class ZipCodePatternValidationBehavior
    {
        public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(ZipCodePatternValidationBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static readonly BindableProperty PatternProperty =
        BindableProperty.CreateAttached("Pattern", typeof(string), typeof(ZipCodePatternValidationBehavior), "", propertyChanged: OnPatternChanged);

        static string zipCodePattern = string.Empty;

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

        static void OnPatternChanged(BindableObject view, object oldValue, object newValue)
        {
            try
            {
                zipCodePattern = (string)newValue;
            }
            catch
            {
                zipCodePattern = string.Empty;
            }
        }

        public static ValidationState? CheckInputValidity(string? input)
        {
            if (input == null) { return null; }

            Regex regex = new Regex(zipCodePattern);

            Match match = regex.Match(input);

            decimal result = 0;
            bool isValid = decimal.TryParse(input, out result);
            // Die PLZ darf nur aus Ziffern bestehen und es müssen genau 5 davon sein
            if (match.Success && isValid && input.Length == 5)
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
