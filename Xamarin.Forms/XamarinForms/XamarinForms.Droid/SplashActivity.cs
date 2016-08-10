
using Android.App;
using Android.OS;
using Android.Content;
using Xamarin.Forms;
using Android.Support.V7.App;
using System.Threading.Tasks;

namespace XamarinForms.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            Forms.SetTitleBarVisibility(AndroidTitleBarVisibility.Never);
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                Task.Delay(10000); // Simulate a bit of startup work.
            });

            startupWork.ContinueWith(t =>
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}

