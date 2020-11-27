using SpeechRecg.Mobile.AudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpeechRecg.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Speech2TextPage : ContentPage
    {
        #region Prop
        private bool _IsBusy;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged(); }
        }
        private bool _IsDownloded;
        public bool IsDownloded
        {
            get { return _IsDownloded; }
            set { _IsDownloded = value; OnPropertyChanged(); }
        }
        private bool _IsPlaying;
        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set { _IsPlaying = value;  OnPropertyChanged(); }
        }
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { _Text = value; OnPropertyChanged(); }
        }
        private bool _AudioIsPrepared;
        public bool AudioIsPrepared
        {
            get { return _AudioIsPrepared; }
            set { _AudioIsPrepared = value; OnPropertyChanged(); }
        }
        #endregion
        public ViewModel.AudioFile AudioFile { get; set; }
        public BLL.Sql.Models.AudioFile ModelAudioFile { get; set; }
        public Speech2TextPage()
        {
            InitializeComponent();
            this.BindingContext = this;            
        }
        protected override void OnAppearing()
        {
            InitialVariable();
            base.OnAppearing();
        }
        private async void InitialVariable()
        {
            try
            {
                AudioIsPrepared = true;
                Text = string.Empty;
                IsDownloded = false;
                IsPlaying = false;
                AudioFile = new ViewModel.AudioFile();
                ModelAudioFile = new BLL.Sql.Models.AudioFile();
                AudioIsPrepared = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
        }

        private void MainButton(object sender, EventArgs e)
        {
            if (!IsDownloded)
            {
                GetAudio();
            }
            else
            {
                PausePlayingAudio();
            }
        }
        private async void GetAudio()
        {
            AudioIsPrepared = true;
            try
            {
                var Response = await new BLL.Services.HttpExtension<BLL.Sql.Models.AudioFile>().GetReturnStatusCodeAndString("AudioFiles", AppStatics.Token); ;
                if (Response.statusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelAudioFile = Response.body;
                    AudioFile.PathFile = ModelAudioFile.Path;
                    if (!string.IsNullOrEmpty(ModelAudioFile?.Path))
                    {
                        DependencyService.Get<DependenciesServices.IDownloader>().DownloadFile(
                           ModelAudioFile?.Path,
                           Xamarin.Essentials.FileSystem.CacheDirectory,
                           "audio_file.wav");
                        DependencyService.Get<DependenciesServices.IDownloader>().OnFileDownloaded += (se, ex) =>
                        {
                            if (ex.FileSaved)
                            {
                                AudioFile.PathFile = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory,"audio_file.wav");
                                //LoadFile(AudioFile.PathFile);
                                //AudioFile.AudioPlayer = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                                DependencyService.Get<AudioPlayer.IAudioPlayer>().PlaybackEnded += StopAudioFile;
                                DependencyService.Get<AudioPlayer.IAudioPlayer>().Load(AudioFile.PathFile);
                                //PlayAudioFile(null, null);
                                IsDownloded = true;
                                AudioIsPrepared = false;
                            }
                        };
                    }
                }
                else if (Response.statusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await DisplayAlert("No More Audio", Response.message, "Ok");
                    AudioIsPrepared = false;
                }
                else
                {
                    await DisplayAlert("Error", Response.message, "Ok"); ;
                    AudioIsPrepared = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
                AudioIsPrepared = false;
            }

        }
        private async void PausePlayingAudio()
        {
            AudioIsPrepared = true;
            try
            {
                if (AudioFile != null && !string.IsNullOrEmpty(AudioFile.PathFile))
                {
                    DependencyService.Get<AudioPlayer.IAudioPlayer>().PlayPause();
                    IsPlaying = !IsPlaying;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
            AudioIsPrepared = false;
        }
        private async void StopAudioFile(object sender, EventArgs e)
        {
            AudioIsPrepared = true;
            try
            {
                DependencyService.Get<AudioPlayer.IAudioPlayer>().Stop();
                IsPlaying = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
            AudioIsPrepared = false;
        }
        private async void CancelOprationAudioFile(object sender, EventArgs e)
        {
            try { AudioIsPrepared = false; }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
        private async void SubmitButton(object sender, EventArgs e)
        {
            IsBusy = true;
            try
            {
                DependencyService.Get<AudioPlayer.IAudioPlayer>().Stop();
                var Duration = DependencyService.Get<IPlayService>().GetDuration(AudioFile.PathFile);
                var model = new BLL.Sql.ViewModels.TransData
                {
                    AudioFile = new BLL.Sql.ViewModels.AudioFile
                    {
                        ID = ModelAudioFile.ID
                    },
                    TransText = new BLL.Sql.Models.TransText
                    {
                        Text = Text,
                        Dialect = BLL.Enum.Dialect.Hybrid,
                    },
                    TypePost = BLL.Enum.TypePost.Text
                };
                var respones = await new BLL.Services.HttpExtension<BLL.Sql.ViewModels.TransData>().PostReturnStatusCodeAndString("TransDatas", model, AppStatics.Token);

                await Navigation.PushAsync(new Pages.StatusPage("Speech To Text", respones.statusCode == System.Net.HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            IsBusy = false;
        }
        private async void SkipButton(object sender, EventArgs e)
        {
            IsBusy = true;
            try 
            {
                DependencyService.Get<AudioPlayer.IAudioPlayer>().Stop();
                try
                {
                    var Duration = DependencyService.Get<IPlayService>().GetDuration(AudioFile.PathFile);
                    var model = new BLL.Sql.ViewModels.TransData
                    {
                        AudioFile = new BLL.Sql.ViewModels.AudioFile
                        {
                            ID = ModelAudioFile.ID,
                            Status = BLL.Enum.Status.Rejected
                        },
                        TypePost = BLL.Enum.TypePost.Text
                    };
                    var respones = await new BLL.Services.HttpExtension<BLL.Sql.ViewModels.TransData>().PostReturnStatusCodeAndString("TransDatas", model, AppStatics.Token);
                    if (respones.statusCode == System.Net.HttpStatusCode.OK)
                    {
                        InitialVariable();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.ToString(), "OK");
            }
            IsBusy = false;
        }

    }
}