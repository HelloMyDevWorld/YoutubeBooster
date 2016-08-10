using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoSamples;
using Xamarin.Forms;
using XamarinForms.Views;

namespace XamarinForms
{

    public class App : Application
    {
        public App()
        {
            //The root page of your application
            //var tabbedPage = new TabbedPage();

            // tabbedPage.Children.Add(new YoutubeViewPage());

            //MainPage = tabbedPage;

            //with toolbar
            // MainPage = new NavigationPage(new YoutubeViewPage());
            /// 
            //without toolbar
            MainPage = new YoutubeViewPage();
          
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
