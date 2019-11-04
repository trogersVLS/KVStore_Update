using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
namespace KVStore_Update
{
    public partial class LoginForm : Form
    {
        XmlNode users;
        string username;
        public LoginForm()
        {

            
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string user = this.Field_User.Text;
            string pass = this.Field_Pass.Text;

            foreach(XmlNode x in this.users)
            {   
                if(x.Attributes[0].InnerText == user)
                {
                    if(x.Attributes[1].InnerText == pass)
                    {
                        this.username = user;
                        this.Close();
                        break;
                       
                    }
                }

                 
             
            }
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {

        }

        public string ShowForm(XmlNode users)
        {
            this.users = users;
            this.ShowDialog();
            

            return this.username;
        }


    }


}
