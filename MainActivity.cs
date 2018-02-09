using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using System.Data.SqlClient;

namespace SonraiMasterHHApp_Android
{
    [Activity(Label = "Main Screen", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            CreateDevice();

            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            FrameLayout frame1 = FindViewById<FrameLayout>(Resource.Id.frameLayout1);
            frame1.Click += Frame1_Click;
            FrameLayout frame2 = FindViewById<FrameLayout>(Resource.Id.frameLayout2);
            frame2.Click += Frame2_Click;
            FrameLayout frame3 = FindViewById<FrameLayout>(Resource.Id.frameLayout3);
            frame3.Click += Frame3_Click;
            FrameLayout frame4 = FindViewById<FrameLayout>(Resource.Id.frameLayout4);
            frame4.Click += Frame4_Click;
            FrameLayout frame5 = FindViewById<FrameLayout>(Resource.Id.frameLayout5);
            frame5.Click += Frame5_Click;
            FrameLayout frame6 = FindViewById<FrameLayout>(Resource.Id.frameLayout6);
            frame5.Click += Frame6_Click;
            FrameLayout frame7 = FindViewById<FrameLayout>(Resource.Id.frameLayout7);
            frame5.Click += Frame7_Click;
            ImageButton imgButton1 = FindViewById<ImageButton>(Resource.Id.imageButton1);
            imgButton1.Click += Frame1_Click;
            ImageButton imgButton2 = FindViewById<ImageButton>(Resource.Id.imageButton2);
            imgButton2.Click += Frame2_Click;
            ImageButton imgButton3 = FindViewById<ImageButton>(Resource.Id.imageButton3);
            imgButton3.Click += Frame3_Click;
            ImageButton imgButton4 = FindViewById<ImageButton>(Resource.Id.imageButton4);
            imgButton4.Click += Frame4_Click;
            ImageButton imgButton5 = FindViewById<ImageButton>(Resource.Id.imageButton5);
            imgButton5.Click += Frame5_Click;
            ImageButton imgButton6 = FindViewById<ImageButton>(Resource.Id.imageButton6);
            imgButton6.Click += Frame6_Click;
            ImageButton imgButton7 = FindViewById<ImageButton>(Resource.Id.imageButton7);
            imgButton7.Click += Frame7_Click;
            TextView txtView1 = FindViewById<TextView>(Resource.Id.textView1);
            txtView1.Click += Frame1_Click;
            TextView txtView2 = FindViewById<TextView>(Resource.Id.textView2);
            txtView2.Click += Frame2_Click;
            TextView txtView3 = FindViewById<TextView>(Resource.Id.textView3);
            txtView3.Click += Frame3_Click;
            TextView txtView4 = FindViewById<TextView>(Resource.Id.textView4);
            txtView4.Click += Frame4_Click;
            TextView txtView5 = FindViewById<TextView>(Resource.Id.textView5);
            txtView5.Click += Frame5_Click;
            TextView txtView6 = FindViewById<TextView>(Resource.Id.textView6);
            txtView6.Click += Frame6_Click;
            TextView txtView7 = FindViewById<TextView>(Resource.Id.textView7);
            txtView7.Click += Frame7_Click;
        }

        private void Frame1_Click(object sender, EventArgs e)
        {
            Intent searchIntent = new Intent(this, typeof(SearchActivity));
            StartActivity(searchIntent);
        }

        private void Frame2_Click(object sender, EventArgs e)
        {
            Intent deliveryIntent = new Intent(this, typeof(DeliveryActivity));
            StartActivity(deliveryIntent);
        }

        private void Frame3_Click(object sender, EventArgs e)
        {
            Intent exchangeIntent = new Intent(this, typeof(ExchangeActivity));
            StartActivity(exchangeIntent);
        }

        private void Frame4_Click(object sender, EventArgs e)
        {
            Intent workOrderIntent = new Intent(this, typeof(WorkOrderActivity));
            StartActivity(workOrderIntent);
        }

        private void Frame5_Click(object sender, EventArgs e)
        {
            Intent settingsIntent = new Intent(this, typeof(SettingsActivity));
            StartActivity(settingsIntent);
        }

        private void Frame6_Click(object sender, EventArgs e)
        {
            Intent removalIntent = new Intent(this, typeof(RemovalActivity));
            StartActivity(removalIntent);
        }

        private void Frame7_Click(object sender, EventArgs e)
        {
            Intent repairIntent = new Intent(this, typeof(RepairActivity));
            StartActivity(repairIntent);
        }

        private void CreateDevice()
        {
            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            conn.Open();

            SqlCommand command = new SqlCommand("Select COUNT(*) from [HHDevice] where ProgramName='" + Resource.String.program_code + "' AND DeviceID='" + Resource.String.truck_number + "'", conn);
            Int32 count = Convert.ToInt32(command.ExecuteScalar());

            if (count < 1)
            {
                command = new SqlCommand("INSERT INTO HHDevice (ProgramName, DeviceID) VALUES ('" + Resource.String.program_code + "', " + "'" + Resource.String.truck_number + "');", conn);
                conn.Close();
            }
            else
            {
                //Do Nothing.
            }
            
        }
    }
}

