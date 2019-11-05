using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;




namespace KVStore_Update
{
    enum Verbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    enum Options
    {
        SGA = 3
    }
    public class VLS_Tlm
    {
        int cmd_port = 5000;
        int qnx_port = 23;
        int tlm_port = 5001;

        TcpClient cmd_shell;
        TcpClient qnx_shell;
        TcpClient tlm_shell;

        bool Connected = false;

        int TimeOutMs = 100;

        string _ip_address;


        public VLS_Tlm(string _ip_address)
        {
            this._ip_address = _ip_address;
            try
            {

                qnx_shell = new TcpClient();
                var result = qnx_shell.BeginConnect(_ip_address, qnx_port, null, null);

                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(0.1));

                if (success)
                {
                    this.QNX_Login("root", "", 500);
                }

            }
            catch
            {
                throw new Exception();
            }
        }

        ~VLS_Tlm()
        {
            try
            {
                this.Disconnect();
            }
            catch { 
            }
        }

        public string QNX_Login(string Username, string Password, int LoginTimeOutMs)
        {
            int oldTimeOutMs = TimeOutMs;
            TimeOutMs = LoginTimeOutMs;
            string s = QNX_Read();
            s += QNX_Read();
            if (!s.TrimEnd().EndsWith(":"))
                throw new Exception("Failed to connect : no login prompt");
            QNX_WriteLine(Username);

            s += QNX_Read();
            if (!s.TrimEnd().EndsWith("#"))
                throw new Exception("Failed to connect : no password prompt");
            else
            {
                this.Connected = true;
            }


            s += QNX_Read();
            TimeOutMs = oldTimeOutMs;
            return s;
        }

        public void QNX_WriteLine(string cmd)
        {
            QNX_Write(cmd + "\n");
        }

        public void QNX_Write(string cmd)
        {
            if (!qnx_shell.Connected) return;
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            qnx_shell.GetStream().Write(buf, 0, buf.Length);
        }

        public string QNX_Read()
            {
            if (!qnx_shell.Connected) return null;
            StringBuilder sb = new StringBuilder();
            do
            {
                QNX_ParseTelnet(sb);
                System.Threading.Thread.Sleep(TimeOutMs);
            } while (qnx_shell.Available > 0);
            string str = sb.ToString();
            return str;
        }

        public bool QNX_IsConnected()
        {
            return qnx_shell.Connected;
        }

        void QNX_ParseTelnet(StringBuilder sb)
        {
            while (qnx_shell.Available > 0)
            {
                int input = qnx_shell.GetStream().ReadByte();
                switch (input)
                {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = qnx_shell.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = qnx_shell.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                qnx_shell.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    qnx_shell.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    qnx_shell.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                qnx_shell.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }


        public void Disconnect()
        {
            this.qnx_shell.Close();
        }
        /*read_until:
         * Reads from the telnet stream until the specified string.
         * 
         */
        //private String read_until(string str)
        //{
        //    String response = "";
        //    int tempByte;

        //    if (this.Connected)
        //    {
        //        while (true)
        //        {
        //            tempByte = this.cmd_shell.ReadByte();
        //            if (tempByte != (-1))
        //            {

        //                response += (char)tempByte;
        //            }
        //            if (response.Contains(str))
        //            {
        //                break;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        response = null;
        //    }
        //    return response;

        //}
        /****************************************************************
         * Command
         * Sends a command over port 5000;
         * 
         * **************************************************************/
        //public List<String> Command(string message)
        //{
        //    List<String> responseData = new List<String>();
        //    Byte[] command = System.Text.Encoding.ASCII.GetBytes(message);
        //    if (message == "exit")
        //    {
        //        this.stream_cmd.Write(command, 0, command.Length); //Send the command
        //        responseData.Add("Successful Exit");
        //    }
        //    else
        //    {
        //        this.stream_cmd.Write(command, 0, command.Length); //Send the command
        //        responseData.AddRange(this.read_until("$vserver>").Split(new string[] { "\r","\n" }, StringSplitOptions.None)); // Wait and receive the response.
        //        responseData.ForEach(i => i.Trim());
        //    }
        //    return responseData;
        //}



        ///* Connect
        // * Connects to the telnet port at the specificied ip address
        // */
        //private bool Connect_CMD(String _ip_address)
        //{
        //    string responseString;
        //    Byte[] response = new Byte[256];
        //    int bytes;
        //    try
        //    {
        //        // Create a TcpClient connection to VOCSN
        //        this.cmd_shell = new TcpClient(_ip_address, this.cmd_port);

        //        //Get the VOCSN network stream.
        //        this.stream_cmd = this.vocsn_cmd.GetStream();
        //        bytes = this.stream_cmd.Read(response, 0, response.Length);
        //        responseString = System.Text.Encoding.ASCII.GetString(response, 0, bytes);


        //        if (bytes == 0 || responseString != "$vserver> ")
        //        {
        //            Console.WriteLine("Unable to connect");
        //            this.Connected = false;
        //        }
        //        else
        //        {
        //            this.Connected = true;
        //        }
        //    }
        //    catch 
        //    {
        //    }

        //    return true;
        //}
        //private bool Connect_QNX(string _ip_address)
        //{
            
        //    return true;
        //}
        //public List<String> Command_QNX(string message)
        //{
        //    List<String> responseData = new List<String>();
        //    Byte[] command = System.Text.Encoding.ASCII.GetBytes(message);
        //    if (message == "exit")
        //    {
        //        this.stream_cmd.Write(command, 0, command.Length); //Send the command
        //        responseData.Add("Successful Exit");
        //    }
        //    else
        //    {
        //        this.stream_cmd.Write(command, 0, command.Length); //Send the command
        //        responseData.AddRange(this.read_until("#").Split(new string[] { "\r", "\n" }, StringSplitOptions.None)); // Wait and receive the response.
        //        responseData.ForEach(i => i.Trim());
        //    }
        //    return responseData;
        //}



        //private void Close()
        //{
        //    try
        //    {
        //        this.Command("exit");
        //        this.Command_QNX("exit");
        //        this.vocsn_cmd.Close();
        //        this.Connected = false;
        //    }
        //    catch 
        //    {

        //    }
        //}
    }
}
