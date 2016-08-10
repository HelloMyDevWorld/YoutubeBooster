using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinForms.Droid.MyEntry;

[assembly: ExportRenderer (typeof(MyEntry), typeof(MyEntryRenderer))]
namespace XamarinForms.Droid.MyEntry
{
        class MyEntryRenderer : EntryRenderer
        {
            protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
            {
                base.OnElementChanged(e);

                if (Control != null)
                {
                    Control.SetBackgroundColor(global::Android.Graphics.Color.White);
                    Control.SetTextColor(global::Android.Graphics.Color.Gray);
                    //Control.SetBackgroundResource(XamarinForms.Droid.Resource.Drawable.abc_ic_search_api_mtrl_alpha);
                }
            }
        }
    
}