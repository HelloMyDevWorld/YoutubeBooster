using System;
using Xamarin.Forms;
using VideoSamples.Library;

namespace VideoSamples
{
	public class VideoData
	{
		public VideoData()
		{

		}
		public double At {get;set;}

		public double Duration {get;set;}

		public VideoState State { get; set; }

		public override bool Equals (object obj)
		{
			var t = obj as VideoData;
			if (t == null)
				return false;

			if (t.At == this.At && t.Duration == this.Duration && State == t.State) {
				return true;
			}

			return false;
		}

		public override string ToString ()
		{
			return string.Format ("[VideoData: At={0}, Duration={1}, State={2}]", At, Duration, State);
		}

		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}
	}
}


