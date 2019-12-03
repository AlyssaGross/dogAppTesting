using System;

using System.Data;
using System.Security.Permissions;

using MySql.Data;
using MySql.Data.MySqlClient;


namespace ConsoleApp1
{
    class Program
    {
        //function to display the Owner table from the database
        static void displayOwners(MySqlConnection conn)
        {
            try
            {
                conn.Open();
                string sql = "SELECT * FROM Owner";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Console.Clear();
                Console.WriteLine("Owners");
                Console.WriteLine("{0,-17}{1,-30}{2,-30}{3,-30}{4,-30}", "ID", "Name", "Email", "Phone #", "Password");
                while (reader.Read())
                {
                    Console.WriteLine("{0,-17}{1,-30}{2,-30}{3,-30}{4,-30}", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                }
                reader.Close();
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Cannot view Owners");
                Console.ReadLine();
            }
            conn.Close();
        }



        //function to add an entry to the Owner table from the database
        static void addOwner(MySqlConnection conn)
        {
            string ID, name, email, phoneNo, password;
            try
            {
                conn.Open();
                Console.Clear();
                Console.Write("Enter owner ID: ");
                ID = Console.ReadLine();
                Console.Write("Enter owner name: ");
                name = Console.ReadLine();
                Console.Write("Enter owner email: ");
                email = Console.ReadLine();
                Console.Write("Enter owner phone number: ");
                phoneNo = Console.ReadLine();
                Console.Write("Enter owner password: ");
                password = Console.ReadLine();

                string sql = "INSERT INTO Owner VALUES ('" + ID + "', '" + name + "', '" + email + "', '" + phoneNo + "', '" + password + "');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("Owner Added!");
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {

                Console.WriteLine("Owner could not be added");
                Console.ReadLine();
            }
            conn.Close();
        }

        //function to display the Screen table from the database
        static void displayDogs(MySqlConnection conn)
        {
            try
            {
                conn.Open();
                string ownerID;
                Console.Write("Enter OwnerID: ");
                ownerID = Console.ReadLine();
                string sql = "SELECT dogID, dogName FROM Dog WHERE ownerID = '" + ownerID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Console.Clear();
                Console.WriteLine("Dogs");
                Console.WriteLine("{0,-17}{1,-30}", "ID", "Name");
                while (reader.Read())
                {
                    Console.WriteLine("{0,-17}{1,-30}", reader.GetString(0), reader.GetString(1));
                }
                reader.Close();
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Cannot view Dogs");
                Console.ReadLine();
            }
            conn.Close();
        }

        //function to add an entry to the Screen table from the database
        static void addDog(MySqlConnection conn)
        {
            string dogID, ownerID, dogName;
            try
            {
                conn.Open();
                Console.Clear();
                Console.Write("Enter dog ID: ");
                dogID = Console.ReadLine();
                Console.Write("Enter owner ID: ");
                ownerID = Console.ReadLine();
                Console.Write("Enter dog name: ");
                dogName = Console.ReadLine();

                string sql = "INSERT INTO Dog (dogID, ownerID, dogName) VALUES ('" + dogID + "', '" + ownerID + "', '" + dogName + "');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("Dog Added!");
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {

                Console.WriteLine("Dog could not be added");
                Console.ReadLine();
            }
            conn.Close();
        }

        static void Main(string[] args)
        {
            string connString =  "server=remotemysql.com;user=Nh0almAE4r;database=Nh0almAE4r;port=3306;password=Qp9J3MiFJE;";
            //  string connString = "server=localhost;user=root;database=theaters;port=3306;password=birdy16!;";
            MySqlConnection conn = new MySqlConnection(connString);

            //menu to allow user to pick an option
            int option = 1;
            while (option > 0 & option < 5)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Movie Theater Database!");
                Console.WriteLine("1. View Owners");
                Console.WriteLine("2. Add Owner");
                Console.WriteLine("3. View Dogs");
                Console.WriteLine("4. Add Dog");
                Console.WriteLine("5. Exit");
                Console.Write("\nSelect an option: ");
                option = int.Parse(Console.ReadLine());
                Console.WriteLine(option);

                switch (option)
                {
                    case 1:
                        displayOwners(conn);
                        break;
                    case 2:
                        addOwner(conn);
                        break;
                    case 3:
                        displayDogs(conn);
                        break;
                    case 4:
                        addDog(conn);
                        break;


                }


            }




            conn.Close();

        }


    }





}
