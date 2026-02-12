using System;
using System.Collections.Generic;
using System.Text;

using System;
using Microsoft.Data.SqlClient;

internal class UserServices
{
    //public void CreateUser()
    //{
    //    //    Console.Write("ID: ");
    //    //    int id = int.Parse(Console.ReadLine());
    //    Console.Write("Username: ");
    //    string u = Console.ReadLine();
    //    Console.Write("Password: ");
    //    string p = Console.ReadLine();
    //    Console.Write("Email: ");
    //    string e = Console.ReadLine();

    //    using SqlConnection c = new SqlConnection(Db.conn);
    //    SqlCommand cmd = new SqlCommand(
    //        "INSERT INTO Users VALUES (@u,@p,@e,0,0)", c);
    //    //cmd.Parameters.AddWithValue("@i", id);
    //    cmd.Parameters.AddWithValue("@u", u);
    //    cmd.Parameters.AddWithValue("@p", p);
    //    cmd.Parameters.AddWithValue("@e", e);
    //    c.Open();
    //    cmd.ExecuteNonQuery();
    //}

    public void CreateUser()
    {
        Console.Write("Username: ");
        string u = Console.ReadLine();

        Console.Write("Password: ");
        string p = Console.ReadLine();

        Console.Write("Email: ");
        string e = Console.ReadLine();

        using SqlConnection c = new SqlConnection(Db.conn);

        SqlCommand cmd = new SqlCommand(
            @"INSERT INTO Users (Username, Password, Email, Subscription, Role)
          VALUES (@u, @p, @e, 0, 0);
          SELECT SCOPE_IDENTITY();", c);

        cmd.Parameters.AddWithValue("@u", u);
        cmd.Parameters.AddWithValue("@p", p);
        cmd.Parameters.AddWithValue("@e", e);

        c.Open();

        int newUserId = Convert.ToInt32(cmd.ExecuteScalar());

        Console.WriteLine("User created successfully!");
        Console.WriteLine("Your User ID is: " + newUserId);
    }


    public void viewall()
    {
        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand("SELECT * FROM Users", c);
        c.Open();
        SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
            Console.WriteLine($"{r["Id"]} {r["Username"]} {r["Role"]}");
    }

    public void removeuser(int id)
    {
        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand(
            "DELETE FROM Users WHERE Id=@id", c);
        cmd.Parameters.AddWithValue("@id", id);
        c.Open();
        cmd.ExecuteNonQuery();
    }

    public int UserLogin()
    {
        Console.Write("ID: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("Password: ");
        string p = Console.ReadLine();

        using SqlConnection c = new SqlConnection(Db.conn);
        SqlCommand cmd = new SqlCommand(
            "SELECT Role FROM Users WHERE Id=@i AND Password=@p", c);
        cmd.Parameters.AddWithValue("@i", id);
        cmd.Parameters.AddWithValue("@p", p);
        c.Open();
        object role = cmd.ExecuteScalar();
        if (role == null) return -1;
        return (int)role == 1 ? 999 : id;
    }
}

