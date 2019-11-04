using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using System.Windows.Forms;



namespace KVStore_Update
{   
    enum ProgrammerType
    {
        HERCULES = 1,
        CPLD = 2,
        SOM = 3,
    }

    
    class Programmer
    {
        SerialPort som_programmer;
        public bool Connected = false;
        ProgrammerType target;
        public Programmer(ProgrammerType target, string comport = null)
        {
            this.target = target;
            if (target != ProgrammerType.SOM)
            {
                ConnectProgrammer();
            }
            else
            {
                ConnectSOM(comport);
            }
        }

        public bool ConnectProgrammer()
        {
            string cpld_search = "Select * from Win32_USBHub";
            string cpld_name = "FlashPro";
            string herc_search = "Select * from Win32_SerialPort";
            string herc_name = "XDS2xx";
            string som_search = "Select * from Win32_SerialPort";
            string som_name = "USB";


            string name = "";
            string search = "";
            if(this.target == ProgrammerType.HERCULES)
            {
                name = herc_name;
                search = herc_search;
            }
            else if(this.target == ProgrammerType.CPLD)
            {
                name = cpld_name;
                search = cpld_search;
            }
            else if(this.target == ProgrammerType.SOM)
            {
                name = som_name;
                search = som_search;
            }

            

            ManagementObjectCollection ManObjReturn;
            ManagementObjectSearcher ManObjSearch;
            ManObjSearch = new ManagementObjectSearcher(search);
            ManObjReturn = ManObjSearch.Get();
            List<string> names = new List<string>();
            

            foreach (ManagementObject ManObj in ManObjReturn)
            {
                names.Add(ManObj["Name"].ToString());

            }
            ManObjReturn.Dispose();
            ManObjSearch.Dispose();
            foreach (string str in names)
            {
                if (str.Contains(name))
                {
                    this.Connected = true;
                }
            }

            return this.Connected;
        }

        public bool ConnectSOM(string comport)
        {
            try
            {
                this.som_programmer = new SerialPort(comport, 115200, System.IO.Ports.Parity.None, 8);
                this.som_programmer.Open();
            }
            catch
            {

            }
            if (this.som_programmer.IsOpen)
            {
                this.Connected = true;
            }
            
            return true;
        }

        //TODO: ADD SOM specific communication methods, (e.g. Command, Query, etc., )
    }
}
