using Android.Content;
using Android.OS;
using BackgroundStreamingAudio.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class YoutubeDetailsViewPage : ContentPage
    {
        private ListView listView2;
        private bool play = true;
        private bool pause = true;
        private Button online;
        public static ObservableCollection<ItemForList> urlList;
        public YoutubeDetailsViewPage()
        {
            urlList = new ObservableCollection<ItemForList>();
            Title = "Youtube";
            BackgroundColor = Color.White;

            online = new Button()
            {
                FontSize = 22,
                Text = "Stop",
                TextColor = Color.White,
                BackgroundColor = Color.Red,
            };

            var url = @"/data/data/com.youtube.xam/files/";
            var format = ".mp4";
            foreach (var el in Directory.GetFiles(url))
            {
                if (el.ToString().Contains(format))
                {
                    int i = el.ToString().IndexOf(url);
                    var title = el.ToString().Substring(i + url.Length).TrimEnd(format.ToCharArray());
                    urlList.Add(new ItemForList() { Title = title, URL = el.ToString() });
                }
            }
     
             listView2 = new ListView
            {
                ItemsSource = urlList,
                ItemTemplate = new DataTemplate(() =>
                {
            
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Title");

                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                                {
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 10,
                                        Children =
                                        {
                                            nameLabel
                                        }
                                        }
                                }
                        }
                    };
                })
            };

            var play_pause = new Button {
                FontSize = 21,
                BackgroundColor = Color.Red,
                TextColor = Color.White,
                Text = "Stopped"
            };
            play_pause.Clicked += (s, e) =>
            {
                if (pause)
                {
                    Forms.Context.StartService(new Intent(StreamingBackgroundService.ActionPause));
                    pause = false;
                    play_pause.BackgroundColor = Color.Yellow;
                    play_pause.Text = "Paused";
                }
                else
                {
                    Forms.Context.StartService(new Intent(StreamingBackgroundService.ActionPlay));
                    pause = true;
                    play_pause.BackgroundColor = Color.Green;
                    play_pause.Text = "Playing";
                }
            };
           
            var delete = new Button {
                FontSize = 21,
                BackgroundColor = Color.Red,
                TextColor = Color.White,
                Text = "Delete"
            };
            delete.Clicked += (s, e) =>
             {
                 var item = (Button)s;
                 File.Delete(item.CommandParameter.ToString());
                 urlList.Remove(urlList.Where(d => d.URL == item.CommandParameter.ToString()).First());
                 Forms.Context.StartService(new Intent(StreamingBackgroundService.ActionStop));
                 play_pause.BackgroundColor = Color.Red;
                 play_pause.Text = "Stopped";
             };

            Content = new StackLayout
            {
                Padding = new Thickness(5, 10),
                Children =
                {
                    play_pause,
                    listView2,
                    delete
                }
            };

            listView2.ItemTapped += (s, i) =>
            {
                listView2.SelectedItem = null;
                listView2.IsEnabled = false;
                listView2.IsVisible = false;
                var Item = i.Item as ItemForList;
                delete.BindingContext = Item;
                delete.SetBinding<ItemForList>(Button.CommandParameterProperty, d => d.URL, BindingMode.TwoWay);
                StreamingBackgroundService.initial_path(Item.URL);
                Forms.Context.StartService(new Intent(StreamingBackgroundService.ActionStop));
                Forms.Context.StartService(new Intent(StreamingBackgroundService.ActionPlay));
                play_pause.BackgroundColor = Color.Green;
                play_pause.Text = "Playing";
                listView2.IsEnabled = true;
                listView2.IsVisible = true;
            };
        }
    }
}
