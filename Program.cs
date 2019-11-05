/* Program.cs
 * - Main entry point of program
 * 
 * Author: Taylor Rogers
 * Date: 10/23/2019
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KVStore_Update
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



            var users = GetSettings();
            //Show Login screen
            var login = new LoginForm(users);

            string user = login.ShowForm();
            if(user != null)
            {
                if(user == "")
                {
                    user = "TEST";
                }
                Application.Run(new GUI_Main(user));
            }

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
        private static XmlNode GetSettings()
        {
            //open the settings.xml file

            XmlDocument configuration = new XmlDocument();
            XmlNode user_ids = null;


            configuration.Load(@"..\..\Configuration\settings.xml");


            foreach (XmlNode xml in configuration.DocumentElement.ChildNodes)
            {

                if (xml.Name == "users")
                {
                    user_ids = xml;
                }

            }

            return user_ids;

        }
    }
}
