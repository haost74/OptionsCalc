using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OptionsCalc
{
    public partial class Form1 : Form
    {
        List<Entities.Instrument> instruments = new List<Entities.Instrument>();
        List<Entities.Portfolio> portfolios = new List<Entities.Portfolio>();

        int indx = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PortfoliosDG.AutoGenerateColumns = true;
            PortfoliosDG.DataSource = portfoliosBS;
            PortfoliosDG.DefaultCellStyle.NullValue = "";

            portfoliosBS.DataSource = instruments;
            PortfoliosDG.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            foreach(Entities.Portfolio p in portfolios)
            {
                p.Refresh();

            }
        }

        private void btnAddInstrumetn_Click(object sender, EventArgs e)
        {
            Entities.Instrument i = new Entities.Instrument(indx++,"UX"+indx.ToString(),"FUTUX",indx%2==1?Entities.InstrumentType.Futures:Entities.InstrumentType.Option,"another full name");
            instruments.Add(i);
            PortfoliosDG.Refresh();
        }

        


       
    }
}
