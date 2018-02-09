using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SonraiMasterHHApp_Android
{
    [Activity(Label = "SearchActivity")]
    public class SearchActivity : Activity
    {
        //Creates all the buttons on the page
        Button btnBack;
        Button btnSearch;
        Button btnClearInfo;

        //Creates the user input text fields
        EditText rfidValue;
        EditText customerName;
        EditText customerAddress;
        EditText customerCity;
        EditText customerState;
        EditText customerZip;
        EditText cartSerial;
        EditText cartType;
        EditText cartSize;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //When the page is created, sets the content view to Search.axml
            SetContentView(Resource.Layout.Search);

            //Finds the element and sets it to the button we created
            btnBack = FindViewById<Button>(Resource.Id.btnBack);

            //Creates the Click event that triggers the function
            FindViewById<Button>(Resource.Id.btnBack).Click += BtnBack_Click;

            btnSearch = FindViewById<Button>(Resource.Id.btnSearch);
            FindViewById<Button>(Resource.Id.btnSearch).Click += BtnSearch_Click;

            btnClearInfo = FindViewById<Button>(Resource.Id.btnClearInfo);
            FindViewById<Button>(Resource.Id.btnClearInfo).Click += BtnClearInfo_Click;

            //Finds the element and associates that with the variable we created above
            rfidValue = FindViewById<EditText>(Resource.Id.editText1);
            customerName = FindViewById<EditText>(Resource.Id.editText2);
            customerAddress = FindViewById<EditText>(Resource.Id.editText3);
            customerCity = FindViewById<EditText>(Resource.Id.editText4);
            customerState = FindViewById<EditText>(Resource.Id.editText5);
            customerZip = FindViewById<EditText>(Resource.Id.editText6);
            cartSerial = FindViewById<EditText>(Resource.Id.editText7);
            cartType = FindViewById<EditText>(Resource.Id.editText8);
            cartSize = FindViewById<EditText>(Resource.Id.editText9);
        }

        //Sets the content view back to the Main Screen
        //Changes the .cs page to MainActivity
        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent MainIntent = new Intent(this, typeof(MainActivity));
            StartActivity(MainIntent);
        }

        //Search RFID Values in the CartMaster Database
        //Also populates fields as long as the data is in the Database
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            if (rfidValue.Text != null)
            {
                conn.Open();
                SqlCommand command = new SqlCommand("Select COUNT(*) from [CustomerBeta] where RFID='" + rfidValue.Text.TrimEnd() + "'", conn);
                Int32 count = Convert.ToInt32(command.ExecuteScalar());

                if(count >= 1)
                {
                    command = new SqlCommand("Select * from [CustomerBeta] where RFID='" + rfidValue.Text.TrimEnd() + "'", conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // iterate your results here
                        while (reader.Read())
                        {
                            if (reader.GetString(15) != null)
                                customerName.Text = reader.GetString(15);
                            else
                                customerName.Text = "Not Available";

                            if (reader.GetString(1) != null)
                                customerAddress.Text = reader.GetString(1);
                            else
                                customerAddress.Text = "Not Available";

                            if (reader.GetString(2) != null)
                                customerCity.Text = reader.GetString(2);
                            else
                                customerCity.Text = "Not Available";

                            if (reader.GetString(3) != null)
                                customerState.Text = reader.GetString(3);
                            else
                                customerState.Text = "Not Available";

                            if (reader.GetString(4) != null)
                                customerZip.Text = reader.GetString(4);
                            else
                                customerZip.Text = "Not Available";
                        }

                    }

                    command = new SqlCommand("Select * from [CartMasterBeta] where RFID='" + rfidValue.Text.TrimEnd() + "'", conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // iterate your results here
                        while (reader.Read())
                        {
                            cartSerial.Text = reader.GetString(0);
                            cartType.Text = reader.GetString(2);
                            cartSize.Text = reader.GetString(3);
                        }

                    }
                    conn.Close();
                }
                else
                {
                    alert.SetTitle("ERROR");
                    alert.SetMessage("RFID tag is not in our system, try again.");

                    alert.SetPositiveButton("OK", (senderAlert, args) => {
                        Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                    conn.Close();
                }
            }
        }

        //Sets all fields back to empty
        //Best for using the search feature more than once
        private void BtnClearInfo_Click(object sender, EventArgs e)
        {
            rfidValue.Text = "";
            customerName.Text = "";
            customerAddress.Text = "";
            customerCity.Text = "";
            customerState.Text = "";
            customerZip.Text = "";
            cartSerial.Text = "";
            cartType.Text = "";
        }
    }
}