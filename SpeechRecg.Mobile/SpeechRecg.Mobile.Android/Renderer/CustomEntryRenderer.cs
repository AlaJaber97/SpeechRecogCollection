using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using SpeechRecg.Mobile.CustomRenderer;
using SpeechRecg.Mobile.Droid.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEntryRenderer))]
namespace SpeechRecg.Mobile.Droid.CustomRenderer
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Ellipsize = TextUtils.TruncateAt.Start;
                Control.Gravity = Android.Views.GravityFlags.Top;
                Control.SetHorizontallyScrolling(false);
                Control.SetLines(5);
                Control.SetSingleLine(false); 
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                Control.SetBackgroundDrawable(gd);
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}

