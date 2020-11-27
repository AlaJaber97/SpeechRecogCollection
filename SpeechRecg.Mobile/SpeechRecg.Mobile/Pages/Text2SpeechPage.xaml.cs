using Plugin.AudioRecorder;
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
    public partial class Text2SpeechPage : ContentPage
    {
        #region Prop
        private bool _IsBusy;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged(); }
        }
        private bool _IsDownloded = false;
        public bool IsDownloded
        {
            get { return _IsDownloded; }
            set { _IsDownloded = value; OnPropertyChanged(); }
        }
        private bool _IsRecording = false;
        public bool IsRecording
        {
            get { return _IsRecording; }
            set { _IsRecording = value; IsRecorded = !value; OnPropertyChanged(); }
        }
        private bool _IsRecorded = false;
        public bool IsRecorded
        {
            get { return _IsRecorded; }
            set { _IsRecorded = value; OnPropertyChanged(); }
        }
        private bool _IsAudioPlaying = false;
        public bool IsAudioPlaying
        {
            get { return _IsAudioPlaying; }
            set { _IsAudioPlaying = value; OnPropertyChanged(); }
        }
        private bool _AudioIsPrepared = false;
        public bool AudioIsPrepared
        {
            get { return _AudioIsPrepared; }
            set { _AudioIsPrepared = value; OnPropertyChanged(); }
        }
        private string _Text= "Press Button Download to get new audio";

        public string Text
        {
            get { return _Text; }
            set { _Text = value; OnPropertyChanged(); }
        }

        public BLL.Sql.Models.TransText ModelText { get; set; }
        #endregion
        public ViewModel.AudioFile AudioFile { get; set; }
        public Text2SpeechPage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }
        protected override void OnAppearing()
        {
            InitialVariable();
            base.OnAppearing();
        }
        private void InitialVariable()
        {
            IsBusy = true;
            AudioIsPrepared = true;
            Text = string.Empty;
            IsDownloded = false;
            IsRecording = false;
            IsRecorded = false;
            IsAudioPlaying = false;
            AudioFile = new ViewModel.AudioFile();
            ModelText = new BLL.Sql.Models.TransText();
            AudioIsPrepared = false;
            IsBusy = false;
        }
        private void MainButton(object sender, EventArgs e)
        {
            try
            {
                if (!IsDownloded)
                {
                    GetText();
                    return;
                }
                else
                {
                    if (!IsRecorded && !IsRecording)
                    {
                        StartRecordeAudio(sender, e);
                        return;
                    }
                    else
                    {
                        if (IsRecorded)
                        {
                            PausePlayingAudio(sender, e);
                            return;
                        }

                        if (IsRecording)
                        {
                            StopRecordeAudio(sender, e);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
        private async void StartRecordeAudio(object sender, EventArgs e)
        {
            AudioIsPrepared = true;
            try
            {
                AudioFile.AudioRecorder = new AudioRecorderService
                {
                    StopRecordingOnSilence = false
                };
                if (!AudioFile.AudioRecorder.IsRecording)
                {
                    await AudioFile.AudioRecorder.StartRecording(); ;
                    IsRecording = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
            AudioIsPrepared = false;
        }
        private async void StopRecordeAudio(object sender, EventArgs e)
        {
            AudioIsPrepared = true;
            try
            {
                if (AudioFile.AudioRecorder.IsRecording)
                {
                    await AudioFile.AudioRecorder.StopRecording();
                    IsRecording = false;
                    AudioFile.PathFile = AudioFile.AudioRecorder.FilePath;
                    if (!string.IsNullOrEmpty(AudioFile.PathFile))
                    {
                        DependencyService.Get<AudioPlayer.IAudioPlayer>().PlaybackEnded += StopPlayingAudio;
                        DependencyService.Get<AudioPlayer.IAudioPlayer>().Load(AudioFile.PathFile);
                        IsRecorded = true;
                    }
                }
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
        private async void PausePlayingAudio(object sender, EventArgs e)
        {
            AudioIsPrepared = true;
            try
            {
                if (AudioFile != null && !string.IsNullOrEmpty(AudioFile.PathFile))
                {
                    DependencyService.Get<AudioPlayer.IAudioPlayer>().PlayPause();
                    IsAudioPlaying = !IsAudioPlaying;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
            AudioIsPrepared = false;
        }
        private async void StopPlayingAudio(object sender, EventArgs e)
        {
            AudioIsPrepared = true;
            try
            {
                if (AudioFile != null)
                {
                    DependencyService.Get<AudioPlayer.IAudioPlayer>().Stop();
                    IsAudioPlaying = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
            AudioIsPrepared = false;
        }
        private async void GetText()
        {
            AudioIsPrepared = true;
            try
            {
                var Response = await new BLL.Services.HttpExtension<BLL.Sql.Models.TransText>().GetReturnStatusCodeAndString("TransTexts", AppStatics.Token);
                if (Response.statusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelText = Response.body;
                    Text = ModelText.Text;
                    IsDownloded = true;
                }
                else if (Response.statusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await DisplayAlert("No More Text", "no more text in database, can try Speech to Text mode", "Ok"); ;
                }
                else
                {
                    await DisplayAlert("Error", Response.message, "Ok");
                }
            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", ex.ToString(), "OK");
            }
            AudioIsPrepared = false;
        }
        private async void SubmitButton(object sender, EventArgs e)
        {
            IsBusy = true;
            try
            {
                var Duration = DependencyService.Get<IPlayService>().GetDuration(AudioFile.PathFile);
                using (Stream streamfile = new MemoryStream(File.ReadAllBytes(AudioFile.PathFile)))
                {
                    var model = new BLL.Sql.ViewModels.TransData
                    {
                        AudioFile = new BLL.Sql.ViewModels.AudioFile
                        {
                            ID = Guid.NewGuid(),
                            Length = TimeSpan.FromSeconds(Duration),
                            SpeakerAge = AppStatics.User.Age,
                            SpeakerGender = AppStatics.User.Gender,
                        },
                        TransText = new BLL.Sql.Models.TransText
                        {
                            ID = ModelText.ID,
                            Dialect = ModelText.Dialect,
                            Text = this.Text,
                        },
                        TypePost = BLL.Enum.TypePost.Audio
                    };
                    var result = await BLL.Services.UploadFile.UploadAudioAsync(model.AudioFile.ID, AppStatics.Token, streamfile, "Mobile");
                    if (result.statusCode == System.Net.HttpStatusCode.Created)
                    {
                        var respones = await new BLL.Services.HttpExtension<BLL.Sql.ViewModels.TransData>().PostReturnStatusCodeAndString("TransDatas", model, AppStatics.Token);
                        if (respones.statusCode == System.Net.HttpStatusCode.OK)
                        {
                            await Navigation.PushAsync(new Pages.StatusPage("Text To Speech", respones.statusCode == System.Net.HttpStatusCode.OK));
                        }
                        else
                        {
                            throw new Exception(string.IsNullOrEmpty(respones.message) ? respones.message : "An error occurred while uploading the file (when trying to update file information)");
                        }
                    }
                    else
                    {
                        throw new Exception(string.IsNullOrEmpty(result.message) ? result.message : "An error occurred while uploading the file");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",ex.ToString(),"OK");
            }
            IsBusy = false;
        }
        private async void SkipButton(object sender, EventArgs e)
        {
            IsBusy = true;
            try
            {
                await AudioFile.AudioRecorder.StopRecording();
                DependencyService.Get<AudioPlayer.IAudioPlayer>().Stop();
                var model = new BLL.Sql.ViewModels.TransData
                {
                    TransText = new BLL.Sql.Models.TransText
                    {
                        ID = ModelText.ID,
                        Status = BLL.Enum.Status.Rejected
                    },
                    TypePost = BLL.Enum.TypePost.Audio
                };
                var respones = await new BLL.Services.HttpExtension<BLL.Sql.ViewModels.TransData>().PostReturnStatusCodeAndString("TransDatas", model, AppStatics.Token);
                if(respones.statusCode == System.Net.HttpStatusCode.OK)
                    InitialVariable();
            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", ex.ToString(), "OK");
            }
            IsBusy = false;
        }
    }
}