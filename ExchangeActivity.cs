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
using System.Data;

namespace SonraiMasterHHApp_Android
{
    [Activity(Label = "ExchangeActivity")]
    public class ExchangeActivity : Activity
    {
        TextView txtView1;
        TextView txtView2;

        EditText editText1;
        EditText editText2;

        Button btnBack;
        Button btnSubmit;
        Button btnCartData;

        //FrameLayout frameLayout1;
        //FrameLayout frameLayout2;
        //FrameLayout frameLayout3;
        //FrameLayout frameLayout4;

        //RadioGroup radioGroup1;
        //RadioGroup radioGroup2;
        //RadioGroup radioGroup3;
        //RadioGroup radioGroup4;

        RadioButton radioButton1;
        RadioButton radioButton2;
        RadioButton radioButton3;
        RadioButton radioButton4;
        RadioButton radioButton5;
        RadioButton radioButton6;
        RadioButton radioButton7;
        RadioButton radioButton8;
        RadioButton radioButton9;
        RadioButton radioButton10;

        dataLayer DA = new dataLayer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Exchange);

            txtView1 = FindViewById<TextView>(Resource.Id.txtView1);
            txtView2 = FindViewById<TextView>(Resource.Id.txtView2);

            editText1 = FindViewById<EditText>(Resource.Id.editText1);
            editText2 = FindViewById<EditText>(Resource.Id.editText2);

            radioButton1 = FindViewById<RadioButton>(Resource.Id.radioButton1);
            radioButton2 = FindViewById<RadioButton>(Resource.Id.radioButton2);
            radioButton3 = FindViewById<RadioButton>(Resource.Id.radioButton3);
            radioButton4 = FindViewById<RadioButton>(Resource.Id.radioButton4);
            radioButton5 = FindViewById<RadioButton>(Resource.Id.radioButton5);
            radioButton6 = FindViewById<RadioButton>(Resource.Id.radioButton6);
            radioButton7 = FindViewById<RadioButton>(Resource.Id.radioButton7);
            radioButton8 = FindViewById<RadioButton>(Resource.Id.radioButton8);
            radioButton9 = FindViewById<RadioButton>(Resource.Id.radioButton9);
            radioButton10 = FindViewById<RadioButton>(Resource.Id.radioButton10);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            FindViewById<Button>(Resource.Id.btnBack).Click += BtnBack_Click;

            btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);
            FindViewById<Button>(Resource.Id.btnSubmit).Click += BtnSubmit_Click;

            btnCartData = FindViewById<Button>(Resource.Id.btnCartData);
            btnCartData.Click += BtnCartData_Click;
        }

        private void BtnCartData_Click(object sender, EventArgs e)
        {
            PopulateData();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent MainIntent = new Intent(this, typeof(MainActivity));
            StartActivity(MainIntent);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            if (editText1.Text == "" || editText2.Text == "")
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("Please make sure you have entered both RFID values.");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Thank you!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                if (!DA.ExchangeCarts(editText1.Text, editText2.Text))
                {
                    alert.SetTitle("ERROR");
                    alert.SetMessage("Old RFID Value is not in our system, please try again with a different value.");

                    alert.SetPositiveButton("OK", (senderAlert, args) =>
                    {
                        Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else
                {
                    alert.SetTitle("COMPLETE");
                    alert.SetMessage("RFID values have been updated!");

                    alert.SetPositiveButton("OK", (senderAlert, args) =>
                    {
                        Toast.MakeText(this, "RFID Updated!", ToastLength.Short).Show();

                        editText2.Text = "";
                        editText1.Text = "";
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                
            }
        }

        void PopulateData()
        {
            if (editText1.Text != null)
            {
                DataTable dt = new DataTable();
                dt = DA.OldRFIDCartInformation(editText1.Text);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "35")
                    {
                        radioButton1.PerformClick();
                    }
                    else if (dt.Rows[0][0].ToString() == "65")
                    {
                        radioButton2.PerformClick();
                    }
                    else if (dt.Rows[0][0].ToString() == "95")
                    {
                        radioButton3.PerformClick();
                    }

                    if (dt.Rows[0][1].ToString() == "TRASH")
                    {
                        radioButton4.PerformClick();
                    }
                    else if (dt.Rows[0][1].ToString() == "RECYCLING")
                    {
                        radioButton5.PerformClick();
                    }
                }
            }
            else
            {
                //Do nothing
            }

            if (editText2.Text != null)
            {
                DataTable dt = new DataTable();
                dt = DA.NewRFIDCartInformation(editText2.Text);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "35")
                    {
                        radioButton6.PerformClick();
                    }
                    else if (dt.Rows[0][0].ToString() == "65")
                    {
                        radioButton7.PerformClick();
                    }
                    else if (dt.Rows[0][0].ToString() == "95")
                    {
                        radioButton8.PerformClick();
                    }

                    if (dt.Rows[0][1].ToString() == "TRASH")
                    {
                        radioButton9.PerformClick();
                    }
                    else if (dt.Rows[0][1].ToString() == "RECYCLING")
                    {
                        radioButton10.PerformClick();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Make sure to specify cart size and type for new cart!", ToastLength.Long).Show();
                }
            }
            else
            {
                //Do nothing
            }
        }
    }
}