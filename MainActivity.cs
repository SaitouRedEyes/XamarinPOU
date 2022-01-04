using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;
using System;

namespace XamarinPOU
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IServiceConnection, IRunnable
    {

        private Count counter;
        private LifePOUServiceBinder binder;
        private bool isConnected;
        private TextView tvLifePOU;
        private Intent i;
        private Handler h;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            SetContentView(Resource.Layout.activity_main);

            isConnected = false;
            binder = null;

            tvLifePOU = (TextView)FindViewById(Resource.Id.tvTime);

            Button btnFeed = (Button)FindViewById(Resource.Id.btnFeed);

            btnFeed.Click += Feed;

            i = new Intent(this, typeof(LifePOU));
            StartService(i);
            BindService(i, this, Bind.AutoCreate);

            h = new Handler(MainLooper);
            h.Post(this);

        }
        protected override void OnResume()
        {
            base.OnResume();
            if(LifePOU.active) BindService(i, this, Bind.AutoCreate);
        }
        protected override void OnStop()
        {
            base.OnStop();
            UnbindConnection();
        }

        private void UnbindConnection()
        {
            if (binder != null)
            {
                binder = null;
                counter = null;
                UnbindService(this);
            }
            else Toast.MakeText(this, "Service is already unbind", ToastLength.Short).Show();
            
        }

        private void Feed(object sender, EventArgs e)
        {
            if (counter != null) counter.Count += 10;
            else Toast.MakeText(this, "Problems to access the service", ToastLength.Short).Show();
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            binder = (LifePOUServiceBinder)service;

            isConnected = binder != null;

            if (isConnected) counter = binder.Service;            
            else counter = null;            
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            counter = null;
            binder = null;
            isConnected = false;
        }

        public void Run()
        {
            if(counter != null) tvLifePOU.Text = counter.Count.ToString();

            h.PostDelayed(this, 30);
        }
    }
}