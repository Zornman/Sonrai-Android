using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SonraiMasterHHApp_Android
{
    class dataLayer
    {
        SqlConnection conn = new SqlConnection("Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#");
        string connStr = "Data Source=vcnsql88.webhost4life.com,1433;Initial Catalog=sonrai001;Persist Security Info=True;User ID=alinaqvi;Password=Sonrai33#";

        #region Delivery Activity
        public DataTable GetCustomerDataByAddress(string address)
        {
            string sqlStmt = "SELECT * FROM CustomerBeta WHERE Address='" + address + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(sqlStmt, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            return dt;
        }

        public DataTable GetCustomerDataByRFID(string rfid)
        {
            string sqlStmt = "SELECT * FROM CustomerBeta WHERE RFID='" + rfid + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(sqlStmt, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            return dt;
        }

        public string GetLatestWorkOrderNumber(string programCode)
        {
            string command = "SELECT MAX(CAST(WorkOrderNumber as INT)) FROM [WorkOrderProgressive] WHERE Program='" + programCode + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(command, conn);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            string newWorkOrderNumber = null;
            int workOrderNumber = Convert.ToInt32(dt.Rows[0][0]);

            if (dt.Rows.Count > 0)
            {
                newWorkOrderNumber = (workOrderNumber + 1).ToString();
            }
            else
            {
                return "Unable to get latest work order number, try again";
            }

            return newWorkOrderNumber;
        }

        public bool InsertWorkOrderInfo(string newWorkOrderNumber, string workOrderType, string Program, string RFID, string Address, string City, 
            string State, string Zip, string Size, string CartType, string Notes, string Status, 
            string AssignedTo, DateTime CreatedTimestamp, DateTime CompletedTimestamp)
        {
            bool updated = false;

            string sqlStmt = "INSERT INTO WorkOrderProgressive (WorkOrderNumber, WorkOrderType, Program, RFIDTag, Address_1, City, State, Zip, Size, CartType, Notes, Status, AssignedTo, CreatedTimestamp, CompletedTimestamp) " +
                "VALUES (@newWorkOrderNumber, @WorkOrderType, @program, @RFIDTag, @Address_1, @City, @State, @Zip, @Size, @CartType, @Notes, @Status, @AssignedTo, @CreatedTimestamp, @CompletedTimestamp);";

            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand dbCommand = new SqlCommand(sqlStmt, connection))
                    {
                        dbCommand.Parameters.Add(new SqlParameter("@newWorkOrderNumber", newWorkOrderNumber));
                        dbCommand.Parameters.Add(new SqlParameter("@WorkOrderType", workOrderType));
                        dbCommand.Parameters.Add(new SqlParameter("@Program", Program));
                        dbCommand.Parameters.Add(new SqlParameter("@RFIDTag", RFID));
                        dbCommand.Parameters.Add(new SqlParameter("@Address_1", Address));
                        dbCommand.Parameters.Add(new SqlParameter("@City", City));
                        dbCommand.Parameters.Add(new SqlParameter("@State", State));
                        dbCommand.Parameters.Add(new SqlParameter("@Zip", Zip));
                        dbCommand.Parameters.Add(new SqlParameter("@Size", Size));
                        dbCommand.Parameters.Add(new SqlParameter("@CartType", CartType));
                        dbCommand.Parameters.Add(new SqlParameter("@Notes", Notes));
                        dbCommand.Parameters.Add(new SqlParameter("@Status", Status));
                        dbCommand.Parameters.Add(new SqlParameter("@AssignedTo", AssignedTo));
                        dbCommand.Parameters.Add(new SqlParameter("@CreatedTimestamp", CreatedTimestamp));
                        dbCommand.Parameters.Add(new SqlParameter("@CompletedTimestamp", CompletedTimestamp));

                        connection.Open();
                        dbCommand.ExecuteNonQuery();
                        connection.Close();

                        updated = true;
                    }
                }
            }
            catch (Exception)
            {
                updated = false;
            }
            

            return updated;
        }

        public void UpdateWorkOrderInfo(string workOrderNumber, string rfid)
        {
            string sqlStmt = "UPDATE WorkOrderProgressive SET CartSerial = @cartSerial, Latitude = @lat, Longitude = @long, CustomerNumber = @custNumber, " +
                "Name = @name, Size = @size, Type = @type WHERE WorkOrderNumber='" + workOrderNumber + "'";

            DataTable dt = new DataTable();

            dt = GetCustomerDataByRFID(rfid);

            using (SqlConnection connect = new SqlConnection(connStr))
            {
                using (SqlCommand commandSql = new SqlCommand(sqlStmt, connect))
                {
                    commandSql.Parameters.Add(new SqlParameter("@cartSerial", dt.Rows[0][6]));
                    commandSql.Parameters.Add(new SqlParameter("@lat", dt.Rows[0][12]));
                    commandSql.Parameters.Add(new SqlParameter("@long", dt.Rows[0][13]));
                    commandSql.Parameters.Add(new SqlParameter("@custNumber", dt.Rows[0][9]));
                    commandSql.Parameters.Add(new SqlParameter("@name", dt.Rows[0][15]));
                    commandSql.Parameters.Add(new SqlParameter("@size", dt.Rows[0][5]));
                    commandSql.Parameters.Add(new SqlParameter("@type", dt.Rows[0][26]));

                    try
                    {
                        connect.Open();
                        commandSql.ExecuteNonQuery();
                        connect.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void InsertNewCustomer(string cartType, string Address, string City, string State, string Zip, string Size, string rfid, string ProgramName, double Lat,
            double Long, string workOrderNumber)
        {
            string sqlStmt = null;

            if (cartType == "Trash")
            {
                sqlStmt = "INSERT INTO [CustomerBeta] (Address, City, State, Zip, Size, RFID, Timestamp, Lat, Long, ProgramName, BinType, CreatedTS, LastUpdatedTS, Type, Manufacturer, Color, WorkOrderID)" +
                "VALUES (@Address, @City, @State, @Zip, @Size, @RFID, @Timestamp, @Lat, @Long, @ProgramName, 'TR', @CreatedTS, @LastUpdatedTS, 'TRASH', 'REHRIG', 'GREY', @WorkOrderID)";
            }
            else if (cartType == "Recycling")
            {
                sqlStmt = "INSERT INTO [CustomerBeta] (Address, City, State, Zip, Size, RFID, Timestamp, Lat, Long, ProgramName, BinType, CreatedTS, LastUpdatedTS, Type, Manufacturer, Color, WorkOrderID)" +
                "VALUES (@Address, @City, @State, @Zip, @Size, @RFID, @Timestamp, @Lat, @Long, @ProgramName, 'RC', @CreatedTS, @LastUpdatedTS, 'RECYCLING', 'SIERRA', 'BLACK', @WorkOrderID)";
            }

            using (SqlConnection connect = new SqlConnection(connStr))
            {
                using (SqlCommand commandSql = new SqlCommand(sqlStmt, connect))
                {
                    commandSql.Parameters.Add(new SqlParameter("@Address", Address));
                    commandSql.Parameters.Add(new SqlParameter("@City", City));
                    commandSql.Parameters.Add(new SqlParameter("@State", State));
                    commandSql.Parameters.Add(new SqlParameter("@Zip", Zip));
                    commandSql.Parameters.Add(new SqlParameter("@Size", Size));
                    commandSql.Parameters.Add(new SqlParameter("@RFID", rfid));
                    commandSql.Parameters.Add(new SqlParameter("@Timestamp", DateTime.Now));
                    commandSql.Parameters.Add(new SqlParameter("@ProgramName", ProgramName));
                    commandSql.Parameters.Add(new SqlParameter("@Lat", Lat));
                    commandSql.Parameters.Add(new SqlParameter("@Long", Long));
                    commandSql.Parameters.Add(new SqlParameter("@CreatedTS", DateTime.Now));
                    commandSql.Parameters.Add(new SqlParameter("@LastUpdatedTS", DateTime.Now));
                    commandSql.Parameters.Add(new SqlParameter("@WorkOrderID", workOrderNumber));

                    try
                    {
                        connect.Open();
                        commandSql.ExecuteNonQuery();
                        connect.Close();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public string UpdateCustomerInfo(string rfid)
        {
            string message = null;
            string sqlStmt = "UPDATE CustomerBeta SET CustNumber = @CustNumber, Name = @Name, BinCategory = @Bin, County = @County, Zone = @Zone";
            DataTable dt = new DataTable();

            dt = GetCustomerDataByRFID(rfid);

            using (SqlConnection connect = new SqlConnection(connStr))
            {
                using (SqlCommand commandSql = new SqlCommand(sqlStmt, connect))
                {
                    commandSql.Parameters.Add(new SqlParameter("@CustNumber", dt.Rows[0][9].ToString()));
                    commandSql.Parameters.Add(new SqlParameter("@Name", dt.Rows[0][15].ToString()));
                    commandSql.Parameters.Add(new SqlParameter("@Bin", dt.Rows[0][32].ToString()));
                    commandSql.Parameters.Add(new SqlParameter("@County", dt.Rows[0][33].ToString()));
                    commandSql.Parameters.Add(new SqlParameter("@Zone", dt.Rows[0][39].ToString()));

                    try
                    {
                        connect.Open();
                        commandSql.ExecuteNonQuery();
                        connect.Close();
                        message = "Successfully updated work order with customer info!";
                    }
                    catch (Exception error)
                    {
                        message = "Error - " + error.Message;
                    }
                }
            }

            return message;
        }
        #endregion

        #region Exchange Activity
        public DataTable OldRFIDCartInformation(string oldRFID)
        {
            string command = "Select Size, Type from [CustomerBeta] where RFID='" + oldRFID + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(command, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            conn.Open();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            conn.Close();

            return dt;
        }

        public DataTable NewRFIDCartInformation(string newRFID)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            conn.Open();

            SqlCommand command = new SqlCommand("Select COUNT(*) from [CustomerBeta] where RFID='" + newRFID + "'", conn);
            Int32 count = Convert.ToInt32(command.ExecuteScalar());

            if (count >= 1)
            {
                string comm = "Select Size, Type from [CustomerBeta] where RFID='" + newRFID + "'";
                SqlDataAdapter sqlData = new SqlDataAdapter(comm, conn);

                sqlData.Fill(ds);
                dt = ds.Tables[0];
            }
            else
            {
                //Do nothing
            }

            conn.Close();
            return dt;
        }

        public bool ExchangeCarts(string oldRFID, string newRFID)
        {
            bool exchanged = false;

            conn.Open();

            SqlCommand command = new SqlCommand("update [CustomerBeta] set RFID='" + newRFID + "' where RFID='" + oldRFID + "'", conn);
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                exchanged = true;
            }
            else
            {
                exchanged = false;
            }

            return exchanged;
        }
        #endregion

        #region Removal Activity
        public DataTable GetCustomerInfo(string rfid)
        {
            string command = "Select Address, City, State, Zip, Name from [CustomerBeta] where RFID='" + rfid + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(command, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            conn.Open();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            conn.Close();

            return dt;
        }

        public DataTable GetCartInfo(string rfid)
        {
            string command = "Select * from [CartMasterBeta] where RFID='" + rfid + "'";
            SqlDataAdapter sqlData = new SqlDataAdapter(command, conn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            conn.Open();

            sqlData.Fill(ds);
            dt = ds.Tables[0];

            conn.Close();

            return dt;
        }

        public int RemoveCustomer(string rfid)
        {
            int rowsAffected = 0;

            conn.Open();
            SqlCommand command = new SqlCommand("Select COUNT(*) from [CustomerBeta] where RFID='" + rfid + "'", conn);
            int count = Convert.ToInt32(command.ExecuteScalar());

            if (count > 0)
            {
                command = new SqlCommand("UPDATE [CustomerBeta] SET Size='', CartSerial='', RFID='', BinType='', Material='', Type='', MaterialType='', Manufacturer='', Color='' WHERE RFID='" + rfid + "'", conn);
                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected;
        }
        #endregion
    }
}