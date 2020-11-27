using SpeechRecg.Mobile.Helper.FontAwesome;
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
#pragma warning disable IDE1006 // Naming Styles
    public partial class _TabbedPage : TabbedPage
#pragma warning restore IDE1006 // Naming Styles
    {
        public _TabbedPage()
        {
            InitializeComponent();
        }
    }
}