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
    [Activity(Label = "RemovalActivity")]
    public class RemovalActivity : Activity
    {
        Button btnBack;
        Button btnRemoval;
        Button btnClearInfo;
        Button btnGetInfo;

        EditText rfidValue;
        EditText customerName;
        EditText customerAddress;
        EditText customerCity;
        EditText customerState;
        EditText customerZip;
        EditText cartSerial;
        EditText cartType;
        EditText cartSize;

        dataLayer DA = new dataLayer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Removal);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            FindViewById<Button>(Resource.Id.btnBack).Click += BtnBack_Click;
            btnClearInfo = FindViewById<Button>(Resource.Id.btnClearInfo);
            FindViewById<Button>(Resource.Id.btnClearInfo).Click += BtnClearInfo_Click;
            btnRemoval = FindViewById<Button>(Resource.Id.btnRemoval);
            FindViewById<Button>(Resource.Id.btnRemoval).Click += BtnRemoval_Click;
            btnGetInfo = FindViewById<Button>(Resource.Id.btnGetInfo);
            FindViewById<Button>(Resource.Id.btnGetInfo).Click += BtnGetInfo_Click;

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

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent MainIntent = new Intent(this, typeof(MainActivity));
            StartActivity(MainIntent);
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
            cartSize.Text = "";
        }

        private void BtnGetInfo_Click(object sender, EventArgs e)
        {
            DataTable dtCustomer = new DataTable();
            DataTable dtCart = new DataTable();
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            if (rfidValue.Text != null)
            {
                dtCustomer = DA.GetCustomerInfo(rfidValue.Text);

                if (dtCustomer.Rows.Count > 0)
                {
                    customerName.Text = dtCustomer.Rows[0][4].ToString();
                    customerAddress.Text = dtCustomer.Rows[0][0].ToString();
                    customerCity.Text = dtCustomer.Rows[0][1].ToString();
                    customerState.Text = dtCustomer.Rows[0][2].ToString();
                    customerZip.Text = dtCustomer.Rows[0][3].ToString();
                }
                else
                {
                    customerName.Text = "Info not available";
                    customerAddress.Text = "Info not available";
                    customerCity.Text = "Info not available";
                    customerState.Text = "Info not available";
                    customerZip.Text = "Info not available";
                }

                dtCart = DA.GetCartInfo(rfidValue.Text);

                if (dtCart.Rows.Count > 0)
                {
                    cartSerial.Text = dtCart.Rows[0][0].ToString();
                    cartType.Text = dtCart.Rows[0][1].ToString();
                    cartSize.Text = dtCart.Rows[0][2].ToString();
                }
                else
                {
                    cartSerial.Text = "Info not available";
                    cartType.Text = "Info not available";
                    cartSize.Text = "Info not available";
                }
            }
        }

        private void BtnRemoval_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            if (rfidValue.Text != null)
            {
                CreateWorkOrder();
            }
            else
            {
                // If count is 0, display an alert
                alert.SetTitle("ERROR");
                alert.SetMessage("RFID tag is not in our system, try again.");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        private void CreateWorkOrder()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            string address = customerAddress.Text;
            DataTable dataTable = DA.GetCustomerDataByAddress(address);

            if (dataTable.Rows.Count > 0)
            {
                alert.SetTitle("ALERT");
                alert.SetMessage("Are you sure you want to remove this cart from this customer?");

                alert.SetPositiveButton("YES", (senderAlert, args) => {
                    Toast.MakeText(this, "Removing cart from customer...", ToastLength.Short).Show();

                    DataAccess();
                });

                alert.SetNegativeButton("NO", (senderAlert, args) => {
                    Toast.MakeText(this, "Removal cancelled", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        private void DataAccess()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            DataTable dtAddress = new DataTable();
            DataTable dtRFID = new DataTable();
            string workOrderNumber = DA.GetLatestWorkOrderNumber(Resources.GetString(Resource.String.program_code));
            string rfid = rfidValue.Text;
            string address = customerAddress.Text;
            string city = customerCity.Text;
            string state = customerState.Text;
            string zip = customerZip.Text;
            string size = cartSize.Text;
            string type = cartType.Text;
            string notes = null;

            if (rfid != null)
            {
                if (DA.InsertWorkOrderInfo(workOrderNumber, "REMOVAL", Resources.GetString(Resource.String.program_code), rfid, address,
                city, state, zip, size, type, notes, "C", Resources.GetString(Resource.String.truck_number), DateTime.Now, DateTime.Now))
                {
                    Toast.MakeText(this, "Work Order Created! Updated work order...", ToastLength.Long).Show();
                    dtRFID = DA.GetCustomerDataByRFID(rfid);

                    if (dtRFID.Rows.Count > 0)
                    {
                        DA.UpdateWorkOrderInfo(workOrderNumber, rfid);
                        Toast.MakeText(this, DA.UpdateCustomerInfo(rfid), ToastLength.Long).Show();

                        int rowsAffected = DA.RemoveCustomer(rfidValue.Text);

                        if (rowsAffected > 0)
                        {
                            Toast.MakeText(this, "Successfully removed cart info from customer!", ToastLength.Short).Show();

                            Dialog dialog = alert.Create();
                            dialog.Show();

                            rfidValue.Text = "";
                            customerName.Text = "";
                            customerAddress.Text = "";
                            customerCity.Text = "";
                            customerState.Text = "";
                            customerZip.Text = "";
                            cartSerial.Text = "";
                            cartType.Text = "";
                            cartSize.Text = "";
                        }
                        else
                        {
                            alert.SetTitle("ERROR");
                            alert.SetMessage("There was an error removing the cart from the customer database, please try again.");

                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                                Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                            });

                            Dialog dialog = alert.Create();
                            dialog.Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Customer does not exist!", ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "An error occurred while creating the Work Order, Please try again!", ToastLength.Long).Show();
                }
            }
            else
            {
                alert = new AlertDialog.Builder(this);
                alert.SetTitle("ERROR");
                alert.SetMessage("Please make sure you enter in an RFID value before removing a cart!");

                alert.SetPositiveButton("OK", (senderAlert, args) => {
                    Toast.MakeText(this, "Enter RFID Value", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }
    }
}