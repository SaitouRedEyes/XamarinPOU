using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinPOU
{
    [Service(IsolatedProcess = false, Name = "com.XamarinPOU.LifePOU")]

    class LifePOU : Service, IRunnable, Count
    {
        protected int count;
        public static bool active;
        private Handler h;

        public override void OnCreate()
        {
            base.OnCreate();

            active = true;
            count = 30;
            h = new Handler(MainLooper);
            h.Post(this);
        }

        public override void OnDestroy()
        {
            Binder = null;
            active = false;

            base.OnDestroy();
        }

        public IBinder Binder { get; private set; }
        public int Count { get => count; set => count = value; }        

        public override IBinder OnBind(Intent intent)
        {
            this.Binder = new LifePOUServiceBinder(this);
            return this.Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public void Run()
        {
            if (active)
            {
                Log.Debug("Counter Service", "Count: " + count);
                count--;

                if (count == 20) SendNotification();

                h.PostDelayed(this, 1000);
            }
            else
            {
                count = 0;
                Log.Debug("Counter Service", "Service End!!");

                StopSelf();
            }
        }
        
        private void SendNotification()
        {            
            Intent resultIntent = new Intent(this, typeof(MainActivity));                        
         
            Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);

            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "location_notification")
            .SetAutoCancel(true)
            .SetContentIntent(resultPendingIntent)
            .SetContentTitle("POU Morrendoooo")            
            .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
            .SetContentText(string.Format("Alimente seu POU!!"));

            NotificationManager nf = (NotificationManager)GetSystemService(NotificationService);
            
            if (Build.VERSION.SdkInt > BuildVersionCodes.O)
            {
                string channelName = "Canal de Feed do POU";
                string channelDescription = "Canal dedicado a avisar sobre as condicoes do POU para o usuario";
                NotificationChannel nc = new NotificationChannel("location_notification", channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                nf.CreateNotificationChannel(nc);
            }
            
            NotificationManagerCompat nmc = NotificationManagerCompat.From(this);
            nmc.Notify(1000, builder.Build());
        }
    }
}