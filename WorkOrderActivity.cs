using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics;
using static Android.Views.View;

namespace SonraiMasterHHApp_Android
{
    [Activity(Label = "WorkOrderActivity")]
    public class WorkOrderActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.WorkOrder);

            //SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            //conn.Open();
            //SqlCommand command = new SqlCommand("Select COUNT(*) from [WorkOrderProgressive] where Program='" + Resources.GetString(Resource.String.program_code) + "' AND AssignedTo='" + Resources.GetString(Resource.String.truck_number) + "'", conn);
            //int numOfRows = Convert.ToInt32(command.ExecuteScalar());
            //conn.Close();
            //Might be completely useless if using the DataTable in LoadAllWorkOrders()

            Button button1 = FindViewById<Button>(Resource.Id.button1);
            button1.Click += BtnBack_Click;

            StartLoadData();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent MainIntent = new Intent(this, typeof(MainActivity));
            StartActivity(MainIntent);
        }

        private void StartLoadData()
        {
            //progressBar.IncrementProgressBy(1);
            LoadAllWorkOrders();
        }

        public void LoadAllWorkOrders()
        {
            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            conn.Open();
            string command = "Select WorkOrderProgressiveID, Address_1, City, State, Zip, WorkOrderType, CartType, Status, Notes from [WorkOrderProgressive] where Program='" + Resources.GetString(Resource.String.program_code) + "' AND AssignedTo='" + Resources.GetString(Resource.String.truck_number) + "' AND status='O'";
            SqlDataAdapter sqlData = new SqlDataAdapter(command, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            ShapeDrawable TextView_Border = new ShapeDrawable();
            TextView_Border.SetBounds(10, 10, 10, 10);

            ScrollView scrollView1 = FindViewById<ScrollView>(Resource.Id.scrollView1);
            TableLayout workOrderTable = FindViewById<TableLayout>(Resource.Id.tableLayout1);
            workOrderTable.SetColumnStretchable(0, true);

            TableRow tableHeader = new TableRow(this);
            TextView header1 = new TextView(this);
            tableHeader.AddView(header1);
            header1.Text = "Address";
            header1.Background = TextView_Border;

            TextView header2 = new TextView(this);
            tableHeader.AddView(header2);
            header1.Text = "WO Type";
            header1.Background = TextView_Border;

            TextView header3 = new TextView(this);
            tableHeader.AddView(header3);
            header1.Text = "Cart Type";
            header1.Background = TextView_Border;

            TextView header4 = new TextView(this);
            tableHeader.AddView(header4);
            header1.Text = "Status";
            header1.Background = TextView_Border;

            TextView header5 = new TextView(this);
            tableHeader.AddView(header5);
            header1.Text = "Address";
            header1.Background = TextView_Border;

            workOrderTable.AddView(tableHeader);

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                TableRow tableRow = new TableRow(this);
                //tableRow.GenerateLayoutParams();

                Button SelectWO = new Button(this);
                tableRow.AddView(SelectWO);
                SelectWO.Text = "SELECT";
                //SelectWO.SetOnClickListener(this);

                //SelectWO.PerformClick(SelectButtonClicked(true));

                TextView Address = new TextView(this);
                tableRow.AddView(Address);
                Address.Background = TextView_Border;
                Address.Text = dt.Rows[i][1].ToString() + ", " + dt.Rows[i][2].ToString() + ", " + dt.Rows[i][3].ToString() + " " + dt.Rows[i][4].ToString();

                TextView WorkOrderType = new TextView(this);
                tableRow.AddView(WorkOrderType);
                WorkOrderType.Background = TextView_Border;
                WorkOrderType.Text = dt.Rows[i][5].ToString();

                TextView CartType = new TextView(this);
                tableRow.AddView(CartType);
                CartType.Background = TextView_Border;
                CartType.Text = dt.Rows[i][6].ToString();

                TextView Status = new TextView(this);
                tableRow.AddView(Status);
                Status.Background = TextView_Border;
                Status.Text = dt.Rows[i][7].ToString();

                TextView Notes = new TextView(this);
                tableRow.AddView(Notes);
                Notes.Background = TextView_Border;
                Notes.Text = dt.Rows[i][8].ToString();

                TextView WorkOrderID = new TextView(this);
                tableRow.AddView(WorkOrderID);
                WorkOrderID.Background = TextView_Border;
                WorkOrderID.Text = dt.Rows[i][0].ToString();
                SelectWO.SetTag(Resource.Id.Button_ID, WorkOrderID);

                SelectWO.Click += delegate (object sender, EventArgs e)
                { SelectedWorkOrder(sender, e, WorkOrderID.Text); };

                workOrderTable.AddView(tableRow);
            }
        }

        private void SelectWO_Click(object sender, EventArgs e)
        {
            
        }

        private void SelectedWorkOrder(Object senter, EventArgs e, string WorkOrderID)
        {

            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("CONFIRMATION");
            alert.SetMessage("Would you like to open and modify this work order?");

            alert.SetPositiveButton("YES", (senderAlert, args) => {
                Toast.MakeText(this, "Opening Work Order...", ToastLength.Short).Show();
                ModifyWorkOrder(WorkOrderID);
            });

            alert.SetNegativeButton("NO", (senderAlert, args) => {
                Toast.MakeText(this, "Please select a work order to modify", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void ModifyWorkOrder(string WorkOrderID)
        {
            string WorkOrderIDint = WorkOrderID;

            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            conn.Open();
            string command = "Select WorkOrderProgressiveID, RFIDTag, Address_1, City, State, Zip, WorkOrderType, CartType, Status, Notes from [WorkOrderProgressive] where WorkOrderProgressiveID= '" + WorkOrderIDint + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(command, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                Intent MainIntent = new Intent(this, typeof(ModifyWorkOrderActivity));
                MainIntent.PutExtra("RFIDTag", dt.Rows[0][1].ToString());
                MainIntent.PutExtra("AddressText", dt.Rows[0][2].ToString() + ", " + dt.Rows[0][3].ToString() + ", " + dt.Rows[0][4].ToString() + " " + dt.Rows[0][5].ToString());
                MainIntent.PutExtra("WOTypeText", dt.Rows[0][6].ToString());
                MainIntent.PutExtra("CartTypeText", dt.Rows[0][7].ToString());
                MainIntent.PutExtra("StatusText", dt.Rows[0][8].ToString());
                MainIntent.PutExtra("NotesText", dt.Rows[0][9].ToString());
                MainIntent.PutExtra("ID", dt.Rows[0][0].ToString());
                StartActivity(MainIntent);
            }
            else
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);

                alert.SetTitle("ERROR");
                alert.SetMessage("There is no data for the selected work order!");

                alert.SetPositiveButton("OK", (senderAlert, args) => {
                    Toast.MakeText(this, "Please Try Again", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }
    }
}