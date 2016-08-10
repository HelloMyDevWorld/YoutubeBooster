using System;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Android.Media;
using Android.Content.Res;
using Android.Widget;
using Xamarin.Forms;
using VideoSamples.Controls;
using VideoSamples.Library;
using Android.Runtime;
using Android.App;
using Android.Util;
using Android.Graphics;
using Android.Content;
using BackgroundStreamingAudio.Services;

[assembly: ExportRenderer(typeof(MyVideoPlayer), typeof(VideoSamples.Droid.MyVideoPlayerRenderer))]
namespace VideoSamples.Droid
{
	public class MyVideoPlayerRenderer : ViewRenderer<MyVideoPlayer, Android.Widget.RelativeLayout>
    {
		private MediaController _MCController;
		private MyVideoView _MyVideoView;
		private bool _AttachedController;
		private Android.Widget.RelativeLayout _MainLayout;
        
        public MyVideoPlayerRenderer ()
		{
			this._AttachedController = false;
		}


		protected override void Dispose (bool disposing)
		{
			if (this._MCController != null && this._MyVideoView != null) {
				this._MyVideoView.SetMediaController (null);
			}
			if (this._MCController != null) {
				this._MCController.Dispose ();
				this._MCController = null;
			}

			if (this._MyVideoView != null) {
				this._MyVideoView.StopPlayback ();
				this._MyVideoView.Dispose ();
				this._MyVideoView = null;
			}
			base.Dispose (disposing);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<MyVideoPlayer> e)
		{
			base.OnElementChanged (e);
			if (this.Control == null) {	
				var layoutInflater = (LayoutInflater)Context.GetSystemService(global::Android.Content.Context.LayoutInflaterService);
				this._MainLayout = (Android.Widget.RelativeLayout)layoutInflater.Inflate (XamarinForms.Droid.Resource.Layout.VideoLayout, null);	
				SetNativeControl (this._MainLayout);
			}

			this._MyVideoView = this.Control.FindViewById<MyVideoView>(XamarinForms.Droid.Resource.Id.videoView1);

			this._MyVideoView.ParentElement = this.Element;

			this._MCController = new MediaController (this.Context);
			this._MCController.SetMediaPlayer (this._MyVideoView);
      
            if (this.Element.AddVideoController) {				
				this._AttachedController = true;
				this._MyVideoView.SetMediaController (this._MCController);
			} else {
				this._AttachedController = false;
			}

      
            this._MyVideoView.LoadFile (this.Element.FileSource);

		}	
		
	}
}

