using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.Behaviors
{
    public static class PatternValidationBehavior
    {
        public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(PatternValidationBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static readonly BindableProperty PatternProperty =
        BindableProperty.CreateAttached("Pattern", typeof(string), typeof(PatternValidationBehavior), "", propertyChanged: OnPatternChanged);

        //public static readonly BindableProperty TypeProperty =
        //BindableProperty.CreateAttached("Type", typeof(string), typeof(PatternValidationBehavior), "", propertyChanged: OnTypeChanged);

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


        //public static string GetType(BindableObject view)
        //{
        //    return (string)view.GetValue(TypeProperty);
        //}

        //public static void SetType(BindableObject view, string value)
        //{
        //    view.SetValue(TypeProperty, value);
        //}

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

        //static void OnTypeChanged(BindableObject view, object oldValue, object newValue)
        //{
        //    try
        //    {
        //        type = (string)newValue;
        //    }
        //    catch
        //    {
        //        type = string.Empty;
        //    }
        //}

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

        static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            Regex regex = new Regex(emailPattern);

            //if (type == "email")
            //{
            //    regex = new Regex(emailPattern);
            //}
            //else if (type == "path")
            //{
            //    regex = new Regex(pathpattern);
            //}

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
