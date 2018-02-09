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
    [Activity(Label = "RepairActivity")]
    public class RepairActivity : Activity
    {
        Button btnBack;
        Button btnExchange;
        Button btnClearInfo;
        Button btnRepair;
        Button btnGetInfo;

        EditText txtRFID;
        EditText txtCustomer;
        EditText txtAddress;
        EditText txtCity;
        EditText txtState;
        EditText txtZip;
        EditText txtCartSerial;
        EditText txtCartType;
        EditText txtCartSize;

        Spinner repairSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Repair);

            txtRFID = FindViewById<EditText>(Resource.Id.txtRFID);
            txtCustomer = FindViewById<EditText>(Resource.Id.txtCustomer);
            txtAddress = FindViewById<EditText>(Resource.Id.txtAddress);
            txtCity = FindViewById<EditText>(Resource.Id.txtCity);
            txtState = FindViewById<EditText>(Resource.Id.txtState);
            txtZip = FindViewById<EditText>(Resource.Id.txtZip);
            txtCartSerial = FindViewById<EditText>(Resource.Id.txtCartSerial);
            txtCartType = FindViewById<EditText>(Resource.Id.txtCartType);
            txtCartSize = FindViewById<EditText>(Resource.Id.txtCartSize);

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;

            btnExchange = FindViewById<Button>(Resource.Id.btnExchange);
            btnExchange.Click += BtnExchange_Click;

            btnClearInfo = FindViewById<Button>(Resource.Id.btnClearInfo);
            btnClearInfo.Click += BtnClearInfo_Click;

            btnGetInfo = FindViewById<Button>(Resource.Id.btnGetInfo);
            btnGetInfo.Click += BtnGetInfo_Click;

            btnRepair = FindViewById<Button>(Resource.Id.btnRepair);
            btnRepair.Click += BtnRepair_Click;

            repairSpinner = FindViewById<Spinner>(Resource.Id.spinnerRepair);
            repairSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.repair_codes, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            repairSpinner.Adapter = adapter;

        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            Intent MainIntent = new Intent(this, typeof(MainActivity));
            StartActivity(MainIntent);
        }

        protected void BtnExchange_Click(object sender, EventArgs e)
        {
            Intent MainIntent = new Intent(this, typeof(ExchangeActivity));
            StartActivity(MainIntent);
        }

        private void BtnClearInfo_Click(object sender, EventArgs e)
        {
            txtRFID.Text = "";
            txtCustomer.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZip.Text = "";
            txtCartSerial.Text = "";
            txtCartType.Text = "";
            txtCartSize.Text = "";
        }

        private void BtnGetInfo_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            conn.Open();

            if (txtRFID.Text != null)
            {
                SqlCommand command = new SqlCommand("Select COUNT(*) from [CustomerBeta] where RFID='" + txtRFID.Text.TrimEnd() + "'", conn);
                Int32 count = Convert.ToInt32(command.ExecuteScalar());

                if (count >= 1)
                {
                    command = new SqlCommand("Select * from [CustomerBeta] where RFID='" + txtRFID.Text.TrimEnd() + "'", conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // iterate your results here
                        while (reader.Read())
                        {
                            if (reader.GetString(15) != null)
                                txtCustomer.Text = reader.GetString(15);
                            else
                                txtCustomer.Text = "Not Available";

                            if (reader.GetString(1) != null)
                                txtAddress.Text = reader.GetString(1);
                            else
                                txtAddress.Text = "Not Available";

                            if (reader.GetString(2) != null)
                                txtCity.Text = reader.GetString(2);
                            else
                                txtCity.Text = "Not Available";

                            if (reader.GetString(3) != null)
                                txtState.Text = reader.GetString(3);
                            else
                                txtState.Text = "Not Available";

                            if (reader.GetString(4) != null)
                                txtZip.Text = reader.GetString(4);
                            else
                                txtZip.Text = "Not Available";
                        }

                    }

                    command = new SqlCommand("Select * from [CartMasterBeta] where RFID='" + txtRFID.Text.TrimEnd() + "'", conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // iterate your results here
                        while (reader.Read())
                        {
                            txtCartSerial.Text = reader.GetString(0);
                            txtCartType.Text = reader.GetString(2);
                            txtCartSize.Text = reader.GetString(3);
                        }

                    }
                    conn.Close();
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
                    conn.Close();
                }
            }
            else
            {
                //Do nothing.
            }
        }

        private void BtnRepair_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            

            if (txtRFID.Text == null)
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("RFID tag cannot be null");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else if (txtAddress.Text == null)
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("Address cannot be null, please enter a value before continuing.");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else if (txtCity.Text == null)
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("City cannot be null, please enter a value before continuing.");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else if (txtState.Text == null)
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("State cannot be null, please enter a value before continuing.");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else if (txtZip.Text == null)
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("Zip Code cannot be null, please enter a value before continuing.");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else if (repairSpinner.SelectedItem == null)
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("Repair Type must be selected, please enter a value before continuing.");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Please Try Again!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                CreateWorkOrder();
            }
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("The repair code selected is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Short).Show();
        }

        private void CreateWorkOrder()
        {
            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            string sqlStmt = "SELECT MAX(CAST(WorkOrderNumber as INT)) FROM [WorkOrderProgressive] WHERE Program='" + Resources.GetString(Resource.String.program_code) + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(sqlStmt, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            string newWorkOrderNumber = null;
            string workOrderNumber = dt.Rows[0][0].ToString();

            if (dt.Rows.Count > 0)
            {
                //string workOrderNumber = dt.Rows[0][0].ToString();

                newWorkOrderNumber = (Convert.ToInt32(workOrderNumber) + 1).ToString();
            }

            conn.Close();

            if (txtRFID.Text != null)
            {
                string connStr = "Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#";
                sqlStmt = "INSERT INTO WorkOrderProgressive (WorkOrderNumber, WorkOrderType, Program, RFIDTag, Address_1, City, State, Zip, Size, CartType, RepairCodeID, Status, AssignedTo, CreatedTimestamp, CompletedTimestamp) VALUES (@newWorkOrderNumber, @WorkOrderType, @program, @RFIDTag, @Address_1, @City, @State, @Zip, @Size, @CartType, @RepairCodeID, @Status, @AssignedTo, @CreatedTimestamp, @CompletedTimestamp);";

                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand dbCommand = new SqlCommand(sqlStmt, connection))
                    {
                        dbCommand.Parameters.Add(new SqlParameter("@newWorkOrderNumber", newWorkOrderNumber));
                        dbCommand.Parameters.Add(new SqlParameter("@WorkOrderType", "REPAIR"));
                        dbCommand.Parameters.Add(new SqlParameter("@Program", Resources.GetString(Resource.String.program_code)));
                        dbCommand.Parameters.Add(new SqlParameter("@RFIDTag", txtRFID.Text));
                        dbCommand.Parameters.Add(new SqlParameter("@Address_1", txtAddress.Text));
                        dbCommand.Parameters.Add(new SqlParameter("@City", txtCity.Text));
                        dbCommand.Parameters.Add(new SqlParameter("@State", txtState.Text));
                        dbCommand.Parameters.Add(new SqlParameter("@Zip", txtZip.Text));
                        dbCommand.Parameters.Add(new SqlParameter("@Size", txtCartSize.Text));
                        dbCommand.Parameters.Add(new SqlParameter("@CartType", txtCartType.Text));
                        dbCommand.Parameters.Add(new SqlParameter("@RepairCodeID", repairSpinner.SelectedItemId));
                        dbCommand.Parameters.Add(new SqlParameter("@Status", "C"));
                        dbCommand.Parameters.Add(new SqlParameter("@AssignedTo", Resources.GetString(Resource.String.truck_number)));
                        dbCommand.Parameters.Add(new SqlParameter("@CreatedTimestamp", DateTime.Now));
                        dbCommand.Parameters.Add(new SqlParameter("@CompletedTimestamp", DateTime.Now));

                        try
                        {
                            connection.Open();
                            dbCommand.ExecuteNonQuery();
                            connection.Close();
                            Toast.MakeText(this, "Updating customer info for work order...", ToastLength.Short).Show();
                        }
                        catch (SqlException e)
                        {
                            Toast.MakeText(this, "ERROR - " + e.Message, ToastLength.Short).Show();
                        }
                    }
                }

                conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
                alert = new AlertDialog.Builder(this);

                conn.Open();
                string command = "SELECT Size, CustNumber, CartSerial, Lat, Long, Name, Type FROM [CustomerBeta] WHERE RFID='" + txtRFID.Text + "'";
                sqlData = new SqlDataAdapter(command, conn);
                ds = new DataSet();
                dt = new DataTable();

                sqlData.Fill(ds);
                dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    sqlStmt = "UPDATE WorkOrderProgressive SET CartSerial = @cartSerial, Latitude = @lat, Longitude = @long, CustomerNumber = @custNumber, CustomerName = @name, Size = @size, CartType = @type WHERE WorkOrderNumber='" + newWorkOrderNumber + "'";

                    using (SqlConnection connect = new SqlConnection(connStr))
                    {
                        using (SqlCommand commandSql = new SqlCommand(sqlStmt, connect))
                        {
                            commandSql.Parameters.Add(new SqlParameter("@cartSerial", dt.Rows[0][2]));
                            commandSql.Parameters.Add(new SqlParameter("@lat", dt.Rows[0][3]));
                            commandSql.Parameters.Add(new SqlParameter("@long", dt.Rows[0][4]));
                            commandSql.Parameters.Add(new SqlParameter("@custNumber", dt.Rows[0][1]));
                            commandSql.Parameters.Add(new SqlParameter("@name", dt.Rows[0][5]));
                            commandSql.Parameters.Add(new SqlParameter("@size", dt.Rows[0][0]));
                            commandSql.Parameters.Add(new SqlParameter("@type", dt.Rows[0][6]));

                            try
                            {
                                connect.Open();
                                commandSql.ExecuteNonQuery();

                                Toast.MakeText(this, "Work Order Updated!", ToastLength.Long).Show();
                                connect.Close();
                            }
                            catch (Exception e)
                            {
                                Toast.MakeText(this, "Error delivering - " + e.Message, ToastLength.Long).Show();
                            }
                        }
                    }
                }
            }
        }
    }
}