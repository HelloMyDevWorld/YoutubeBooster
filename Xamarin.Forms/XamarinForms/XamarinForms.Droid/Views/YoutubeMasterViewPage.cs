using Android.Content;
using Android.OS;
using BackgroundStreamingAudio.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoSamples;
using Xamarin.Forms;
using XamarinForms.Droid.Models;
using XamarinForms.Droid.MyEntry;
using XamarinForms.Models;
using XamarinForms.ViewModels;
using YoutubeExtractor;

namespace XamarinForms.Views
{
    public class YoutubeMasterViewPage : ContentPage
    {
        public MyEntry searchEntry;
        public ListView listView;
        public bool play = true;
        DataTemplate dataTemplate;
        private VideoDownloader videoDownloader;
        public YoutubeMasterViewPage()
        {
            BackgroundColor = Color.White;

            searchEntry = new MyEntry
            {
                Placeholder = "Search",
                HeightRequest = 45,
                TextColor = Color.Gray,
            };

            var label = new Label
            {
                Text = "Youtube Booster",
                TextColor = Color.Black,
                FontSize = 24
            };

            listView = new ListView
            {
                HasUnevenRows = true
            };

            dataTemplate = new DataTemplate(() =>
            {
                var titleLabel = new Label
                {
                    TextColor = Color.Black,
                    FontSize = 22,
                    FontAttributes = FontAttributes.Bold
                };
                var viewCountLabel = new Label
                {
                    TextColor = Color.FromHex("#0D47A1"),
                    FontSize = 14
                };
                var likeCountLabel = new Label
                {
                    TextColor = Color.FromHex("#2196F3"),
                    FontSize = 14,
                };
                var dislikeCountLabel = new Label
                {
                    TextColor = Color.Red,
                    FontSize = 14
                };
                var favoriteCountLabel = new Label
                {
                    TextColor = Color.FromHex("#2196F3"),
                    FontSize = 14
                };
                var mediaImage = new Image
                {
                    HeightRequest = 200
                };
                var downloadButton = new Button
                {
                    Text = "Download",
                    TextColor = Color.White
                };
                downloadButton.Clicked += OnDownload;

                StringToColorConverter colorconverter = new StringToColorConverter();
                StringToBoolConverter boolconverter = new StringToBoolConverter();
                downloadButton.SetBinding(Button.BackgroundColorProperty, new Binding("background", BindingMode.TwoWay, colorconverter));
                downloadButton.SetBinding(Button.CommandParameterProperty, new Binding("VideoId")); //Nazwa pliku jest taka sam jak title
                downloadButton.SetBinding(Button.IsEnabledProperty, new Binding("isenablebutton", BindingMode.TwoWay, boolconverter));
                downloadButton.SetBinding(Button.TextProperty, new Binding("textbutton"));


                titleLabel.SetBinding(Label.TextProperty, new Binding("Title"));

                mediaImage.SetBinding(Image.SourceProperty, new Binding("HighThumbnailUrl"));
                viewCountLabel.SetBinding(Label.TextProperty, new Binding("ViewCount", BindingMode.Default, null, null, "{0:n0} views"));
                likeCountLabel.SetBinding(Label.TextProperty, new Binding("LikeCount", BindingMode.Default, null, null, "{0:n0} likes"));
                dislikeCountLabel.SetBinding(Label.TextProperty, new Binding("DislikeCount", BindingMode.Default, null, null, "{0:n0} dislikes"));

                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Padding = new Thickness(5, 10),
                        Children =
                        {
                            titleLabel,
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    viewCountLabel,
                                    likeCountLabel,
                                    dislikeCountLabel,
                                }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                TranslationY = -7,
                                Children =
                                {
                                    favoriteCountLabel,
                                }
                            },
                            mediaImage,
                            downloadButton,
                        }
                    }
                };
            });

            Content = new StackLayout
                {
                
                    Padding = new Thickness(5, 10),
                    Children =
                {
                    label,
                    searchEntry,
                    listView
                }
                
                };

                searchEntry.Completed += (sender, e) => 
                {
                searchEntry.Focus();

                listView.BindingContext = new YoutubeViewModel(searchEntry.Text); // uzywaj nazw z viewModel i wsadz je w context
                listView.SetBinding(ListView.ItemsSourceProperty, "YoutubeItems"); // uzyj tego co nazywa sie "YoutubeItems" i uznaj to za ItemSource
                listView.ItemTemplate = dataTemplate; // jak wyglada szkielet CELL

                listView.ItemTapped += async (s, i) =>
                {
                    listView.SelectedItem = null;
                    listView.IsEnabled = false;
                    listView.IsVisible = false;
                    var youtubeItem = i.Item as YoutubeItem;
                    var url = @"/data/data/com.youtube.xam/files/";
                    foreach (var el in Directory.GetFiles(url))
                    {
                        if (el.ToString().Contains(YoutubeViewModel.RemoveIllegalPathCharacters(youtubeItem.Title)))
                        {
                            //FINALLY WORKING !!!!!!!!!!!!!!!!!!!!!!!!!!!!!! zapobiega double click tylko w takiej konfiguracji
                            var page = new AndroidVideoPlayerPage(youtubeItem);
                            if (App.Current.MainPage.Navigation.ModalStack.Count == 0 || App.Current.MainPage.Navigation.ModalStack.Last().GetType() != page.GetType())
                            {
                                //Navigation.PushAsync to dzialalo // wylaczyc trzeba mp3 przed ogladaniem
                                await Navigation.PushModalAsync(page, false);
                            }
                        }
                    }
                    listView.IsEnabled = true;
                    listView.IsVisible = true;
                };
            };
        }

        private void OnDownload(object sender, EventArgs e)
        {
            var item = (Button)sender;

            if (item.Text == "Download")
            {
                    item.Text = "Downloading";
                    item.BackgroundColor = Color.FromHex("#336699");
                    BackgroundWorker bg = new BackgroundWorker();
                    bg.DoWork += (s, es) =>
                    {
                        string link = String.Format("https://www.youtube.com/watch?v={0}", item.CommandParameter.ToString());
                        IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link, false);
                        VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

                        if (video.RequiresDecryption)
                            DownloadUrlResolver.DecryptDownloadUrl(video);

                        //global::Android.OS.Environment.ExternalStorageDirectory.Path
                        var SavePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), YoutubeViewModel.RemoveIllegalPathCharacters(video.Title) + video.VideoExtension);
                        videoDownloader = new VideoDownloader(video, SavePath);
                        videoDownloader.Execute();

                        YoutubeDetailsViewPage.urlList.Add(new ItemForList() { Title = video.Title, URL = SavePath });
                    };

                    bg.RunWorkerCompleted += (s, es) =>
                    {
                        item.IsEnabled= false;
                        item.BackgroundColor = Color.FromHex("#2C6700");
                        item.Text = "Downloaded";
                    };

                    bg.RunWorkerAsync();
                }

        }

    }
}
