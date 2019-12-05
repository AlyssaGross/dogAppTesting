

using System;
using MySql.Data.MySqlClient;


namespace ConsoleApp1
{
    class Program
    {
        // log in user with their email and password
        static void login(MySqlConnection conn)
        {
            string email, password, userID = "";
            try
            {
                Console.Clear();
                Console.WriteLine("Log In");
                Console.WriteLine("------\n");

                // get email and password from user
                Console.Write("Enter Email: ");
                email = Console.ReadLine();
                Console.Write("Enter Password: ");
                password = Console.ReadLine();

                conn.Open();
                // use query to get the ownerID matching the email and password 
                string sql = "Select ownerID from Owner where email = @email AND password = @password;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);
                MySqlDataReader reader = cmd.ExecuteReader();

                // if an owner with the email and password exist, store the ownerID in userID
                if (reader.Read())
                    userID = reader.GetString(0);
                reader.Close();
                conn.Close();

                // if there is no owner with the entered username and password display an error message 
                // otherwise send user to user Menu
                if (userID != "")
                    userMenu(conn, userID);
                else
                {
                    Console.WriteLine("Invalid Email or Password");
                    Console.ReadLine();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Login Failed");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // create new account (new owner)
        static void createOwner(MySqlConnection conn)
        {
            string ID, name, email, phoneNo, password;
            try
            {
                Console.Clear();
                Console.WriteLine("Create Account");
                Console.WriteLine("--------------\n");

                // get name, email, phone number, and password for the new account
                Console.Write("Enter Owner Name: ");
                name = Console.ReadLine();
                Console.Write("Enter Email: ");
                email = Console.ReadLine();
                Console.Write("Enter Phone Number: ");
                phoneNo = Console.ReadLine();
                Console.Write("Enter Password: ");
                password = Console.ReadLine();

                // use query to get the next ownerID value from the AutoID table
                ID = nextID(conn, "OWNER");

                conn.Open();
                // create new account by inserting values into the Owner table
                String sql = "INSERT INTO Owner VALUES (@ownerID, @ownerName, @email, @phoneNo, @password);";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ownerID", ID);
                cmd.Parameters.AddWithValue("@ownerName", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phoneNo", phoneNo);
                cmd.Parameters.AddWithValue("@password", password);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                conn.Close();

                // if the account was created with no problems
                // increment the ownerID in the AutoID table by calling incrementID
                incrementID(conn, "OWNER");
                Console.Clear();
                Console.WriteLine("Account Created!");
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Owner could not be added");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // displays user menu and directs the user to their selection
        static void userMenu(MySqlConnection conn, string userID)
        {
            // have user select dog
            string dogID = selectDog(conn, userID);
            int option = 1;
            if (dogID != "")
            {
                while (option > 0 && option < 7)
                {
                    Console.Clear();
                    Console.WriteLine("1. Events");
                    Console.WriteLine("2. Notes");
                    Console.WriteLine("3. Log History");
                    Console.WriteLine("4. Dog Information");
                    Console.WriteLine("5. Account Information");
                    Console.WriteLine("6. Switch Dogs");
                    Console.WriteLine("7. Log Out");
                    option = int.Parse(Console.ReadLine());
                    switch (option)
                    {
                        //Events
                        case 1:
                            eventMenu(conn, userID, dogID);
                            break;
                        //Notes
                        case 2:
                            noteMenu(conn, dogID);
                            break;
                        //Log History
                        case 3:
                            eventLogs(conn, "%", dogID);
                            break;
                        //Dog Information
                        case 4:
                            dogInfo(conn, dogID);
                            break;
                        //Account Information
                        case 5:
                            accountInfo(conn, userID);
                            break;
                        //Switch Dogs
                        case 6:
                            dogID = selectDog(conn, userID);
                            break;
                            //Log Out
                    }
                }
            }
        }

        static string selectDog(MySqlConnection conn, string userID)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Dogs");
                Console.WriteLine("----\n");

                int numDogs, dog, i = 0;
                string[] dogs; //array to hold the owner's dogs' IDs

                conn.Open();
                // use query to get the number of dogs the owner has
                string sql = "select count(dogID) from Dog where ownerID = @userID;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                numDogs = int.Parse(reader.GetString(0));
                reader.Close();

                dogs = new string[numDogs];

                // use query to get dogIDs for all owners dogs and store them, and display the 
                // corresponding dog name for the owner to select
                sql = "select dogID, dogName from Dog where ownerID = @userID;";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dogs[i] = reader.GetString(0);
                    Console.WriteLine((i + 1) + ". {0,-30}", reader.GetString(1));
                    i++;
                }
                reader.Close();
                conn.Close();

                // display options to add a dog or to log out
                Console.WriteLine((i + 1) + ". Add dog");
                Console.WriteLine((i + 2) + ". Log Out");
                dog = int.Parse(Console.ReadLine()) - 1;

                // add dog and return to select dog menu after
                if (dog == i)
                {
                    addDog(conn, userID);
                    return selectDog(conn, userID);
                }
                else if (dog < i)
                    // return the dogID of the selected dog
                    return dogs[dog];
                else
                    return "";
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Could not select dog");
                Console.WriteLine(ex);
                Console.ReadLine();
                return "";
            }
        }

        //add dog to an owners account
        static void addDog(MySqlConnection conn, string ownerID)
        {
            string dogID, dogName;
            try
            {
                Console.Clear();
                Console.WriteLine("Add Dog");
                Console.WriteLine("-------\n");

                //get dog's name from user
                Console.Write("Enter dog name: ");
                dogName = Console.ReadLine();

                //get the next dogID from AutoID table
                dogID = nextID(conn, "DOG");

                conn.Open();
                // add new dog by inserting values into the Dog table
                string sql = "INSERT INTO Dog (dogID, ownerID, dogName) VALUES (@dogID, @ownerID, @dogName);";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dogID", dogID);
                cmd.Parameters.AddWithValue("@ownerID", ownerID);
                cmd.Parameters.AddWithValue("@dogName", dogName);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                conn.Close();

                // if the dog was created with no problems
                // increment the dogID in the AutoID table by calling incrementID
                incrementID(conn, "DOG");
                Console.WriteLine("Dog Added!");
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Dog could not be added");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // display the event menu with the list of events and their options
        static void eventMenu(MySqlConnection conn, string userID, string dogID)
        {
            int numEvents, selectedEvent = 1, i = 2, option;
            string[] events;
            try
            {
                while (selectedEvent >= 0 && selectedEvent < i)
                {
                    Console.Clear();
                    Console.WriteLine("Events");
                    Console.WriteLine("------\n");

                    conn.Open();
                    // use a query to get the number of Events the user has
                    string sql = "select count(eventID) from Event where ownerID = @userID;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userID", userID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    numEvents = int.Parse(reader.GetString(0));
                    reader.Close();
                    events = new string[numEvents];

                    // use query to get eventIDs for all owners events and store them, and display the 
                    // corresponding event name for the owner to select          
                    sql = "select eventID, eventName from Event where ownerID = @userID AND displayEvent = 'Y';";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userID", userID);
                    reader = cmd.ExecuteReader();

                    i = 0;
                    while (reader.Read())
                    {
                        events[i] = reader.GetString(0);
                        Console.WriteLine((i + 1) + ". {0,-30}", reader.GetString(1));
                        i++;
                    }
                    reader.Close();
                    conn.Close();

                    // display options to add a event or go back (to user menu)
                    Console.WriteLine((i + 1) + ". Add Event");
                    Console.WriteLine((i + 2) + ". Go Back");
                    selectedEvent = int.Parse(Console.ReadLine()) - 1;

                    // redirect user to add event and then return user to event menu
                    if (selectedEvent == i)
                    {
                        addEvent(conn, userID);
                        eventMenu(conn, userID, dogID);
                    }
                    // display options for the events and redirect the user
                    else if (selectedEvent >= 0 && selectedEvent < i)
                    {
                        option = 1;
                        while (option > 0 && option < 4)
                        {
                            Console.Clear();
                            Console.WriteLine("1. Logs");
                            Console.WriteLine("2. Reminders");
                            Console.WriteLine("3. Remove Event From Menu");
                            Console.WriteLine("4. Go Back");
                            option = int.Parse(Console.ReadLine());
                            // Logs
                            if (option == 1)
                                logMenu(conn, events[selectedEvent], dogID);
                            // Reminders
                            else if (option == 2)
                                reminderMenu(conn, events[selectedEvent], dogID);
                            // Remove Event From Menu
                            else if (option == 3)
                                deleteEvent(conn, events[selectedEvent]);
                            // Go Back (to Events)
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Could not select event");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // add a new event for a user/owner
        static void addEvent(MySqlConnection conn, string ownerID)
        {
            string eventID, eventName, eventDescr;
            try
            {
                Console.Clear();
                Console.WriteLine("Add Event");
                Console.WriteLine("---------\n");

                // get the event name and optional description from the user
                Console.Write("Enter event name: ");
                eventName = Console.ReadLine();
                Console.Write("Enter event description: ");
                eventDescr = Console.ReadLine();

                // use query to get the next eventID from the AutoID table
                eventID = nextID(conn, "EVENT");

                conn.Open();
                // use query to add the new event for the user by inserting it into the Event table
                string sql = "INSERT INTO Event VALUES (@eventID, @ownerID, @eventName, @eventDescr, 'Y');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@eventID", eventID);
                cmd.Parameters.AddWithValue("@ownerID", ownerID);
                cmd.Parameters.AddWithValue("@eventName", eventName);
                cmd.Parameters.AddWithValue("@eventDescr", eventDescr);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                conn.Close();

                //if the event was successfully added increment the next eventID in the AutoID table
                incrementID(conn, "EVENT");
                Console.WriteLine("Event Added!");
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Event could not be added");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }


        // display the Event's Logs and allow user to add a log
        static void logMenu(MySqlConnection conn, string eventID, string dogID)
        {
            int option = 1;
            while (option == 1)
            {
                // show the log history for the event
                eventLogs(conn, eventID, dogID);

                Console.WriteLine("\n1. Add Log");
                Console.WriteLine("2. Back");
                option = int.Parse(Console.ReadLine());
                if (option == 1)
                    // add Log
                    addLog(conn, eventID, dogID);
            }
        }

        // display the logs for an event
        static void eventLogs(MySqlConnection conn, string eventID, string dogID)
        {
            try
            {
                conn.Open();
                Console.Clear();
                Console.WriteLine("Event Logs");
                Console.WriteLine("----------\n");

                // use query to get and display all logs for the event and dog
                string sql = "SELECT eventName, dateTimeLogged, logNotes FROM Log JOIN Event USING (eventID) WHERE eventID like @eventID AND  dogID = @dogID;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@eventID", eventID);
                cmd.Parameters.AddWithValue("@dogID", dogID);

                if (eventID == "%")
                    Console.WriteLine("{0, -25}  {1, -30}  {2}", "DateTime Logged", "Event", "Log Notes");
                else
                    Console.WriteLine("{0, -25}  {1}", "DateTime Logged", "Log Notes");

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (eventID == "%")
                        Console.WriteLine("{0, -25}  {1, -30}  {2}", reader.GetDateTime(1), reader.GetString(0), reader.GetString(2));
                    else
                        Console.WriteLine("{0, -25}  {1}", reader.GetDateTime(1), reader.GetString(2));
                }
                reader.Close();
                conn.Close();

                if (eventID == "%")
                    Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Logs could not be retrieved");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // add a log for an event
        static void addLog(MySqlConnection conn, string eventID, string dogID)
        {
            string logID, logNotes;
            // get the current date and time
            DateTime dateTimeLogged = DateTime.Now;
            try
            {
                Console.Clear();
                Console.WriteLine("Add Log");
                Console.WriteLine("-------\n");

                // get log notes from the user
                Console.Write("Enter log notes: ");
                logNotes = Console.ReadLine();

                // use query to get the next logID from the AutoID table
                logID = nextID(conn, "LOG");

                conn.Open();
                // insert the log into the Log table
                string sql = "INSERT INTO Log VALUES (@logID, @eventID, @dogID, @dateTimeLogged, @logNotes);";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@logID", logID);
                cmd.Parameters.AddWithValue("@eventID", eventID);
                cmd.Parameters.AddWithValue("@dogID", dogID);
                cmd.Parameters.AddWithValue("@dateTimeLogged", dateTimeLogged);
                cmd.Parameters.AddWithValue("@logNotes", logNotes);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                conn.Close();

                //if the log was inserted without problems increment the logID in the AutoID table
                incrementID(conn, "LOG");
                Console.WriteLine("Log Added!");
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Log could not be added");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // display event reminders and the options to add a reminder
        static void reminderMenu(MySqlConnection conn, string eventID, string dogID)
        {
            int option = 1;
            while (option == 1)
            {
                // display the event's existing reminders
                eventReminders(conn, eventID, dogID);
                // add log
                Console.WriteLine("\n1. Add Reminder");
                Console.WriteLine("2. Back");
                option = int.Parse(Console.ReadLine());
                if (option == 1)
                    // add a new reminder
                    addReminder(conn, eventID, dogID);
            }

        }

        // display existing reminders for an event
        static void eventReminders(MySqlConnection conn, string eventID, string dogID)
        {
            try
            {
                conn.Open();
                Console.Clear();
                Console.WriteLine("Event Reminders");
                Console.WriteLine("---------------\n");

                // use query to get the reminders for the event and display them all
                string sql = "SELECT reminderName, reminderDateTime, frequencyVal, frequencyOption, reminderStatus FROM Reminder WHERE eventID = @eventID AND  dogID = @dogID ;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@eventID", eventID);
                cmd.Parameters.AddWithValue("@dogID", dogID);

                Console.WriteLine("{0, -30}  {1, -25}  {2, -10} {3}", "Reminder Name", "Reminder DateTime", "Frequency", "Status");
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("{0, -30}  {1, -25}  {2, -3} {3,-6} {4}", reader.GetString(0), reader.GetDateTime(1), reader.GetInt16(2), reader.GetString(3), reader.GetString(4));
                }
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Reminders could not be retrieved");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }


        // add a new reminder for an event
        static void addReminder(MySqlConnection conn, string eventID, string dogID)
        {
            string reminderID, reminderName, frequencyOption;
            DateTime reminderDateTime;
            int frequencyVal;
            try
            {
                Console.Clear();
                Console.WriteLine("Add Reminder");
                Console.WriteLine("------------\n");

                // get the reminder name, datetime, and frequency from the user
                Console.Write("Enter Reminder Name: ");
                reminderName = Console.ReadLine();
                Console.Write("Enter Reminder Date Time: ");
                reminderDateTime = DateTime.Parse(Console.ReadLine());
                Console.Write("Enter Reminder Frequency Number: ");
                frequencyVal = int.Parse(Console.ReadLine());
                Console.Write("Enter Reminder Frequency Type: ");
                frequencyOption = Console.ReadLine();

                // use query to get the next reminderID from the AutoID table
                reminderID = nextID(conn, "RMNDR");

                conn.Open();
                // use query to add the new reminder by inserting the reminder into the Reminder table
                string sql = "INSERT INTO Reminder VALUES (@reminderID, @eventID, @dogID, @reminderName, @reminderDateTime, @frequencyVal, @frequencyOption, 'O');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@reminderID", reminderID);
                cmd.Parameters.AddWithValue("@eventID", eventID);
                cmd.Parameters.AddWithValue("@dogID", dogID);
                cmd.Parameters.AddWithValue("@reminderName", reminderName);
                cmd.Parameters.AddWithValue("@reminderDateTime", reminderDateTime);
                cmd.Parameters.AddWithValue("@frequencyVal", frequencyVal);
                cmd.Parameters.AddWithValue("@frequencyOption", frequencyOption);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                conn.Close();

                // if the reminder was successfully added, increment the reminderID in the AutoID table
                incrementID(conn, "RMNDR");
                Console.WriteLine("Reminder Added!");
                Console.ReadLine();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Reminder could not be added");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // remove event from displaying in the menu
        static void deleteEvent(MySqlConnection conn, string eventID)
        {
            try
            {
                conn.Open();
                // use query to update the displayEvent flag to No for the event 
                string sql = "UPDATE Event SET displayEvent = 'N' WHERE eventID = @eventID";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@eventID", eventID);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                Console.WriteLine("Event Removed From  Menu!");
                Console.ReadLine();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Event could not be removed from Menu");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // display menu for notes with existing notes to view, and option to add a note
        static void noteMenu(MySqlConnection conn, string dogID)
        {
            try
            {
                conn.Open();
                int numNotes, selectedNote, i = 0;
                string[] notes;
                Console.Clear();
                Console.WriteLine("Notes");
                Console.WriteLine("-----\n");

                // use query to get the number of notes for the dog
                string sql = "select count(noteID) from Notes where dogID = @dogID;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dogID", dogID);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                numNotes = int.Parse(reader.GetString(0));
                reader.Close();
                notes = new string[numNotes];

                // use query to get all the notes for the dog and display the titles in a numbered list for the user to select from
                sql = "select noteID, noteName, dateTimeAdded from Notes where dogID = @dogID;";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dogID", dogID);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    notes[i] = reader.GetString(0);
                    Console.WriteLine((i + 1) + ". {0,-30} {1}", reader.GetString(1), reader.GetString(2));
                    i++;
                }
                reader.Close();
                conn.Close();

                //add options to add a note or go back (to user menu)
                Console.WriteLine((i + 1) + ". Add Note");
                Console.WriteLine((i + 2) + ". Go Back");
                selectedNote = int.Parse(Console.ReadLine()) - 1;

                // add Note and return to note menu
                if (selectedNote == i)
                {
                    addNote(conn, dogID);
                    noteMenu(conn, dogID);
                }
                else if (selectedNote >= 0 && selectedNote < numNotes)
                {
                    // display the contents of the note, then return to the note menu
                    viewNote(conn, notes[selectedNote]);
                    noteMenu(conn, dogID);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Could not process selection");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // add a note for an dog
        static void addNote(MySqlConnection conn, string dogID)
        {
            try
            {
                string noteID, noteName, noteBody;
                Console.Clear();
                Console.WriteLine("Add Note");
                Console.WriteLine("--------\n");

                //get the current date and time and get the note name and body from the user
                DateTime dateTimeAdded = DateTime.Now;
                Console.WriteLine("Enter Note Name: ");
                noteName = Console.ReadLine();
                Console.WriteLine("Enter Note: ");
                noteBody = Console.ReadLine();

                // use query to get the next noteID from the AutoID table
                noteID = nextID(conn, "NOTES");

                conn.Open();
                // use query to add note by inserting it into the Notes table
                string sql = "INSERT INTO Notes VALUES (@noteID, @dogID, @noteName, @noteBody, @dateTimeAdded);";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("noteID", noteID);
                cmd.Parameters.AddWithValue("@dogID", dogID);
                cmd.Parameters.AddWithValue("@noteName", noteName);
                cmd.Parameters.AddWithValue("@noteBody", noteBody);
                cmd.Parameters.AddWithValue("@dateTimeAdded", dateTimeAdded);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                conn.Close();

                //if the note was successfully added then increment the noteID in the AutoID table
                Console.WriteLine("Note Added!");
                incrementID(conn, "NOTES");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Could not add note");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // view the body of a note
        static void viewNote(MySqlConnection conn, string noteID)
        {
            try
            {
                conn.Open();
                Console.Clear();
                Console.WriteLine("Note");
                Console.WriteLine("----\n");

                // use query to get the name and the body of the note and display them
                string sql = "SELECT noteName, noteBody FROM Notes WHERE noteID = @noteID;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@noteID", noteID);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                Console.WriteLine(reader.GetString(0));
                Console.WriteLine(reader.GetString(1));
                reader.Close();
                Console.ReadLine();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Cannot view note");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // view and update the information for a dog
        static void dogInfo(MySqlConnection conn, string dogID)
        {
            try
            {
                conn.Open();
                Console.Clear();
                Console.WriteLine("Dog Information");
                Console.WriteLine("---------------\n");

                // use query to get and display dog's current information
                string sql = " SELECT IFNULL(weight, 0), IFNULL(breed, \"\"), IFNULL(gender, \"\"), IFNULL(birthday, '1970-01-01'), IFNULL(other, \"\") FROM Dog WHERE dogID = @dogID;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dogID", dogID);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                Console.WriteLine("Weight   : " + reader.GetDouble(0));
                Console.WriteLine("Breed    : " + reader.GetString(1));
                Console.WriteLine("Gender   : " + reader.GetString(2));
                Console.WriteLine("Birthday : " + DateTime.Parse(reader.GetString(3)).ToString("d"));
                Console.WriteLine("Other    : " + reader.GetString(4));
                reader.Close();
                string breed, gender, other, weight;
                string birthday;

                // get any new data from the user 
                Console.WriteLine("\nLeave any field blank to leave data unchanged");
                Console.Write("Update Weight: ");
                weight = Console.ReadLine();
                Console.Write("Update Breed: ");
                breed = Console.ReadLine();
                Console.Write("Update Gender: ");
                gender = Console.ReadLine();
                Console.Write("Update Birthday: ");
                birthday = Console.ReadLine();
                Console.Write("Update Other Information: ");
                other = Console.ReadLine();

                // for each field, if the user entered a new value
                // use a query to update the value of the field for that dog's information in the Dog table
                if (weight != "")
                {
                    sql = "UPDATE Dog SET weight = @weight WHERE dogID = @dogID;";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@weight", double.Parse(weight));
                    cmd.Parameters.AddWithValue("@dogID", dogID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                if (breed != "")
                {
                    sql = "UPDATE Dog SET breed = @breed WHERE dogID = @dogID;";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@breed", breed);
                    cmd.Parameters.AddWithValue("@dogID", dogID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                if (gender != "")
                {
                    sql = "UPDATE Dog SET gender = @gender WHERE dogID = @dogID;";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@dogID", dogID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                if (birthday != "")
                {
                    sql = "UPDATE Dog SET birthday = @birthday WHERE dogID = @dogID;";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@birthday", DateTime.Parse(birthday));
                    cmd.Parameters.AddWithValue("@dogID", dogID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                if (other != "")
                {
                    sql = "UPDATE Dog SET other = @other WHERE dogID = @dogID;";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@other", other);
                    cmd.Parameters.AddWithValue("@dogID", dogID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Information not updated");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // view and update account Information for the owner/user
        static void accountInfo(MySqlConnection conn, string ownerID)
        {
            try
            {
                conn.Open();
                Console.Clear();
                Console.WriteLine("Account Information");
                Console.WriteLine("-------------------\n");

                // use query to get current account information for a user and display it
                string sql = " SELECT ownerName, email, phoneNo, password FROM Owner WHERE ownerID = @ownerID;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ownerID", ownerID);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                Console.WriteLine("Name         : " + reader.GetString(0));
                Console.WriteLine("Email        : " + reader.GetString(1));
                Console.WriteLine("Phone Number : " + reader.GetString(2));
                Console.WriteLine("Password     : " + reader.GetString(3));
                reader.Close();
                string name, email, phoneNo, password;

                // get any new data from the user
                Console.WriteLine("\nLeave any field blank to leave data unchanged");
                Console.Write("Update Name: ");
                name = Console.ReadLine();
                Console.Write("Update Email: ");
                email = Console.ReadLine();
                Console.Write("Update Phone Number: ");
                phoneNo = Console.ReadLine();
                Console.Write("Update Password: ");
                password = Console.ReadLine();

                // for each field, if the user entered a new value
                // use a query to update the value of the field for that users's information in the Owner table
                if (name != "")
                {
                    sql = "UPDATE Owner SET ownerName = @name WHERE ownerID = @ownerID";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@ownerID", ownerID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                if (email != "")
                {
                    sql = "UPDATE Owner SET email = @email WHERE ownerID = @ownerID";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@ownerID", ownerID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                if (phoneNo != "")
                {
                    sql = "UPDATE Owner SET phoneNo = @phoneNo WHERE ownerID = @ownerID";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@phoneNo", phoneNo);
                    cmd.Parameters.AddWithValue("@ownerID", ownerID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                if (password != "")
                {
                    sql = "UPDATE Owner SET password = @password WHERE ownerID = @ownerID";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@ownerID", ownerID);
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Information not updated");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        // get the next ID from the AutoID table for the specified type of ID
        static string nextID(MySqlConnection conn, string type)
        {
            string ID = "";
            try
            {
                conn.Open();
                string sql = "SELECT nextID FROM AutoID WHERE typeID = @type;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@type", type);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                ID = reader.GetString(0);
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Could not retireve the next autonumbered ID");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            return ID;
        }

        // increment the current nextID in the AutoID table to the next ID
        static void incrementID(MySqlConnection conn, string type)
        {
            string sID, formatStr;
            int iID, len, i;
            try
            {
                conn.Open();
                // use query to get the current nextID for a type of AutoID
                string sql = "SELECT nextID FROM AutoID WHERE typeID = @type;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@type", type);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                sID = reader.GetString(0);
                reader.Close();

                len = sID.Length - 1;
                // get the number portion of the ID and add 1 to it
                iID = int.Parse(sID.Substring(1, len)) + 1;

                //create a format string for the correct length
                formatStr = "";
                for (i = 0; i < len; i++)
                    formatStr += "0";

                // put the incremented number portion back into the ID
                sID = sID.Substring(0, 1) + iID.ToString(formatStr);

                // use query to update the value of nextID in the AutoID table for a type of AutoID
                sql = "UPDATE AutoID SET nextID = @sID WHERE typeID = @type;";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@sID", sID);
                cmd.Parameters.AddWithValue("@type", type);
                reader = cmd.ExecuteReader();
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Could not increment the AutoID");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        static void Main(string[] args)
        {
            string connString = "server=remotemysql.com;user=Nh0almAE4r;database=Nh0almAE4r;port=3306;password=Qp9J3MiFJE;";
            MySqlConnection conn = new MySqlConnection(connString);


            //menu to allow user to log in or create an account
            int option = 1;
            while (option > 0 & option < 3)
            {
                Console.Clear();
                Console.WriteLine("Dog App");
                Console.WriteLine("-------\n");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Create Account");
                Console.WriteLine("3. Exit\n");
                option = int.Parse(Console.ReadLine());
                Console.WriteLine(option);

                switch (option)
                {
                    case 1:
                        login(conn);
                        break;
                    case 2:
                        createOwner(conn);
                        break;
                }
            }
            conn.Close();
        }
    }
}
