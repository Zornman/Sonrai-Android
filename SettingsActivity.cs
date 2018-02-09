using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SonraiMasterHHApp_Android
{
    [Activity(Label = "Settings")]
    public class SettingsActivity : Activity
    {
        Button button1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            EditText programCode = FindViewById<EditText>(Resource.Id.programCode);
            EditText deviceName = FindViewById<EditText>(Resource.Id.deviceName);

            programCode.Text = Resources.GetString(Resource.String.program_code);
            deviceName.Text = Resources.GetString(Resource.String.truck_number);

            TextView txtView1 = FindViewById<TextView>(Resource.Id.textView1);

            button1 = FindViewById<Button>(Resource.Id.button1);
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Intent mainIntent = new Intent(this, typeof(MainActivity));
            StartActivity(mainIntent);
        }
    }
}