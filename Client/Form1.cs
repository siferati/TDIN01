using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {


        private bool login = false;
        private string username = "";
        private string nickname = "";
        private string password = "";
        private string password2 = "";
        private MainPage mainPage = null;
        private Client client;


        public Form1(Client client)
        {

            InitializeComponent();
            this.panelLogin.Visible = false;
            this.panelInErrorMsg.Visible = false;
            this.panelUpErrorMsg.Visible = false;

            mainPage = new MainPage(client);
            this.client = client;    
        }



        private void label9_Click(object sender, EventArgs e)
        {
            client.Logout();
            Application.Exit();
        }



        private void button3_Click(object sender, EventArgs e)  // sign up - sign up
        {
            this.panelUpErrorMsg.Visible = false;
            this.panelInErrorMsg.Visible = false;

            if (username.Length >= 5 && nickname.Length >= 5 && password.Equals(password2) && client.Register(this.username, this.nickname, this.password))
            {
                client.Login(this.nickname, this.password);
                mainPage.inicialize_wallet();
                this.Hide();
                mainPage.Show();
            }
            else
            {
                this.panelUpErrorMsg.Visible = true;
            }

        }

        private void button1_Click_1(object sender, EventArgs e)    // sign up - login
        {
            this.transition();
        }


        public void transition()
        {
            this.login = this.login ? false : true;
            if (this.login)
            {
                this.panelRight.Visible = false;
                this.panelLogin.Visible = true;
                this.panelLogin.Update();

            }
            else
            {
                this.panelLogin.Visible = false;
                this.panelRight.Visible = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)      // Login - Login
        {
            this.panelUpErrorMsg.Visible = false;
            this.panelInErrorMsg.Visible = false;
            if (!this.nickname.Equals("") && client.Login(this.nickname, this.password))
            {
                mainPage.inicialize_wallet();
                this.Hide();
                mainPage.Show();
            }
            else
            {
               this.panelInErrorMsg.Visible = true;
            }
             
        }

        private void button2_Click(object sender, EventArgs e)  // Login - Register
        {
            this.transition();
        }

        private void bunifuMaterialTextbox4_OnValueChanged(object sender, EventArgs e)
        {
            this.password2 = this.bunifuMaterialTextbox4.Text;
        }

        private void bunifuMaterialTextbox3_OnValueChanged(object sender, EventArgs e)
        {
            this.password = this.bunifuMaterialTextbox3.Text;
        }

        private void bunifuMaterialTextbox1_OnValueChanged(object sender, EventArgs e)
        {
            this.username = this.bunifuMaterialTextbox1.Text;

        }

        private void bunifuMaterialTextbox2_OnValueChanged(object sender, EventArgs e)
        {
            this.nickname = this.bunifuMaterialTextbox2.Text;
            this.bunifuMaterialTextbox8.Text = this.bunifuMaterialTextbox2.Text;
        }
        
        private void bunifuMaterialTextbox8_OnValueChanged_1(object sender, EventArgs e)
        {
            this.nickname = this.bunifuMaterialTextbox8.Text;
            this.bunifuMaterialTextbox2.Text = this.nickname;
        }

        private void bunifuMaterialTextbox7_OnValueChanged_1(object sender, EventArgs e)
        {
            this.password = this.bunifuMaterialTextbox7.Text;
        }
    }
}
