using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinPOU
{
    class LifePOUServiceBinder : Binder
    {
        public LifePOUServiceBinder(LifePOU service)
        {
            this.Service = service;
        }

        public LifePOU Service { get; private set; }    
    }
}