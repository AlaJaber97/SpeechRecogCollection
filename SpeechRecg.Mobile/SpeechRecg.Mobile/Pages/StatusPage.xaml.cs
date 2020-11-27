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
    public partial class StatusPage : ContentPage
    {
        public string Exp { get; set; }
        public string TitlePage { get; set; }
        public bool Status { get; set; }
        public string ImgStatus { get; set; }
        public string TextStatus { get; set; }
        public StatusPage()
        {
            InitializeComponent();
        }

        public StatusPage(string TitlePage, bool Status)
        {
            try
            {
                this.TitlePage = TitlePage;
                this.Status = Status;
                this.ImgStatus = Status ? "box_blue_message.png" : "box_red_message.png";
                this.TextStatus = Status ? "Great, you have successfully!" : "Unfortunately, there appears to be error";

                InitializeComponent();
                this.BindingContext = this;
            }
            catch (Exception ex)
            {
                
                DisplayAlert("Error", ex.ToString(), "OK");
            }
        }

        private void TryAgainAction(object sender, EventArgs e)
        {
            try { Navigation.PopAsync(); }
            catch (Exception ex)
            {
                
                DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
    }
}