﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.Behaviors
{
    public static class PathPatternValidationBehavior
    {
        public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(PathPatternValidationBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static readonly BindableProperty PatternProperty =
        BindableProperty.CreateAttached("Pattern", typeof(string), typeof(PathPatternValidationBehavior), "", propertyChanged: OnPatternChanged);

        static string pathPattern = string.Empty;
       
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
            try
            {
                pathPattern = (string)newValue;
            }
            catch
            {
                pathPattern = string.Empty;
            }
        }

        static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            Regex regex = new Regex(pathPattern);

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