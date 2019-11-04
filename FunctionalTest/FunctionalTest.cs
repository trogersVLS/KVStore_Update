using System;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Diagnostics;
using MccDaq;
using ErrorDefs;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Xml;



namespace KVStore_Update
{

    /******************************************************************************************************************************************
     *                                               Test Data Structure
     ******************************************************************************************************************************************/
    public struct TestData
    {
        public int step;                                // Step number in list of tests
        public string name;                             // Human readable test name
        public string method_name;                      // Method name in the Tests.cs file
        public MethodInfo testinfo;                     // Pointer to the method used to run the test
        public string result;                           // Result of the test
        public Dictionary<string,string> parameters;                    // Hashtable of parameters used in the test

        public TestData(int step, string name, string method_name, MethodInfo function, Dictionary<string,string> parameters)
        {
            this.step = step;
            this.name = name;
            this.method_name = method_name;
            this.testinfo = function;
            this.parameters = parameters;
            this.result = "n/a";

        }
    }
    /******************************************************************************************************************************************
     *                                               Functional Test Class
     ******************************************************************************************************************************************/
    public partial class FunctionalTest
    {
        //Test specific data --> To be stored in results file and in database
        private string serial;             //Test serial number
        private string location;
        private int eqid;
        private int user_id;
        private DateTime date;
        private DateTime time;


        private MccDaq_GPIO GPIO;
        private Test_Equip DMM;
        private Test_Equip PPS;
        private Programmer SOM;
        private VLS_Tlm TLM;

        public List<TestData> Tests = new List<TestData>();
        private readonly ConcurrentQueue<string> Rx_Queue;

        private bool cancel_request = false;
        private bool log_data;



        /************************************************************************************************************
         * Functional Test Class Constructor
         * 
         * Parameters: - ConcurrentQueue<string> _queue--> The message passing data structure used between the calling object (presumably GUI)
         *             - XmlNode tests --> An xml data type containing the tests required and the specs to pass
         *             - String serial--> The serial number of the board in which this test has been initialized
         * 
         * **********************************************************************************************************/
        public FunctionalTest()
        {
            //This constructor is used for collecting methods from this data type
        }
         public FunctionalTest(ConcurrentQueue<string> _rx)
        {   
            //This constructor is used for development
            this.log_data = false;
            //Create the queue used for passing messages between threads
            this.Rx_Queue = _rx;
        }
        public FunctionalTest(ConcurrentQueue<string> _rx, Hashtable Parameters)
        {
            //This constructor is used for production
            this.log_data = true;

            try
            {
                this.GPIO = (MccDaq_GPIO)Parameters["gpio"];
                this.SOM = (Programmer)Parameters["som"];
            }
            catch
            {

            }

            //Create the queue used for passing messages between threads
            this.Rx_Queue = _rx;


        }
        /************************************************************************************************************
         * Functional Test Class Destructor
         * 
         * - Disconnects from the GPIO module, Power Supply, and Multimeter so that the next test can use the resources
         * 
         * **********************************************************************************************************/
        ~FunctionalTest()
        {
            //TODO: Add the destructor tasks


            //Destructor is obsolete after moving resource management to the GUI thread
        }

        /******************************************************************************************************************************************
         *                  MESSAGE PASSING UTILITIES
         ******************************************************************************************************************************************/

        /************************************************************************************************************
         * ClearInput
         * 
         * Function: Clears the message buffer between the main thread and the test thread. 
         * 
         * Arguments: None
         * 
         * Returns: None
         * 
         * **********************************************************************************************************/
        private void ClearInput()
        {
            string message;
            while (!this.Rx_Queue.IsEmpty)
            {
                //Queue is not empty. Pending inputs or a pending cancel. Need to check to see if it is a cancel
                this.Rx_Queue.TryPeek(out message);
                if (message != "cancel")
                {
                    //If message is not a cancel, remove the message. Don't care if it's there 
                    this.Rx_Queue.TryDequeue(out message);
                }
                else
                {
                    //Message is a cancel, need to exit function so that the program can read the message
                    this.cancel_request = true;
                    break;
                }

            }
            return;
        }
        /************************************************************************************************************
         * ReceiveInput
         * 
         * Function: Pops a message from the queue or waits until the queue has received a message.
         * 
         * Arguments: None
         * 
         * Returns: string message - message popped from the queue.
         * 
         * **********************************************************************************************************/
        private string ReceiveInput()
        {
            string message;

            while (this.Rx_Queue.IsEmpty)
            {
                //Do nothing, block until a message is received.
            }
            this.Rx_Queue.TryDequeue(out message);

            return message;

        }


        /******************************************************************************************************************************************
        *                                               TEST RUNNING FUNCTIONS
        ******************************************************************************************************************************************/

        /************************************************************************************************************
         * RunTest() - Runs the list of tests determined by the functional test
         * 
         * Parameters: - progress --> Progress interface variable. Indicates the percentage of the test that is complete
         *             - message  --> Progress interface variable. Used to update the text in the output box.  
         *             - TestList --> List of TestStep. Used to tell RunTest which tests need to be run.
         * **********************************************************************************************************/
        public void RunTest(IProgress<int> progress, IProgress<string> message)
        {
            
            string str;

            //Power up the control board
            this.GPIO.setPort(DigitalPortType.FirstPortA, 0xFF);

            //Wait until we are able to connect to the device
            bool timeout = false;
            int timeout_millis = 20000; //20 seconds
            DateTime start = DateTime.Now;
            DateTime end = start.AddMilliseconds(timeout_millis);
            //Thread.Sleep(1000);

            this.TLM = new VLS_Tlm("10.10.0.5");
            List<string> data = this.TLM.Command_QNX("ls");

            





            while (!timeout)
            {
                for(int i = 5; i<255; i++)
                {
                    string ip = "10.10.0." + i.ToString();
                    if (PingHost(ip)) break;
                }
            }
            //Ip address equals the ip we pinged


                
            return;
        }

          

        public bool PingHost(string nameOrAddress)
            {
                bool pingable = false;
                Ping pinger = null;

                try
                {
                    pinger = new Ping();
                    PingReply reply = pinger.Send(nameOrAddress);
                    pingable = reply.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    // Discard PingExceptions and return false;
                }
                finally
                {
                    if (pinger != null)
                    {
                        pinger.Dispose();
                    }
                }

                return pingable;
            }
    /************************************************************************************************************
     * Program() - Runs the programming section only
     * 
     * Parameters: - progress --> Progress interface variable. Indicates the percentage of the test that is complete
     *             - message  --> Progress interface variable. Used to update the text in the output box.  
     *             
     * **********************************************************************************************************/


    /******************************************************************************************************************************************
     *                                               DATA LOGGING FUNCTIONS
     ******************************************************************************************************************************************/

    /******************************************************************************************************************************************
     * SQLite tables
     *  - Control_Board_Test
     *      - An entry in the control board Test Table is posted every time an instance of the functional test class is created. This holds
     *      a unique test_id value that is used to group multiple tests that are run at the same time by one press of the "Run" button in 
     *      the GUI. A control board test table looks like. The unique id will be shared by a secondary table
     *      _______________________________________________________________________________
     *      |                             Control Board Test                               |
     *      --------------------------------------------------------------------------------
     *      | Test_ID   | EQID     | USER_ID  | LOCATION | TIMESTAMP | SERIAL   | RESULT   |
     *      --------------------------------------------------------------------------------
     *      | <string>  | <string> | <string> | <string> | <string>  | <string> | <string> |
     *      |    ...    |    ...   |    ...   |    ...   |    ...    |    ...   |    ...   |
     *      |    ...    |    ...   |    ...   |    ...   |    ...    |    ...   |    ...   |
     *      
     *      
     * - Test_Table
     *      - An entry in the test table indicates an instance that a test was run on the board and the result of that specific test. This
     *      will store the test name, serial number, parameters that the test was run against and the measured result of the test and the 
     *      ultimate result of the test. The test id is unique to a set of tests, but no two entries in the table should be the same.
     *      _________________________________________________________________________________
     *      |                              Tests                                             |
     *      ----------------------------------------------------------------------------------
     *      | Test_ID   | SERIAL   | TEST_NAME| UpperBound | LowerBound | Measured | RESULT  |
     *      ----------------------------------------------------------------------------------
     *      | <string>  | <string> | <string> | <real>     | <real>     | <real>   | <string>|
     *      |    ...    |    ...   |    ...   |    ...     |    ...     |    ...   |    ...  |
     *      |    ...    |    ...   |    ...   |    ...     |    ...     |    ...   |    ...  |
     *         
     */

    private bool DatabaseExist()
        {


            return true;
        }
        private bool LogData(Hashtable table)
        {
            return true;
        }

    }
}
