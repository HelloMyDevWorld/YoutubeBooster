using Android.App;
using System;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using XamarinForms.Droid;
using Android.Graphics;

namespace BackgroundStreamingAudio.Services
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop })]
    public class StreamingBackgroundService : Service, AudioManager.IOnAudioFocusChangeListener
    {
        //Actions
        public const string ActionPlay = "com.youtube.xam.action.PLAY";
        public const string ActionPause = "com.youtube.xam.action.PAUSE";
        public const string ActionStop = "com.youtube.xam.action.STOP";

        private static string Mp3;

        private MediaPlayer player;
        private AudioManager audioManager;
        private WifiManager wifiManager;
        private WifiManager.WifiLock wifiLock;
        private bool paused;

        private const int NotificationId = 1;

        public static void initial_path(string path)
        {
            Mp3 = path;
        }
        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            audioManager = (AudioManager)GetSystemService(AudioService);
            wifiManager = (WifiManager)GetSystemService(WifiService);
        }

        /// <summary>
        /// Don't do anything on bind
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            switch (intent.Action) {
                case ActionPlay: Play(); break;
                case ActionStop: Stop(); break;
                case ActionPause: Pause(); break;
            }

            //Set sticky as we are a long running operation
            return StartCommandResult.Sticky;
        }

        private void IntializePlayer()
        {
            player = new MediaPlayer();

            //Tell our player to sream music
            player.SetAudioStreamType(Stream.Music);

            //Wake mode will be partial to keep the CPU still running under lock screen
            player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);

            //When we have prepared the song start playback
            player.Prepared += (sender, args) => player.Start();

            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            player.Completion += (sender, args) => Stop();

            player.Error += (sender, args) =>
            {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
                Stop();//this will clean up and reset properly.
            };
        }

        private async void Play()
        {

            if (paused && player != null) {
                paused = false;
                //We are simply paused so just start again
                player.Start();
                StartForeground();
                return;
            }

            if (player == null) {
              IntializePlayer();
            }

            if (player.IsPlaying)
                return;

            try {
                await player.SetDataSourceAsync(ApplicationContext, Android.Net.Uri.Parse(Mp3));
                
                var focusResult = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted)
                {
                    //could not get audio focus
                    Console.WriteLine("Could not get audio focus");
                }
                
                player.PrepareAsync();
                AquireWifiLock();
                StartForeground();
            }
            catch (Exception ex) {
                //unable to start playback log error
                Console.WriteLine("Unable to start playback: " + ex);
            }
        }

        private void StartForeground()
        {

            Intent yesReceive = new Intent();
            yesReceive.SetAction(ActionPause);
            Bundle yesBundle = new Bundle();
            yesBundle.PutInt("userAnswer", 1);//This is the value I want to pass
            yesReceive.PutExtras(yesBundle);
            PendingIntent pendingIntentYes = PendingIntent.GetBroadcast(this, 12345, yesReceive, PendingIntentFlags.UpdateCurrent);

            Notification.Action action = new Notification.Action(Resource.Drawable.ic_media_pause, "Yes", pendingIntentYes);

            Notification.Builder builder = new Notification.Builder(this);
         //  .SetContentTitle("Youtube Music")
        //   .SetContentText("Fell the music!")
        //   .SetSmallIcon(Resource.Drawable.ic_media_play);

            // Build the notification:
            Notification notification = builder.Build();
             notification.Flags |= NotificationFlags.OngoingEvent;
           // notification.Actions.Add(action);

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
            StartForeground(notificationId, notification);
            /**
            var Intent = new Intent(this, typeof(MainActivity));
                  Intent.SetFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0,
                Intent,
                PendingIntentFlags.UpdateCurrent);

            var notification = new Notification
            {
                TickerText = new Java.Lang.String("Song started!"),
                Icon = Resource.Drawable.ic_media_play
            };
            notification.Flags |= NotificationFlags.OngoingEvent;
            notification.SetLatestEventInfo(ApplicationContext, "Xamarin Streaming",
                            "Playing music!", pendingIntent);
            StartForeground(NotificationId, notification);
    **/

        }

        private void Pause()
        {
            if (player == null)
                return;

            if(player.IsPlaying)
                player.Pause();

            StopForeground(true);
            paused = true;
        }

        private void Stop()
        {
            if (player == null)
                return;

            if(player.IsPlaying)
                player.Stop();

            player.Reset();
            paused = false;
            StopForeground(true);
            ReleaseWifiLock();
        }

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        private void AquireWifiLock()
        {
            if (wifiLock == null){
                wifiLock = wifiManager.CreateWifiLock(WifiMode.Full, "xamarin_wifi_lock");
            } 
            wifiLock.Acquire();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private void ReleaseWifiLock()
        {
            if (wifiLock == null)
                return;

            wifiLock.Release();
            wifiLock = null;
        }

        /// <summary>
        /// Properly cleanup of your player by releasing resources
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (player != null)
            {
                player.Release();
                player = null;
            }
        }

        /// <summary>
        /// For a good user experience we should account for when audio focus has changed.
        /// There is only 1 audio output there may be several media services trying to use it so
        /// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
        /// All applications are encouraged to follow this, but are not enforced.
        /// </summary>
        /// <param name="focusChange"></param>
        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    if (player == null)
                        IntializePlayer();

                    if (!player.IsPlaying)
                    {
                        player.Start();
                        paused = false;
                    }
                    
                    player.SetVolume(1.0f, 1.0f);//Turn it up!
                    break;
                case AudioFocus.Loss:
                    //We have lost focus stop!
                    Stop();
                    break;
                case AudioFocus.LossTransient:
                    //We have lost focus for a short time, but likely to resume so pause
                    Pause();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should till play at a muted 10% volume
                    if(player.IsPlaying)
                        player.SetVolume(.1f, .1f);//turn it down!
                    break;

            }
        }
    }
}
