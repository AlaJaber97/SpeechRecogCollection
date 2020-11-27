using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpeechRecg.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private BLL.Sql.Models.LoginAndRegister.Login _Login;

        public BLL.Sql.Models.LoginAndRegister.Login Login
        {
            get { return _Login; }
            set { _Login = value; OnPropertyChanged(); }
        }

        private string _ErrorText;

        public string ErrorText
        {
            get { return _ErrorText; }
            set { _ErrorText = value; OnPropertyChanged(); }
        }

        private bool _IsBusy;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged(); }
        }

        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            Login = new BLL.Sql.Models.LoginAndRegister.Login();
        }

        private void OpenRegisterPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.RegisterPage());
        }

        private async void LoginButton(object sender, EventArgs e)
        {
            IsBusy = true;
            try
            {
                if(string.IsNullOrEmpty(Login.Email))
                {
                    ErrorText = "This field is required!";
                }
                else
                {
                    var Response = await new BLL.Services.HttpExtension<BLL.Sql.Models.LoginAndRegister.Login>().PostReturnStatusCodeAndString("Account/Login", Login, AppStatics.Token); ;
                    if(Response.statusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (CheckBoxRememberMe.IsChecked)
                            await Xamarin.Essentials.SecureStorage.SetAsync("Token", Response.message);
                    
                        AppStatics.Token = Response.message;
                        Application.Current.MainPage = new NavigationPage(new Pages._TabbedPage());
                    }
                    else
                    {
                        ErrorText = Response.message;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK"); ;
            }
            IsBusy = false;
        }
    }
}
