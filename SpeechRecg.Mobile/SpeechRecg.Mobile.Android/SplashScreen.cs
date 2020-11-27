using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace SpeechRecg.Mobile.Droid
{
    [Activity(
        Label = "DataCollection System", Icon = "@drawable/Logo",
        MainLauncher = true, NoHistory = true, Theme = "@style/SplashTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            //ActionBar.SetIcon();
            base.OnCreate(bundle);
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }
    }
}