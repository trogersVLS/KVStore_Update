using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using MccDaq;

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
        public Dictionary<string, string> parameters;                    // Hashtable of parameters used in the test

        public TestData(int step, string name, string method_name, MethodInfo function, Dictionary<string, string> parameters)
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
        private MccDaq_GPIO GPIO;
        private VLS_Tlm TLM;

        public List<TestData> Tests = new List<TestData>();
        private readonly ConcurrentQueue<string> Rx_Queue;
        public Dictionary<string, string> results;

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
            //The result format for this program is as follows
            //  | Board Serial | Date |  User | Command 1 | Command 2 | Command 3 | Command 4 | 
            this.results = new Dictionary<string, string>();
            try
            {
                this.GPIO = (MccDaq_GPIO)Parameters["gpio"];
                this.results.Add("serial", (string)Parameters["serial"]);

            }
            catch
            {

            }

            //Create the queue used for passing messages between threads
            this.Rx_Queue = _rx;

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



            //Power up the control board, this may take a long time. Add a 30s delay, typically takes ~20s
            this.GPIO.setPort(DigitalPortType.FirstPortA, 0xFF);
            message.Report("Waiting for device to boot");
            string msg = ".";
            for (int i = 0; i < 22; i++)
            {
                Thread.Sleep(1000);
                message.Report(msg);

            }


            //DHCP Server IP pool is 10.255.255.1- 255
            //Each device connected will take another ip address, need to check to see which IP adress it is. The addresses expire after a 30 minute lease

            var watch = System.Diagnostics.Stopwatch.StartNew(); // Timing how long the program takes to run

            string ip_address_base = "10.255.255.";
            int end_address = 1;
            while (this.Rx_Queue.IsEmpty && (end_address < 100))
            {
                try
                {
                    message.Report("Trying at " + ip_address_base + end_address.ToString());
                    this.TLM = new VLS_Tlm(ip_address_base + end_address.ToString());

                    if (this.TLM.QNX_IsConnected())
                    {
                        message.Report("connected");
                        message.Report("Connected to device at: " + ip_address_base + end_address.ToString());
                        break;
                    }
                    message.Report("Could not connect");
                }
                catch
                {

                }
                end_address++;
            }
            if (end_address >= 100)
            {
                message.Report("Device is not connected, please check to see if the ethernet cable to connected properly.");
                return;
            }
            else
            {



                if (this.TLM.QNX_IsConnected())
                {
                    bool success = true;
                    //Command 1
                    this.TLM.QNX_WriteLine("/opt/vls/kvstore /fs/etfs/config/uim/datastore/deviceconfig update blower.ctrl 1");
                    var result = this.TLM.QNX_Read();
                    if (result.Contains("exited status=0"))
                    {
                        message.Report("Command 1: Pass");
                        this.results.Add("cmd_1", "PASS");

                    }
                    else
                    {
                        message.Report("Command 1: Fail");
                        this.results.Add("cmd_1", "FAIL");
                        success = false;
                    }


                    //Command 2
                    this.TLM.QNX_WriteLine("/opt/vls/kvstore /fs/sd0/config/uim/datastore/deviceconfig update blower.ctrl 1");
                    result = this.TLM.QNX_Read();
                    if (result.Contains("exited status=0"))
                    {
                        message.Report("Command 2: Pass");
                        this.results.Add("cmd_2", "PASS");

                    }
                    else
                    {
                        message.Report("Command 2: Fail");
                        this.results.Add("cmd_2", "FAIL");
                        success = false;
                    }

                    //Command 3
                    this.TLM.QNX_WriteLine("/opt/vls/kvstore /fs/etfs/config/uim/datastore/deviceconfig list");
                    result = this.TLM.QNX_Read();
                    if (result.Contains("blower.ctrl\t1"))
                    {
                        message.Report("Command 3: Pass");
                        this.results.Add("cmd_3", "PASS");
                    }
                    else
                    {
                        message.Report("Command 3: Fail");
                        this.results.Add("cmd_3", "FAIL");
                        success = false;
                    }

                    //Command 4
                    this.TLM.QNX_WriteLine("/opt/vls/kvstore /fs/sd0/config/uim/datastore/deviceconfig list");
                    result = this.TLM.QNX_Read();
                    if (result.Contains("blower.ctrl\t1"))
                    {
                        message.Report("Command 4: Pass");
                        this.results.Add("cmd_4", "PASS");
                    }
                    else
                    {
                        message.Report("Command 4: Fail");
                        this.results.Add("cmd_4", "FAIL");
                        success = false;
                    }
                    if (success) this.results.Add("result", "PASS");
                    else this.results.Add("result", "FAIL");


                    //Disconnect from Telnet and turn device off.
                    this.TLM.Disconnect();

                    this.GPIO.setPort(DigitalPortType.FirstPortA, 0x00);

                    message.Report("disconnected");
                    //message.Report("Elapsed time = " + watch.ElapsedMilliseconds.ToString());
                }
            }
            return;
        }
    }
}
