using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using SpeechRecg.Mobile.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SpeechRecg.Mobile.CustomControl;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace SpeechRecg.Mobile.Droid.Renderer
{
    public class CustomPickerRenderer : ViewRenderer<CustomPicker, Spinner>
    {
        Spinner spinner;
        public CustomPickerRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomPicker> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                spinner = new Spinner(Context);
                SetNativeControl(spinner);
            }

            if (e.OldElement != null)
            {
                Control.ItemSelected -= OnItemSelected;
            }
            if (e.NewElement != null)
            {
                var view = e.NewElement;
                if (view.ItemsSource != null)
                {
                    ArrayAdapter adapter = new ArrayAdapter(Context, Android.Resource.Layout.SimpleListItem1, view.ItemsSource);
                    Control.Adapter = adapter;

                    if (view.SelectedIndex != -1)
                    {
                        Control.SetSelection(view.SelectedIndex);
                    }

                    Control.ItemSelected += OnItemSelected;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var view = Element;
            if (e.PropertyName == CustomPicker.ItemsSourceProperty.PropertyName)
            {
                ArrayAdapter adapter = new ArrayAdapter(Context, Android.Resource.Layout.SimpleListItem1, view.ItemsSource);
                Control.Adapter = adapter;
            }
            if (e.PropertyName == CustomPicker.SelectedIndexProperty.PropertyName)
            {
                Control.SetSelection(view.SelectedIndex);
            }
            base.OnElementPropertyChanged(sender, e);
        }

        private void OnItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var view = Element;
            if (view != null)
            {
                view.SelectedIndex = e.Position;
                view.OnItemSelected(e.Position);
                view.SelectedItem = spinner.GetItemAtPosition(e.Position).ToString();
            }
        }
    }
}