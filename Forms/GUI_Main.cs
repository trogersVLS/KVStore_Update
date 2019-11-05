/* GUI_Main.cs
 * Partial class GUI_Main
 * 
 * - To be used with GUI_Main.Designer.cs (a visual studio generated file)
 * 
 * Author: Taylor Rogers
 * Date: 10/23/2019
 * 
 */
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;




namespace KVStore_Update
{


    public partial class GUI_Main : Form
    {
        //RUN Button possible strings
        const string RUN = "Run";
        const string ETH_EN = "Enable Ethernet";
        const string CANCEL = "Cancel";
        const string OK = "OK";

        //Non designer GUI components

        //Users that have admin privelege
        const string ADMIN = "admin";
        const string BLANK = "";
        
        //Log variable - Changes if program is run by an admin
        bool LogData = true;
        //Database variables
        string location;
        int eqid;
        string user_id;
        string serial;


        //Equipment connections
        MccDaq_GPIO GPIO;
        

        

        //Functional Test Data
        
        List<TestData> Session_Tests= new List<TestData>();
        FunctionalTest test;
        private readonly ConcurrentQueue<string> message_queue = new ConcurrentQueue<string>();
        public GUI_Main(string user)
        {   
            //Initialize the GUI components
            InitializeComponent();

            //Initialize the user
            this.user_id = user;
            this.Label_Username.Text = user;
            if ((this.user_id == ADMIN) | (this.user_id == BLANK))
            {
                this.Check_LogToDatabase.Show();
                this.Check_LogToDatabase.Checked = true;
            }

            else
            {
                this.Check_LogToDatabase.Show();
                this.Check_LogToDatabase.Checked = true;
                this.Check_LogToDatabase.Enabled = false;
            }

            //Initialize Settings
            this.GetSettings();
            this.Reset_GUI();
            this.ConnectDevices();


        }
        /*****************************************************************************************************************************************
         * GetSettings
         * 
         * Function: Reads from the settings.xml file, collects the system configurations. Collects the specs This list is the tests that will
         * run during a full functional test.
         * 
         * Arguments: None
         * 
         * Returns: None - Update the class variable Tests
         *
         *********************************************************************************************************************************************/
        private void ConnectDevices()
        {

            this.GPIO = new MccDaq_GPIO();
            if (this.GPIO.Connected)
            {
                this.Button_GPIO.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
                this.Button_GPIO.Enabled = false;
            }
            else
            {
               
            }


        }
        /*****************************************************************************************************************************************
         * ConnectDevices
         * 
         * Function: Connects to all peripheral devices that are used in this test fixture
         * 
         * Arguments: None
         * 
         * Returns: None - Creates new class object instances and updates the displayed connections 
         *
         *********************************************************************************************************************************************/
        private bool CheckDeviceConnections()
        {
            bool connected = false;           
        
            
        this.GPIO = new MccDaq_GPIO();
        if (this.GPIO.Connected)
        {
            this.Button_GPIO.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
            this.Button_GPIO.Enabled = false;
            connected = true;
        }
        else
        {
            
        }
            

            return connected;
        }
        /*****************************************************************************************************************************************
         * GetSettings
         * 
         * Function: Reads from the settings.xml file, collects the system configurations. Collects the specs This list is the tests that will
         * run during a full functional test.
         * 
         * Arguments: Device - Takes a string variable as the device connection type and attempts to reconnect to the device.
         * 
         * Returns: None - Updates class object instances
         *
         *********************************************************************************************************************************************/
        private XmlNode GetSettings()
        {
            //open the settings.xml file

            XmlDocument configuration = new XmlDocument();
            XmlNode settings_node;
            XmlNode user_ids = null;

            
            configuration.Load(@"..\..\Configuration\settings.xml");


            foreach(XmlNode xml in configuration.DocumentElement.ChildNodes)
            {
                if(xml.Name == "settings")
                {
                    settings_node = xml;
                    this.eqid = Convert.ToInt32(settings_node.Attributes["eqid"].Value);
                    this.location = settings_node.Attributes["location"].Value;
                    
                }

                
            }
            
            return user_ids;

        }


        /******************************************************************************************************
         * Button_Run_Click(): event handler for run test button click. 
         * 
         * Based on the state of the checkboxes, this event handler will initiate a long running async task
         * 
         * Program && FunctionalTest --> Runs the FunctionalTest.RunTest in an asynchronous task
         * Program --> Runs the FunctionalTest.Program() method in an asynchronous task
         * FunctionalTest -- > Runs the FunctionalTest.RunTest() method in an asynchronous task, won't program the board
         * None checked --> Does nothing. Displays a message on the debug console.
         * 
         * 
         * This function will wait until the task finishes before exiting. // Not sure if that is a good thing or not yet
         * 
         * ****************************************************************************************************/
        private async void Button_Run_Click(object sender, EventArgs e)
        {

            //Constant string variables for brevity
            
            //async progress variables. Update the linked property when changed, hooking the appropriate event handler to update the GUI.
            var progress = new Progress<int>(i => StatusBar.Value = i);
            var messages = new Progress<string>(s => this.Debug_Output(s));
            Hashtable Test_Params;

            this.StatusBar.Value = 0; //Set the statusbar value to 0 to indicate that the test is about to start.

            //Button is pressed while program is not doing anything
            if (this.Button_Run.Text == RUN)
            {
                if (this.GPIO.Connected)
                {
                    //Change button text to indicate to user that the button serves a new function now. 
                    this.Button_Run.Text = CANCEL;

                    //Running a new instance of a functional test
                    if (this.LogData)
                    {
                        //Production environment, data is logged to local database.
                        //Create a dictionary to pass to the functional test class with all the information needed to run and log a test
                        Test_Params = Create_Test_Table();
                        this.test = new FunctionalTest(this.message_queue, Test_Params);
                    }
                    else
                    {
                        //Development environment, data is not logged to a database and is only displayed on the debug window.
                        this.test = new FunctionalTest(this.message_queue);
                    }


                    //Control board first pass. The board requires programming and functional testing
                    if (Check_Program.Checked)
                    {
                        console_debugOutput.Text = "Starting KVStore update";


                        await Task.Factory.StartNew(() => this.test.RunTest(progress, messages),
                                                    TaskCreationOptions.LongRunning);
                        //Check to see if the program passed

                        this.EndTest();
                    }

                    else
                    {
                        console_debugOutput.Text = "Please select an action or exit the program";
                    }
                    this.StatusBar.Value = 100;
                    
                }
                else
                {
                    console_debugOutput.Text = "GPIO module is not connected, please connect the module and restart the program";
                }
            }
            else if(this.Button_Run.Text == ETH_EN)
            {

                this.console_debugOutput.Text = ("Enabling ethernet on the sd card");
                if (this.SD_EnableEthernet())
                {   
                    this.Button_Run.Text = RUN;

                    this.console_debugOutput.Text = ("Ethernet enable successful.\r\nPlease remove the card and place back into the UUT\n\n\rPress RUN when ready");
                }
                else
                {
                    this.console_debugOutput.Text = ("Could not find SD card, is it installed correctly?");
                }
            }
            else if(this.Button_Run.Text == CANCEL)
            {   //Clear the queue and send message to cancel functional test thread.
                while (!this.message_queue.IsEmpty)
                {
                    this.message_queue.TryDequeue(out string str);
                }
                this.message_queue.Enqueue("cancel");

            }
            
        }

        private Hashtable Create_Test_Table()
        {   
            //Dictionary<string,string> param = new Dictionary<string,string>();
            Hashtable param = new Hashtable();
            string test_id;
            string eqid;
            string user;
            string location;
            string timestamp;
            string serial;
            string result;
            //Test_id
            DateTime date = DateTime.UtcNow;
            test_id = this.serial + "_" + date.Year.ToString() +
                                                 date.DayOfYear.ToString() +
                                                 date.Hour.ToString() +
                                                 date.Minute.ToString() +
                                                 date.Second.ToString() +
                                                 date.Millisecond.ToString();
            param.Add("test_id", test_id);
            //Equipment id --> Stored in settings.xml
            eqid = this.eqid.ToString();
            param.Add("eqid", eqid);
            //User id --> These usernames are stored in the settings.xml file. Each user will login prior to opening the program
            user = this.user_id;
            param.Add("user_id", user_id);
            //Location --> Also stored in the settings.xml file. Will be updated after the device moves to a new location
            location = this.location;
            param.Add("location", location);
            //Grabs the current timestamp in local time and converts to string, this value is human readable
            timestamp = DateTime.Now.ToString();
            param.Add("timestamp", timestamp);
            //Store the serial number, the serial number is unique to the board, but the board can be tested multiple times
            serial = this.serial;
            param.Add("serial", serial);
            //The result is false until the functional test class sets the result to true
            result = false.ToString();
            param.Add("result", result);

            //Pointers to test equipment instances (Power supply, gpio, dmm, SOM)
            param.Add("gpio", this.GPIO);

            return param;
        }
        /******************************************************************************************************
         * EndTest()
         * - Method displays a dialog prompt to let the user save to a new location.
         *   The new save location is then updated in the configuration file.
         *   
         * 
         * ****************************************************************************************************/
        private void EndTest()
        {

            try
            {
                var user_confirmation = MessageBox.Show("Test is finished, saving test results\n" + "Results: " + this.test.results["result"], "Saved!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                bool success = this.SaveResults(this.test.results);

            }
            catch
            {
                //Error catching for null reference exceptions
            }
           
            this.Reset_GUI();

        }

        private bool SaveResults(Dictionary<string,string> test_results)
        {
            /*
             * | Serial, Date, User, CMD 1, CMD2, CMD3, CMD4 |
             */
            string resultsFileName = "..\\..\\Results\\ECO-2375_Rework_Log.csv";
            string results = "";
            try
            {
                
                results = "\n\r" + test_results["serial"] + "," + DateTime.Now.ToString() + "," + this.user_id + "," + test_results["cmd_1"] + "," + test_results["cmd_2"] + "," + test_results["cmd_3"] + "," + test_results["cmd_4"];
            }
            catch
            {
                return false;
            }
            if (!File.Exists(resultsFileName))
            {
                string results_header = "SerialNumber" + "," + "TimeStamp" + "," + "Test_Operator" + "," + "Command 1 Result" + "," + "Command 2 Result" + "," + "Command 3 Result" + "," + "Command 4 Result";
                File.WriteAllText(resultsFileName, results_header);
            }
            try
            {
                File.AppendAllText(resultsFileName, results);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool SD_EnableEthernet()
        {
            bool success = false;
            //Check to see which USB devices are present. Find the first letter drive available that is not the C drive.
            DriveInfo[] alldrives = DriveInfo.GetDrives();
            foreach(DriveInfo drive in alldrives)
            {
                //Check to see if ipl_nand.bin exists on the uSD card, if so, then this is the correct drive and we can write the required file to 
                string filepath = drive.Name;
                if (File.Exists(filepath + "ipl_nand.bin"))
                {
                    //Write the ethernetenable file to this path if the file does not exist
                    if (!File.Exists(filepath + "ethernetenable"))
                    {
                        File.Create(filepath + "ethernetenable").Close();
                    }

                    //Confirm that the file exists
                    if(File.Exists(filepath + "ethernetenable")) {
                        success = true;
                        break;
                    }
                    
                }

            }



            return success;
        }
        /******************************************************************************************************
         * Field_SerialNumber_KeyUp
         * - Method checks to see if the user has finished inputting a serial number by pressing <Enter>.
         *   This allows the user to use a scanner to enter the serial number of the boards.
         *   
         *   TODO: Create file for saved configuration settings. This will allow for each installation of this 
         *   program to be configured specifically for the manufacturer
         *   
         *   Serial number format: @ A # # $ # # #
         *   @ - Manufacturer code
         *   # - A number
         *   $ - Letter to indicate the two week period in which the board was made
         *   
         * 
         * ****************************************************************************************************/
        private void Field_SerialNumber_KeyUp(object sender, KeyEventArgs e)
        {   

            //Only update the serial number if the user hits enter.
            //This field is then locked until the GUI is reset after a test has run.
            if (e.KeyData == Keys.Enter)
            {   
                //TODO: Check if the serial is valid and/or has changed.
                this.console_debugOutput.Text = "Serial Number =  " + this.Field_SerialNumber.Text;
                this.console_debugOutput.AppendText("\n\rPlease remove the SD card from the UUT, place in SD card reader. Press \"Enable Ethernet\" when done");
                this.serial = this.Field_SerialNumber.Text;

                this.Field_SerialNumber.Enabled = false;
                 
                //Uncheck all checkboxes;
                this.Check_All();
                this.Button_Run.Text = ETH_EN;
                this.Button_Run.Show();
                this.Check_Program.Show();

                
             
            }
           

        }
        /******************************************************************************************************
         * Debug_Output() - Adds the string s to the debug output console and changes the color of the Telnet label
         * - Parameters: string s - the message sent to be handled
         * - Returns: None
         * ****************************************************************************************************/
        private void Debug_Output(string s)
        {
            if(s == "connected")
            {
                this.Label_Telnet.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
            }
            else if(s == "disconnected")
            {
                this.Label_Telnet.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
            }
            else if (s == ".")
            {
                this.console_debugOutput.AppendText(s);
            }
            else
            {
                this.console_debugOutput.AppendText("\n" + s);
            }
        }
        /******************************************************************************************************
         * Console_DebugOutput_TextChanged   
         * - Method to scroll the rich text box to the end after each text change
         * 
         * 
         * ****************************************************************************************************/
        private void Console_debugOutput_TextChanged(object sender, EventArgs e)
        {
            this.console_debugOutput.SelectionStart = this.console_debugOutput.Text.Length;
            this.console_debugOutput.ScrollToCaret();
        }
      
        /******************************************************************************************************
         * Check_LogToDatabase_CheckedChanged
         * - Event handler that allows user to change whether they want to log to the database during the session
         * 
         * ****************************************************************************************************/
        private void Check_LogToDatabase_CheckedChanged(object sender, EventArgs e)
        {
            if (Check_LogToDatabase.Checked)
            {
                this.LogData = true;
            }
            else
            {
                this.LogData = false;
            }
        }


        /******************************************************************************************************
         * Check_All
         * - Utility method to Check all of the check boxes
         * 
         * ****************************************************************************************************/
        private void Check_All()
        {
            
            this.Check_Program.Checked = true;

        }
        /******************************************************************************************************
         * Reset_GUI
         * - Utility method to reset the GUI to a start-up state. Should be called after a test has finished.
         * 
         * ****************************************************************************************************/
        private void Reset_GUI()
        {
            this.Check_All();

            //Hide buttons
            
            this.Button_Run.Hide();
            //Hide Checkboxes
            
            this.Check_Program.Hide();
            
            //Clear serial number box
            this.Field_SerialNumber.ResetText();
            this.Field_SerialNumber.Enabled = true;
            //Reset status bar
            this.StatusBar.Value = 0;

            //Reset Debug_Output
            this.console_debugOutput.ResetText();
            this.console_debugOutput.AppendText("Please enter a serial number");
        }
        /******************************************************************************************************
         * Button_GPIO_Click  
         * - Tries to reconnect to GPIO module if the module is not conencted on startup. When connected, this
         * button is disabled.
         * 
         * 
         * ****************************************************************************************************/
        private void Button_GPIO_Click(object sender, EventArgs e)
        {
            CheckDeviceConnections();
        }

    }

  
}
