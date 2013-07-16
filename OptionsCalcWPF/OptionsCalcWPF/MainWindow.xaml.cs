using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using Common;
using QuikConnectionManager;


namespace OptionsCalcWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<Entities.Portfolio> lstPortfolios;
        BindingList<Entities.Instrument> lstInstruments;
        Thread dataThread;

        public MainWindow()
        {
            InitializeComponent();

        }



        private void ConnectionManager_OnConnected(string obj)
        {
            textBoxConnectionStatus.Text = "Connected";
        }

        private void OptionsCalculator_Closed(object sender, EventArgs e)
        {
            ConnectionManager.Disconect();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxConnectionStatus.Text == "Disconnected")
            {
                dataThread = new Thread(ConnectionManager.Connect);
                dataThread.Start();
            }
                //ConnectionManager.Connect();
            if (textBoxConnectionStatus.Text == "Connected")
                ConnectionManager.Disconect();
        }

        private void OptionsCalculator_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataManager.Init();
                lstPortfolios = new BindingList<Entities.Portfolio>(DataManager.Portfolios);
                lstInstruments = new BindingList<Entities.Instrument>(DataManager.Instruments);

                ConnectionManager.OnAccount += DataManager.UpdateAccount;
                ConnectionManager.OnNewInstrument += DataManager.AddInstrument;
                ConnectionManager.OnInstrument += DataManager.UpdateInstrument;
                ConnectionManager.OnPosition += DataManager.UpdatePosition;
                ConnectionManager.OnConnected += new Action<string>(ConnectionManager_OnConnected);

                dgrPortfolios.ItemsSource = lstPortfolios;
                dgrODesk.ItemsSource = lstInstruments;

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
        }

    }
}
