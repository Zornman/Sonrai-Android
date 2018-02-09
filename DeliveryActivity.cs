using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Data;

namespace SonraiMasterHHApp_Android
{
    [Activity(Label = "DeliveryActivity")]
    public class DeliveryActivity : Activity, ILocationListener
    {
        static readonly string TAG = "X:" + typeof(DeliveryActivity).Name;
        Location _currentLocation;
        LocationManager _locationManager;

        string _locationProvider;

        TextView txtView1;
        TextView txtView2;
        TextView txtView3;
        TextView txtView4;
        TextView txtView5;
        TextView txtView6;
        TextView txtView8;
        TextView txtView9;
        TextView _locationText;

        EditText editTxt1;
        EditText editTxt2;
        EditText editTxt3;
        EditText editTxt4;
        EditText editTxt5;
        EditText editTxt7;

        Spinner sizeSpinner;
        Spinner typeSpinner;

        Button btnBack;
        Button btnGenerateAddress;
        Button btnSubmit;

        FrameLayout frameLayout1;
        FrameLayout frameLayout2;
        FrameLayout frameLayout3;
        FrameLayout frameLayout4;
        FrameLayout frameLayout5;
        FrameLayout frameLayout6;
        FrameLayout frameLayout7;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Delivery);

            txtView1 = FindViewById<TextView>(Resource.Id.textView1);
            txtView2 = FindViewById<TextView>(Resource.Id.textView2);
            txtView3 = FindViewById<TextView>(Resource.Id.textView3);
            txtView4 = FindViewById<TextView>(Resource.Id.textView4);
            txtView5 = FindViewById<TextView>(Resource.Id.textView5);
            txtView6 = FindViewById<TextView>(Resource.Id.textView6);
            txtView8 = FindViewById<TextView>(Resource.Id.textView8);
            txtView9 = FindViewById<TextView>(Resource.Id.textView9);
            _locationText = FindViewById<TextView>(Resource.Id._locationText);

            editTxt1 = FindViewById<EditText>(Resource.Id.editText1);
            editTxt2 = FindViewById<EditText>(Resource.Id.editText2);
            editTxt3 = FindViewById<EditText>(Resource.Id.editText3);
            editTxt4 = FindViewById<EditText>(Resource.Id.editText4);
            editTxt5 = FindViewById<EditText>(Resource.Id.editText5);
            editTxt7 = FindViewById<EditText>(Resource.Id.editText7);

            frameLayout1 = FindViewById<FrameLayout>(Resource.Id.frameLayout1);
            frameLayout2 = FindViewById<FrameLayout>(Resource.Id.frameLayout2);
            frameLayout3 = FindViewById<FrameLayout>(Resource.Id.frameLayout3);
            frameLayout4 = FindViewById<FrameLayout>(Resource.Id.frameLayout4);
            frameLayout5 = FindViewById<FrameLayout>(Resource.Id.frameLayout5);
            frameLayout6 = FindViewById<FrameLayout>(Resource.Id.frameLayout6);
            frameLayout7 = FindViewById<FrameLayout>(Resource.Id.frameLayout7);

            sizeSpinner = FindViewById<Spinner>(Resource.Id.sizeSpinner);
            var sizeAdapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.cart_sizes, Android.Resource.Layout.SimpleSpinnerItem);
            sizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            sizeSpinner.Adapter = sizeAdapter;

            typeSpinner = FindViewById<Spinner>(Resource.Id.typeSpinner);
            var typeAdapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.cart_type, Android.Resource.Layout.SimpleSpinnerItem);
            typeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            typeSpinner.Adapter = typeAdapter;

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            FindViewById<Button>(Resource.Id.btnBack).Click += BtnBack_Click;
            btnGenerateAddress = FindViewById<Button>(Resource.Id.btnGenerateAddress);
            FindViewById<Button>(Resource.Id.btnGenerateAddress).Click += BtnGenerateAddress_OnClick;
            btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);
            btnSubmit.Click += BtnSubmit_Click;

            InitializeLocationManager();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent MainIntent = new Intent(this, typeof(MainActivity));
            StartActivity(MainIntent);
        }

        public async void OnLocationChanged(Location location)
        {

            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                try
                {
                    Address address = await ReverseGeocodeCurrentLocation();
                    DisplayAddress(address);
                }
                catch (Exception)
                {

                }


            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug(TAG, "{0}, {1}", provider, status);
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            }
            catch (Exception)
            {

            }

            Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
        }

        //protected override void OnPause()
        //{
        //    base.OnPause();
        //    _locationManager.RemoveUpdates(this);
        //    Log.Debug(TAG, "No longer listening for location updates.");
        //}

        async void BtnGenerateAddress_OnClick(object sender, EventArgs eventArgs)
        {
            if (_currentLocation == null)
            {
                editTxt2.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }
            try
            {
                Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);
            }
            catch (Exception)
            {
            }

        }

        protected void BtnSubmit_Click(object sender, EventArgs eventArgs)
        {
            CreateWorkOrder();
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                char[] delimiter = { ',', '"' };
                string[] addressSplit = address.ToString().Split(delimiter);

                foreach (var addresses in addressSplit)
                {
                    editTxt2.Text = addressSplit[1]; // Returns the Address
                    editTxt3.Text = addressSplit[2]; // Returns the City
                    editTxt5.Text = addressSplit[3]; // Returns the State
                    // editTxt4.Text = addressSplit[4]; // Returns the Zip
                }

                char sDelimiter = ' ';
                string state = editTxt5.Text;
                string[] stateZip = state.ToString().Split(sDelimiter);

                foreach (var zState in stateZip)
                {
                    editTxt5.Text = stateZip[1];
                    editTxt4.Text = stateZip[2];
                }
            }
            else
            {
                editTxt2.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }

        private void ClearInputs()
        {
            editTxt1.Text = "";
            editTxt2.Text = "";
            editTxt3.Text = "";
            editTxt4.Text = "";
            editTxt5.Text = "";
            editTxt7.Text = "";
        }

        private void CreateWorkOrder()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            dataLayer DA = new dataLayer();

            string address = editTxt2.Text;
            DataTable dataTable = DA.GetCustomerDataByAddress(address);

            if (dataTable.Rows.Count > 0)
            {
                alert.SetTitle("ALERT");
                alert.SetMessage("This customer already has a bin assigned to them\n\nWould you like to deliver this cart to them anyway?");

                alert.SetPositiveButton("YES", (senderAlert, args) => {
                    Toast.MakeText(this, "Delivering to existing customer...", ToastLength.Short).Show();

                    DataAccess();
                });

                alert.SetNegativeButton("NO", (senderAlert, args) => {
                    Toast.MakeText(this, "Delivery cancelled", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        private void DataAccess()
        {
            dataLayer DA = new dataLayer();
            DataTable dtAddress = new DataTable();
            DataTable dtRFID = new DataTable();
            string workOrderNumber = DA.GetLatestWorkOrderNumber(Resources.GetString(Resource.String.program_code));
            string rfid = editTxt1.Text;
            string address = editTxt2.Text;
            string city = editTxt3.Text;
            string state = editTxt5.Text;
            string zip = editTxt4.Text;
            string size = sizeSpinner.SelectedItem.ToString();
            string type = typeSpinner.SelectedItem.ToString();
            string notes = editTxt7.Text;

            if (rfid != null)
            {
                if (DA.InsertWorkOrderInfo(workOrderNumber, "DELIVERY", Resources.GetString(Resource.String.program_code), rfid, address,
                city, state, zip, size, type, notes, "C", Resources.GetString(Resource.String.truck_number), DateTime.Now, DateTime.Now))
                {
                    dtRFID = DA.GetCustomerDataByRFID(rfid);

                    if (dtRFID.Rows.Count > 1)
                    {
                        DA.UpdateWorkOrderInfo(workOrderNumber, rfid);
                        Toast.MakeText(this, DA.UpdateCustomerInfo(rfid), ToastLength.Long).Show();
                    }
                    else
                    {
                        if (type == "Trash")
                        {
                            DA.InsertNewCustomer("Trash", address, city, state, zip, size, rfid, Resources.GetString(Resource.String.program_code), _currentLocation.Latitude, _currentLocation.Longitude, workOrderNumber);
                            Toast.MakeText(this, "Delivered!", ToastLength.Long).Show();
                            DA.UpdateWorkOrderInfo(workOrderNumber, rfid);
                            Toast.MakeText(this, "Delivered and Updated Work Order!", ToastLength.Long).Show();
                        }
                        else if (type == "Recycling")
                        {
                            DA.InsertNewCustomer("Recycling", address, city, state, zip, size, rfid, Resources.GetString(Resource.String.program_code), _currentLocation.Latitude, _currentLocation.Longitude, workOrderNumber);
                            Toast.MakeText(this, "Delivered!", ToastLength.Long).Show();
                            DA.UpdateWorkOrderInfo(workOrderNumber, rfid);
                            Toast.MakeText(this, "Delivered and Updated Work Order!", ToastLength.Long).Show();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "An error occurred while creating the Work Order, Please try again!", ToastLength.Long).Show();
                }
            }
            else
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("ERROR");
                alert.SetMessage("Please make sure you enter in an RFID value before delivering a cart!");

                alert.SetPositiveButton("OK", (senderAlert, args) => {
                    Toast.MakeText(this, "Enter RFID Value", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }
    }
}