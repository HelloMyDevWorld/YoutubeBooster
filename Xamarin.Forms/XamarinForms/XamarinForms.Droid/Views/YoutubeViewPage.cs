using Android.Content;
using Android.OS;
using BackgroundStreamingAudio.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoSamples;
using Xamarin.Forms;
using XamarinForms.Models;
using XamarinForms.ViewModels;
using YoutubeExtractor;

namespace XamarinForms.Views
{
    public class YoutubeViewPage : MasterDetailPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);
            
        }
        public YoutubeViewPage()
        {
            Master = new YoutubeDetailsViewPage();
            Detail = new YoutubeMasterViewPage();
        }
    }
}
