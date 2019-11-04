using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

namespace KVStore_Update
{
    class Test_Equip
    {
        private readonly int QUERY_DELAY = 500; //millisecond delay for SCPI queries
        string name;
        string comm;
        SerialPort Device;
        string address;
        string ID;
        
        

        public bool Connected = false;
        /* Test_Equip Constructor: 
         * Initializes a Test Equipment SCPI compatible device over RS232, GPIB, LAN, or USB
         */
        public Test_Equip(string name, string ID, string comm, string address)
        {

            this.name = name;
            this.ID = ID;
            this.address = address;
            this.comm = comm;

            if (comm == "RS232")
            {
                this.comm = comm;
                this.Device = new SerialPort(this.address, 230400, Parity.None, 8, StopBits.One);
                this.Device.RtsEnable = true;
                this.Device.DtrEnable = true;
                this.Device.ReadTimeout = 100;
                this.ConnectDevice();
            }
            else if (comm == "GPIB")
            {
                //TODO: GPIB constructor
            }
            else if (comm == "USB")
            {
                //TODO: USB constructor
            }
            else if (comm == "LAN")
            {                
                //TODO: LAN constructor
            }
            
        }
        ~Test_Equip()
        {
            try
            {
                this.Device.Write("SYST:LOC\r");

                this.Device.Close();
                
            }
            catch
            {

            }
        }
        
        public bool ConnectDevice()
        {
            if (this.comm == "RS232")
            {
                try
                {
                    this.Device.Open();
                    string identity = this.Query("*IDN?");
                    if (identity.Contains(this.ID)) this.Connected = true;
                    else this.Connected = false;
                    if(this.Connected) this.Device.Write("SYST:REM\r");
                }
                catch
                {
                    this.Connected = false;
                    this.Device.Close();
                }
                
                

                
            }
            else if (this.comm == "GPIB")
            {
                //TODO: GPIB constructor
            }
            else if (this.comm == "USB")
            {
                //TODO: USB constructor
            }
            else if (this.comm == "LAN")
            {
                //TODO: LAN constructor
            }

            return this.Connected;
        }

        public float Get_Volts()
        {
            string volt_str = "";
            float volts;

            volt_str = this.Query(":MEAS:VOLT:DC?");
            try
            {
                volts = float.Parse(volt_str, System.Globalization.NumberStyles.Float);
            }
            catch
            {
                volts = 0;
            }

            return volts;
        }
        public float Get_Amps()
        {
            string amp_str = "";
            float amps;

            amp_str = this.Query(":MEAS:CURR:DC?");
            try
            {
                amps = float.Parse(amp_str, System.Globalization.NumberStyles.Float);
            }
            catch
            {
                amps = 0;
            }

            return amps;
        }

        private string Query(string cmd)
        {
            string response = "";
            byte[] byte_response = new byte[64];
            if(this.comm == "RS232")
            {
                this.Device.Write(cmd +"\r");
                Thread.Sleep(this.QUERY_DELAY);
                try
                {

                    int num = this.Device.Read(byte_response, 0, byte_response.Length);
                }
                catch
                {

                }
                response = Encoding.ASCII.GetString(byte_response, 0, byte_response.Length);

            }
            

            return response;
        }
    }
}
