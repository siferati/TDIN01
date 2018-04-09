using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
namespace Client
{
    public partial class MainPage : Form
    {

        private Client client;

        public MainPage(Client client)
        {
            InitializeComponent();
            this.panelMyWallet.Visible = true;
            this.panelSellingOrders.Visible = false;
            this.panelPurchaseOrders.Visible = false;

            this.panel3.Visible = true;
            this.panel4.Visible = false;
            this.panel5.Visible = false;

            this.client = client;
            
            

            /* Test */
            add_transactions("sold", "1000", "1.2", "12-06-10");
            updateBalance("" + 2000);
            updateQuote("" + 2.0);
            updateDiginotesOwned("" + 200);
            //updateFullName(client.Name());
        }

        private void updateFullName(string name)
        {
            this.labelFullName.Text = name;
        }

        private void updateDiginotesOwned(string no)
        {
            this.labelDigisOwned.Text = no;
        }

        private void updateQuote(string quote)
        {
            this.labelQuote.Text = quote;
        }

        private void updateBalance(string balance)
        {
            this.Balance.Text = balance;
        }

        private void add_transactions(string type, string quantity, string quote, string date)
        {

            ListViewItem lvi = new ListViewItem(date);
            lvi.SubItems.Add(type);
            lvi.SubItems.Add(quantity);
            lvi.SubItems.Add(quote);
            this.History.Items.Add(lvi);

        }


        private void button3_Click(object sender, EventArgs e)
        {
            client.Logout();
            Application.Exit();
        }

        private void buttonOverview_Click(object sender, EventArgs e)
        {
            this.panelMyWallet.Visible = true;
            this.panelSellingOrders.Visible = false;
            this.panelPurchaseOrders.Visible = false;

            this.panel3.Visible = true;
            this.panel4.Visible = false;
            this.panel5.Visible = false;

        }

        private void buttonSellingOrder_Click(object sender, EventArgs e)
        {
            this.panelMyWallet.Visible = false;
            this.panelSellingOrders.Visible = true;
            this.panelPurchaseOrders.Visible = false;

            this.panel3.Visible = false;
            this.panel4.Visible = true;
            this.panel5.Visible = false;

        }

        private void buttonPurchaseOrder_Click(object sender, EventArgs e)
        {
            this.panelMyWallet.Visible = false;
            this.panelSellingOrders.Visible = false;
            this.panelPurchaseOrders.Visible = true;

            this.panel3.Visible = false;
            this.panel4.Visible = false;
            this.panel5.Visible = true;
        }
    }
}
