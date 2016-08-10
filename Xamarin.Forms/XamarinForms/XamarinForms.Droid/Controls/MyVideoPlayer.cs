using System;
using Xamarin.Forms;
using VideoSamples.Library;

namespace VideoSamples.Controls
{
	public class MyVideoPlayer : View
	{
		public delegate void Tap(MyVideoPlayer player, bool IsDoubleTap);

		public event Tap OnTap;

		public MyVideoPlayer ()
		{

		}

		public void FireTap(bool IsDoubleTap)
		{
			var handler = this.OnTap;
			if (handler != null) {
				handler (this, IsDoubleTap);
			}
		}
        public enum ScreenOrientation
        {
            PORTRAIT,
            LANDSCAPE
        }

        public ScreenOrientation Orientation
        {
            get;
            set;
        }

        public void OrientationChanged()
        {
            OnPropertyChanged("OrientationChanged");
        }

        public static readonly BindableProperty ActionBarHideProperty = 
			BindableProperty.Create("ActionBarHide", typeof(bool), typeof(MyVideoPlayer), false);
		
		public static readonly BindableProperty ContentHeightProperty = 
			BindableProperty.Create ("ContentHeight", typeof(double), typeof(MyVideoPlayer), 0D);
		
		public static readonly BindableProperty ContentWidthProperty = 
			BindableProperty.Create("ContentWidth", typeof(double), typeof(MyVideoPlayer), 0D);

		public static readonly BindableProperty AutoPlayProperty = 
			BindableProperty.Create("AutoPlay", typeof(bool), typeof(MyVideoPlayer), false);	

		public static readonly BindableProperty FitInWindowProperty = 
			BindableProperty.Create("FitInWindow", typeof(bool), typeof(MyVideoPlayer), true);

		public static readonly BindableProperty FullScreenProperty = 
			BindableProperty.Create("FullScreen", typeof(bool), typeof(MyVideoPlayer), false);

		public static readonly BindableProperty HasErrorProperty = 
			BindableProperty.Create("HasError", typeof(bool), typeof(MyVideoPlayer), false);

		public static readonly BindableProperty ErrorMessageProperty = 
			BindableProperty.Create("ErrorMessage", typeof(string), typeof(MyVideoPlayer), "");		
		
		public static readonly BindableProperty AddVideoControllerProperty = 
			BindableProperty.Create("AddVideoController", typeof(bool), typeof(MyVideoPlayer), false);

		public static readonly BindableProperty FileSourceProperty = 
			BindableProperty.Create("FileSource", typeof(string), typeof(MyVideoPlayer), "");
		
		public static readonly BindableProperty StateProperty = 
			BindableProperty.Create("State", typeof(VideoState), typeof(MyVideoPlayer), VideoState.NONE);

		public static readonly BindableProperty SeekProperty = 
			BindableProperty.Create("Seek", typeof(double), typeof(MyVideoPlayer), -1D);

		public static readonly BindableProperty InfoProperty = 
			BindableProperty.Create("Info", typeof(VideoData), typeof(MyVideoPlayer),  null, BindingMode.OneWay);

		public static readonly BindableProperty PlayerActionProperty = 
			BindableProperty.Create("PlayerAction", typeof(VideoState), typeof(MyVideoPlayer), VideoState.NONE);

		public bool ActionBarHide
		{
			get { return (bool)GetValue (ActionBarHideProperty); }
			set { 
				SetValue (ActionBarHideProperty, value);
			}
		}

		public string ErrorMessage
		{
			get { return (string)GetValue (ErrorMessageProperty); }
			set { 
				SetValue (ErrorMessageProperty, value);
			}
		}

		public double ContentHeight
		{
			get { return (double)GetValue (ContentHeightProperty); }
			set { 
				SetValue (ContentHeightProperty, value);
			}
		}

		public double ContentWidth
		{
			get { return (double)GetValue (ContentWidthProperty); }
			set { 
				SetValue (ContentWidthProperty, value);
			}
		}

		public VideoData Info
		{
			get { return (VideoData)GetValue (InfoProperty); }
			set { 
				if (value != Info) {
					SetValue (InfoProperty, value);
				}
			}
		}


		public bool AddVideoController
		{
			get { return (bool)GetValue (AddVideoControllerProperty); }
			set { 
				SetValue (AddVideoControllerProperty, value);
			}
		}

		public bool FullScreen
		{
			get { return (bool)GetValue (FullScreenProperty); }
			set { 
				SetValue (FullScreenProperty, value);
			}
		}

		public bool AutoPlay
		{
			get { return (bool)GetValue (AutoPlayProperty); }
			set { 
				SetValue (AutoPlayProperty, value);
			}
		}

		public double Seek {
			get { return (double)GetValue (SeekProperty); }
			set { 
				if (value != Seek) {
					SetValue (SeekProperty, value); 
				} else {
					OnPropertyChanged (SeekProperty.PropertyName);
				}
			}
		}

		public VideoState PlayerAction {
			get { return (VideoState)GetValue (PlayerActionProperty); }
			set { 

				if (value != PlayerAction) {
					SetValue (PlayerActionProperty, value); 
				} else {
					OnPropertyChanged (PlayerActionProperty.PropertyName);
				}
			}
		}

		public bool HasError {
			get { return (bool)GetValue (HasErrorProperty); }
			set { 
				SetValue (HasErrorProperty, value); 
			}
		}

		public VideoState State {
			get { return (VideoState)GetValue (StateProperty); }
			set { SetValue (StateProperty, value); }
		}

		public string FileSource {
			get { return (string)GetValue (FileSourceProperty); }
			set { SetValue (FileSourceProperty, value); }
		}
	}
}

