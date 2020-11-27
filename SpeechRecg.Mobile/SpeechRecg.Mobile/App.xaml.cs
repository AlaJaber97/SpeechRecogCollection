using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace SpeechRecg.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            NavigationToPage();
        }
        public async void NavigationToPage()
        {
#if RELEASE
            AppStatics.Token = await Xamarin.Essentials.SecureStorage.GetAsync("Token");
            if (string.IsNullOrEmpty(AppStatics.Token))
                MainPage = new NavigationPage(new Pages.LoginPage());
            else
                MainPage = new NavigationPage(new Pages._TabbedPage());
#else
            MainPage = new NavigationPage(new Pages.LoginPage());
#endif
        }
        protected override void OnStart()
        {
            AppCenter.Start("android=f4aafea7-fcb9-43da-b610-d3d0d63adef3;" +
                            "uwp=38746c0a-cd10-43ae-a832-0e816f0ca7db;",
                            typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
