using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SonraiMasterHHApp_Android
{
    [Activity(Label = "ModifyWorkOrderActivity")]
    public class ModifyWorkOrderActivity : Activity
    {
        TextView txtAddress;
        TextView txtWOType;
        TextView txtCart;
        TextView txtStatus;
        TextView txtNotes;
        TextView txtOldRFID;

        EditText editNewRFID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ModifyWorkOrder);

            txtAddress = FindViewById<TextView>(Resource.Id.txtAddress);
            txtAddress.Text = base.Intent.GetStringExtra("AddressText") ?? "Data not available";

            txtWOType = FindViewById<TextView>(Resource.Id.txtWOType);
            txtWOType.Text = base.Intent.GetStringExtra("WOTypeText") ?? "Data not available";

            txtCart = FindViewById<TextView>(Resource.Id.txtCart);
            txtCart.Text = base.Intent.GetStringExtra("CartTypeText") ?? "Data not available";

            txtStatus = FindViewById<TextView>(Resource.Id.txtStatus);
            txtStatus.Text = base.Intent.GetStringExtra("StatusText") ?? "Data not available";

            txtNotes = FindViewById<TextView>(Resource.Id.txtNotes);
            txtNotes.Text = base.Intent.GetStringExtra("NotesText") ?? "Data not available";

            txtOldRFID = FindViewById<TextView>(Resource.Id.txtOldRFID);
            txtOldRFID.Text = base.Intent.GetStringExtra("RFIDTag") ?? "Data not available";

            string workOrderID = base.Intent.GetStringExtra("ID");

            editNewRFID = FindViewById<EditText>(Resource.Id.editNewRFID);

            Button button1 = FindViewById<Button>(Resource.Id.button1);
            button1.Click += BtnBack_Click;

            Button btnConfirm = FindViewById<Button>(Resource.Id.btnConfirm);
            btnConfirm.Click += delegate (object sender, EventArgs e)
            { BtnConfirm_Click(sender, e, workOrderID, editNewRFID.Text); };

            Button btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.WorkOrder);
            Intent MainIntent = new Intent(this, typeof(WorkOrderActivity));
            StartActivity(MainIntent);
        }

        private void BtnConfirm_Click(object sender, EventArgs e, string WorkOrderID, string editNewRFID)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("CONFIRMATION");
            alert.SetMessage("Would you like to close this work order with the information provided?");

            alert.SetPositiveButton("YES", (senderAlert, args) => {
                Toast.MakeText(this, "Work order closed!", ToastLength.Short).Show();
                ModifyWorkOrder(WorkOrderID, editNewRFID);
            });

            alert.SetNegativeButton("NO", (senderAlert, args) => {
                Toast.MakeText(this, "Verify the information before closing the work order", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("MESSAGE");
            alert.SetMessage("Would you like to cancel working on this work order?");

            alert.SetPositiveButton("YES", (senderAlert, args) => {
                Toast.MakeText(this, "Back to Work order selection...", ToastLength.Short).Show();

                SetContentView(Resource.Layout.WorkOrder);
                Intent MainIntent = new Intent(this, typeof(WorkOrderActivity));
                StartActivity(MainIntent);
            });

            alert.SetNegativeButton("NO", (senderAlert, args) => {
                Toast.MakeText(this, "Continue working with this work order", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void ModifyWorkOrder(string WorkOrderID, string editNewRFID)
        {
            string WorkOrderIDint = WorkOrderID;
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
            conn.Open();
            SqlCommand command = new SqlCommand("UPDATE WorkOrderProgressive SET Status='C', AssignRFID='" + editNewRFID +"'" + " WHERE WorkOrderProgressiveID='" + WorkOrderID + "'", conn);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                alert.SetTitle("COMPLETE");
                alert.SetMessage("Work Order has been closed!");

                alert.SetPositiveButton("OK", (senderAlert, args) => {
                    Toast.MakeText(this, "Work Order Closed!", ToastLength.Short).Show();

                    SetContentView(Resource.Layout.WorkOrder);
                    Intent MainIntent = new Intent(this, typeof(WorkOrderActivity));
                    StartActivity(MainIntent);
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                alert.SetTitle("ERROR");
                alert.SetMessage("There was an error closing the work order, please try again!");

                alert.SetPositiveButton("OK", (senderAlert, args) => {
                    Toast.MakeText(this, "Error closing work order!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }
    }
}