using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.Behaviors
{
    public class TooltipBehavior : Behavior<VisualElement>
    {
        //protected override void OnAttachedTo(VisualElement bindable)
        //{
        //    base.OnAttachedTo(bindable);
        //    bindable += OnMouseEntered;
        //    bindable.MouseExited += OnMouseExited;
        //}

        //protected override void OnDetachingFrom(ImageButton bindable)
        //{
        //    base.OnDetachingFrom(bindable);
        //    bindable.MouseEntered -= OnMouseEntered;
        //    bindable.MouseExited -= OnMouseExited;
        //}

        //private void OnMouseEntered(object sender, MouseEventArgs e)
        //{
        //    if (sender is ImageButton imageButton)
        //    {
        //        ToolTip.SetIsVisible(imageButton, true);
        //        ToolTip.SetMessage(imageButton, "Your tooltip text here");
        //    }
        //}

        //private void OnMouseExited(object sender, MouseEventArgs e)
        //{
        //    if (sender is ImageButton imageButton)
        //    {
        //        ToolTip.SetIsVisible(imageButton, false);
        //    }
        //}
    }
}
