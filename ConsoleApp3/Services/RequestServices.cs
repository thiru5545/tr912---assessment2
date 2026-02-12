using System;
using System.Collections.Generic;
using System.Text;

using System;
using Microsoft.Data.SqlClient;

internal class RequestServices
{
    //public void addrequest(int uid)
    //{
    //    using SqlConnection c = new SqlConnection(Db.conn);
    //    SqlCommand cmd = new SqlCommand(
    //        "INSERT INTO Requests(UserId,Message,RequestType) VALUES(@u,'Upgrade',1)", c);
    //    cmd.Parameters.AddWithValue("@u", uid);
    //    c.Open();
    //    cmd.ExecuteNonQuery();
    //}

    public void addrequest(int userId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);

        string query = @"INSERT INTO Requests (UserId, Message, RequestType) 
                     VALUES (@uid, 'Upgrade to Premium', 0)";

        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@uid", userId);

        conn.Open();
        cmd.ExecuteNonQuery();

        Console.WriteLine("Subscription request sent successfully (Pending).");
    }


    public void viewrequest()
    {
        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand("SELECT * FROM Requests", c);
        c.Open();
        SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
            Console.WriteLine($"{r["RequestId"]} {r["UserId"]} {r["RequestType"]}");
    }

    //public void viewrequest(int uid)
    //{
    //    using SqlConnection c = new SqlConnection(Db.conn);
    //    SqlCommand cmd = new SqlCommand(
    //        "SELECT * FROM Requests WHERE UserId=@u", c);
    //    cmd.Parameters.AddWithValue("@u", uid);
    //    c.Open();
    //    SqlDataReader r = cmd.ExecuteReader();
    //    while (r.Read())
    //        Console.WriteLine($"{r["RequestId"]} {r["RequestType"]}");
    //}

    public void viewrequest(int uid)
    {
        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand(
            "SELECT RequestId, RequestType FROM Requests WHERE UserId=@u", c);

        cmd.Parameters.AddWithValue("@u", uid);

        c.Open();
        SqlDataReader r = cmd.ExecuteReader();

        while (r.Read())
        {
            string statusText = "";

            int status = Convert.ToInt32(r["RequestType"]);

            if (status == 0)
                statusText = "Pending";
            else if (status == 1)
                statusText = "Success";
            else
                statusText = "Unknown";

            Console.WriteLine($"REQUEST ID : {r["RequestId"]} \t REUEST STATUS : {statusText}");
        }
    }


    public void ViewPendingRequests()
    {
        using SqlConnection conn = new SqlConnection(Db.conn);

        string query = "SELECT * FROM Requests WHERE RequestType = 0";

        SqlCommand cmd = new SqlCommand(query, conn);

        conn.Open();
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"RequestID: {reader["RequestId"]} | " +
                $"UserID: {reader["UserId"]} | " +
                $"Status: Pending");
        }
    }


    public void ApproveRequest(int requestId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        conn.Open();

        SqlTransaction transaction = conn.BeginTransaction();

        try
        {
            // Step 1: Get UserId from Request
            SqlCommand getUserCmd = new SqlCommand(
                "SELECT UserId FROM Requests WHERE RequestId=@rid",
                conn, transaction);

            getUserCmd.Parameters.AddWithValue("@rid", requestId);

            object result = getUserCmd.ExecuteScalar();

            if (result == null)
            {
                Console.WriteLine("Invalid Request ID.");
                transaction.Rollback();
                return;
            }

            int userId = (int)result;

            // Step 2: Update Request Status → Approved
            SqlCommand updateRequest = new SqlCommand(
                "UPDATE Requests SET RequestType=1 WHERE RequestId=@rid",
                conn, transaction);

            updateRequest.Parameters.AddWithValue("@rid", requestId);
            updateRequest.ExecuteNonQuery();

            // Step 3: Update User Subscription → Premium
            SqlCommand updateUser = new SqlCommand(
                "UPDATE Users SET Subscription=1 WHERE Id=@uid",
                conn, transaction);

            updateUser.Parameters.AddWithValue("@uid", userId);
            updateUser.ExecuteNonQuery();

            transaction.Commit();

            Console.WriteLine("Request Approved! User upgraded to Premium.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine("Error: " + ex.Message);
        }
    }



    public void RejectRequest(int requestId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);

        SqlCommand cmd = new SqlCommand(
            "UPDATE Requests SET RequestType=2 WHERE RequestId=@rid", conn);

        cmd.Parameters.AddWithValue("@rid", requestId);

        conn.Open();
        cmd.ExecuteNonQuery();

        Console.WriteLine("Request rejected.");
    }



}

