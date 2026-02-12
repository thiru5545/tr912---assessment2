using System;
using System.Collections.Generic;
using System.Text;

using System;
using Microsoft.Data.SqlClient;

internal class VideoServices
{
    public void videolist()
    {
        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand("SELECT * FROM Videos", c);
        c.Open();
        SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
            Console.WriteLine($"{r["VideoId"]} {r["VideoName"]}");
    }

    public void videolist(int userId)
    {
        using SqlConnection c = new SqlConnection(Db.conn);
        c.Open();

        // Step 1: Get User Subscription
        SqlCommand getSub = new SqlCommand(
            "SELECT Subscription FROM Users WHERE Id=@uid", c);

        getSub.Parameters.AddWithValue("@uid", userId);

        object result = getSub.ExecuteScalar();

        if (result == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        int userSub = (int)result;

        // Step 2: Fetch Videos Based on Subscription
        string query;

        if (userSub == 0)
        {
            // Free user → only free videos
            query = "SELECT * FROM Videos WHERE Subscription = 0";
        }
        else
        {
            // Premium user → all videos
            query = "SELECT * FROM Videos";
        }

        SqlCommand cmd = new SqlCommand(query, c);
        SqlDataReader r = cmd.ExecuteReader();

        Console.WriteLine("\n--- AVAILABLE VIDEOS ---");

        while (r.Read())
        {
            Console.WriteLine(
                $"ID: {r["VideoId"]} | " +
                $"Title: {r["VideoName"]} | " +
                $"Type: {((int)r["Subscription"] == 0 ? "Free" : "Premium")}");
        }
    }

    public void AddVideo(int id)
    {
        Console.Write("Title: ");
        string t = Console.ReadLine();
        Console.Write("URL: ");
        string u = Console.ReadLine();
        Console.WriteLine("Enter the subscriptions type [ 1-premium | 0-basic ]");
        int sub=int.Parse(Console.ReadLine());
        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand(
            "INSERT INTO Videos VALUES (@t,@u,@sub)", c);
        //cmd.Parameters.AddWithValue("@i", id);
        cmd.Parameters.AddWithValue("@t", t);
        cmd.Parameters.AddWithValue("@u", u);
        cmd.Parameters.AddWithValue("@sub", sub);
        c.Open();
        cmd.ExecuteNonQuery();
    }

    public void removevideo(int id)
    {
        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand(
            "DELETE FROM Videos WHERE VideoId=@id", c);
        cmd.Parameters.AddWithValue("@id", id);
        c.Open();
        cmd.ExecuteNonQuery();
    }
}

