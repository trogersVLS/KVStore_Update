using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using PrimS.Telnet;



namespace KVStore_Update
{
    public class VLS_Tlm
    {
        int cmd_port = 5000;
        int qnx_port = 23;
        int tlm_port = 5001;

        TcpClient vocsn_cmd;
        NetworkStream stream_cmd;
        TcpClient vocsn_qnx;
        NetworkStream stream_qnx;
        TcpClient vocsn_tlm;
        NetworkStream stream_tlm;

        
        bool Connected;
        string _ip_address;
        public VLS_Tlm(string _ip_address)
        {
            this._ip_address = _ip_address;
            try
            {
                //this.Connect_CMD(this._ip_address);
                this.Connect_QNX(this._ip_address);
            }
            catch
            {
                this.Connected = false;
            }
        }
        ~VLS_Tlm()
        {
            this.Close();
        }
        /*read_until:
         * Reads from the telnet stream until the specified string.
         * 
         */
        private String read_until(string str)
        {
            String response = "";
            int tempByte;

            if (this.Connected)
            {
                while (true)
                {
                    tempByte = this.stream_cmd.ReadByte();
                    if (tempByte != (-1))
                    {

                        response += (char)tempByte;
                    }
                    if (response.Contains(str))
                    {
                        break;
                    }

                }
            }
            else
            {
                response = null;
            }
            return response;

        }
        /****************************************************************
         * Command
         * Sends a command over port 5000;
         * 
         * **************************************************************/
        public List<String> Command(string message)
        {
            List<String> responseData = new List<String>();
            Byte[] command = System.Text.Encoding.ASCII.GetBytes(message);
            if (message == "exit")
            {
                this.stream_cmd.Write(command, 0, command.Length); //Send the command
                responseData.Add("Successful Exit");
            }
            else
            {
                this.stream_cmd.Write(command, 0, command.Length); //Send the command
                responseData.AddRange(this.read_until("$vserver>").Split(new string[] { "\r","\n" }, StringSplitOptions.None)); // Wait and receive the response.
                responseData.ForEach(i => i.Trim());
            }
            return responseData;
        }



        /* Connect
         * Connects to the telnet port at the specificied ip address
         */
        private bool Connect_CMD(String _ip_address)
        {
            string responseString;
            Byte[] response = new Byte[256];
            int bytes;
            try
            {
                // Create a TcpClient connection to VOCSN
                this.vocsn_cmd = new TcpClient(_ip_address, this.cmd_port);

                //Get the VOCSN network stream.
                this.stream_cmd = this.vocsn_cmd.GetStream();
                bytes = this.stream_cmd.Read(response, 0, response.Length);
                responseString = System.Text.Encoding.ASCII.GetString(response, 0, bytes);


                if (bytes == 0 || responseString != "$vserver> ")
                {
                    Console.WriteLine("Unable to connect");
                    this.Connected = false;
                }
                else
                {
                    this.Connected = true;
                }
            }
            catch 
            {
            }

            return true;
        }
        private bool Connect_QNX(string _ip_address)
        {
            
            return true;
        }
        public List<String> Command_QNX(string message)
        {
            List<String> responseData = new List<String>();
            Byte[] command = System.Text.Encoding.ASCII.GetBytes(message);
            if (message == "exit")
            {
                this.stream_cmd.Write(command, 0, command.Length); //Send the command
                responseData.Add("Successful Exit");
            }
            else
            {
                this.stream_cmd.Write(command, 0, command.Length); //Send the command
                responseData.AddRange(this.read_until("#").Split(new string[] { "\r", "\n" }, StringSplitOptions.None)); // Wait and receive the response.
                responseData.ForEach(i => i.Trim());
            }
            return responseData;
        }



        private void Close()
        {
            try
            {
                this.Command("exit");
                this.Command_QNX("exit");
                this.vocsn_cmd.Close();
                this.Connected = false;
            }
            catch 
            {

            }
        }
    }
}
