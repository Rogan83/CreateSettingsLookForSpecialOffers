using CreateSettingsLookForSpecialOffers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.Behaviors
{
    public static class EmailPatternValidationBehavior
    {
        public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(EmailPatternValidationBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static readonly BindableProperty PatternProperty =
        BindableProperty.CreateAttached("Pattern", typeof(string), typeof(EmailPatternValidationBehavior), "", propertyChanged: OnPatternChanged);

        static string emailPattern = string.Empty;
        //static string pathpattern = string.Empty;
        //static string type = string.Empty;

        public static bool GetAttachBehavior(BindableObject view)
        {
            return (bool)view.GetValue(AttachBehaviorProperty);
        }

        public static void SetAttachBehavior(BindableObject view, bool value)
        {
            view.SetValue(AttachBehaviorProperty, value);
        }


        public static string GetPattern(BindableObject view)
        {
            return (string)view.GetValue(PatternProperty);
        }

        public static void SetPattern(BindableObject view, string value)
        {
            view.SetValue(PatternProperty, value);
        }

        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            Entry? entry = view as Entry;
            if (entry == null)
            {
                return;
            }
            bool? attachBehavior = null;
            try
            {
                attachBehavior = (bool)newValue;
            }
            catch { }

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
            //try
            //{
            //    if (type == "email")
            //    {
            //        emailPattern = (string)newValue;
            //    }
            //    else if (type == "path")
            //    {
            //        pathpattern = (string)newValue;
            //    }
            //    else
            //    {
            //        emailPattern = pathpattern = string.Empty;
            //    }
            //}
            //catch
            //{
            //    emailPattern = pathpattern = string.Empty;
            //}
            try
            {
                emailPattern = (string)newValue;
            }
            catch
            {
                emailPattern = string.Empty;
            }
        }

        public static ValidationState? CheckInputValidity(string? input)
        {
            Regex regex = new Regex(emailPattern);

            if (input == null) { return null; }
            Match match = regex.Match(input);

            if (match.Success)
            {
                // Eingabe gültig
                return ValidationState.Valid;
            }
            else if (input.Length == 0)
            {
                // Eingabe leer
                return ValidationState.Empty;
            }
            else
            {
                // Eingabe ungültig
                return ValidationState.Invalid;
            }
        }

        static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            Entry? entry = sender as Entry;
            string? input = entry?.Text;
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
