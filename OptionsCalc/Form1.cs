using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Timer=System.Windows.Forms.Timer;

namespace OptionsCalc
{
    public partial class Form1 : Form
    {
        BindingList<Entities.Instrument> instruments ;
        BindingList<Entities.Portfolio> portfolios ;
        TestData data;
        private Timer updateTimer = new Timer() { Interval=1000 };

        public Form1()
        {
            InitializeComponent();
            PortfoliosDG.DefaultCellStyle.NullValue = "";
            PortfoliosDG.AutoGenerateColumns = true;
            PortfoliosDG.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            instrumentsDG.DefaultCellStyle.NullValue = "";
            instrumentsDG.AutoGenerateColumns = true;
            instrumentsDG.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            data=new TestData();
            data.Connect();
            instruments = new BindingList<Entities.Instrument>(data.Instruments);
            portfolios = new BindingList<Entities.Portfolio>(data.Portfolios);
            data.OnUpdate += new EventHandler(data_OnUpdate);
        }

        void data_OnUpdate(object sender, EventArgs e)
        {
            lblRefreshed.Text = DateTime.Now.ToString() + " : refreshed";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            portfoliosBS.DataSource = portfolios;
            PortfoliosDG.DataSource = portfoliosBS;
            instrumentsBS.DataSource = instruments;
            instrumentsDG.DataSource = instrumentsBS;
            updateTimer.Tick += (o, args) => data.updateData();
            if (!updateTimer.Enabled)
                updateTimer.Start();
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            foreach (Entities.Portfolio p in portfolios)
            {
                p.Refresh();
            }
        }

        


       
    }
}
