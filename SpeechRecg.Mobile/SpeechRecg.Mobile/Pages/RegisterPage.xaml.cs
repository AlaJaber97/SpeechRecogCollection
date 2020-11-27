using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpeechRecg.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        private BLL.Sql.Models.LoginAndRegister.Register _InfoUser;

        public BLL.Sql.Models.LoginAndRegister.Register InfoUser
        {
            get { return _InfoUser; }
            set { _InfoUser = value; OnPropertyChanged(); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged(); }
        }
        private string _ErrorText;
        public string ErrorText
        {
            get { return _ErrorText; }
            set { _ErrorText = value; OnPropertyChanged(); }
        }
        public List<string> Days { get; set; }
        public List<string> Months { get; set; }
        public List<string> Years { get; set; }
        public RegisterPage()
        {
            InitializeComponent();
            Days = new List<string>() { "DD" };
            for (int day = 1; day <= 31; day++) Days.Add($"{day}");
            Months = new List<string>() { "MM" };
            foreach (var month in new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" }) Months.Add(month);
            Years = new List<string>() { "YYYY" };
            for (int year = DateTime.Now.Year; year >= 1869; year--) Years.Add($"{year}");
            BindingContext = this;
            InfoUser = new BLL.Sql.Models.LoginAndRegister.Register();
            SetGenderMale(null,null);
        }

        private void OpenLoginPage(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopAsync();
            }

            catch (Exception ex)
            {
                DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }

        private void SetGenderMale(object sender, EventArgs e)
        {
            try
            {
                GirlButton.Source = "GrilGeneder_unactive";
                BoyButton.Source = "YoungGeneder_active";
                InfoUser.Gender = BLL.Enum.Gender.Male;
            }
            catch(Exception ex)
            {
                DisplayAlert("Unable to register",ex.ToString(), "Ok");
            }
        }

        private void SetGenderFemale(object sender, EventArgs e)
        {
            try
            {
                GirlButton.Source = "GrilGeneder_active";
                BoyButton.Source = "YoungGeneder_unactive";
                InfoUser.Gender = BLL.Enum.Gender.Female;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error",ex.ToString(), "Ok");
            }
        }

        private async void RegisterButton(object sender, EventArgs e)
        {
            IsBusy = true;
            try
            {
                if (!string.IsNullOrEmpty(InfoUser.Email))
                {
                    int year = -1;
                    int month = -1;
                    int day = -1;
                    if (!int.TryParse(DayPicker.SelectedItem, out day)) 
                        throw new Exception("Select Day in Birthday Date");
                    if (1 <= MonthPicker.SelectedIndex && MonthPicker.SelectedIndex <= 12)
                        month = MonthPicker.SelectedIndex;
                    else
                        throw new Exception("Select Month in Birthday Date");

                    if (!int.TryParse(YearPicker.SelectedItem, out year)) 
                        throw new Exception("Select Year in Birthday Date");

                    InfoUser.Birthday = new DateTime(year, month, day);
                    var Response = await new BLL.Services.HttpExtension<BLL.Sql.Models.LoginAndRegister.Register>().PostReturnStatusCodeAndString("Account/Register", InfoUser, AppStatics.Token);
                    if (Response.statusCode == System.Net.HttpStatusCode.OK)
                    {
                        AppStatics.Token = Response.message;
                        Application.Current.MainPage = new NavigationPage(new Pages._TabbedPage());
                    }
                    else
                    {
                        ErrorText = Response.message;
                    }
                }
                else
                {
                    throw new Exception("Email field is required!");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Unable to register", ex.ToString(), "OK");
            }
            IsBusy = false;
        }
    }
}