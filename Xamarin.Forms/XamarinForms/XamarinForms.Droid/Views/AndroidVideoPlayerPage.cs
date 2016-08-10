using System;
using Xamarin.Forms;
using VideoSamples.Views;
using VideoSamples.Controls;
using BackgroundStreamingAudio.Services;
using Android.Content;
using Android.Views;
using YoutubeExtractor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using XamarinForms.Models;
using System.ComponentModel;
using XamarinForms.Views;
using XamarinForms.ViewModels;

namespace VideoSamples
{
	public class AndroidVideoPlayerPage : ContentPage
	{
		private VideoPlayerView player;
        private Label label, descriptionLabel;
        private VideoDownloader videoDownloader;
        private YoutubeItem youtubeItem;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
            
        }
        public AndroidVideoPlayerPage(YoutubeItem youtubeItem)
		{
            this.youtubeItem = youtubeItem;

            BackgroundColor = Color.White;
            label = new Label
            {
                TextColor = Color.Black,
                FontSize = 18,
                Text = youtubeItem.Title,
            };

            descriptionLabel = new Label
            {
                TextColor = Color.Gray,
                FontSize = 14,
                Text = youtubeItem.Description,
            };

            var SavePath = @"/data/data/com.youtube.xam/files/" + YoutubeViewModel.RemoveIllegalPathCharacters(youtubeItem.Title) + ".mp4";
            player = new VideoPlayerView();
            player.VideoPlayer.AddVideoController = true;
            player.VideoPlayer.FileSource = SavePath;
            player.VideoPlayer.AutoPlay = false;
            Forms.Context.StartService(new Intent(StreamingBackgroundService.ActionStop));


            ScrollView scrollView = new ScrollView {
                Content = descriptionLabel,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(5, 0),
            };

            this.Content = new StackLayout
			{				
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children =  
				{
                    label,
                    player,
                    scrollView
                }
			};
		}

        protected override void OnSizeAllocated (double width, double height)
		{
			this.player.VideoPlayer.ContentHeight = height;
			this.player.VideoPlayer.ContentWidth = width;
			if (width < height) {
				this.player.VideoPlayer.Orientation = VideoSamples.Controls.MyVideoPlayer.ScreenOrientation.PORTRAIT;
                player.HeightRequest = 250;
                this.label.IsVisible = true;
                this.descriptionLabel.IsVisible = true;
            } else {
				this.player.VideoPlayer.Orientation = VideoSamples.Controls.MyVideoPlayer.ScreenOrientation.LANDSCAPE;
                this.player.HeightRequest = height;
                this.label.IsVisible = false;
                this.descriptionLabel.IsVisible = false;
            }
			this.player.VideoPlayer.OrientationChanged ();
			base.OnSizeAllocated (width, height);
		}
	}
}


