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
using NLog;

namespace OptionsCalcWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<Entities.Portfolio> lstPortfolios;
        BindingList<Entities.Instrument> lstInstruments;
        BindingList<ViewModel.DeskViewModel> lstTest; 
        Thread dataThread;
        Logger mainLog;

        public MainWindow()
        {
            InitializeComponent();
            mainLog = LogManager.GetLogger("Main");
        }



        private void ConnectionManager_OnConnected(string obj)
        {
            textBoxConnectionStatus.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        textBoxConnectionStatus.Text = "Connected";
                        mainLog.Info("Connected to datastream");
                        dgrODesk.Items.Refresh();
                        foreach (var i in lstInstruments)
                        {
                            if (i.Type == Entities.InstrumentType.Option && i.OptionType==Entities.OptionType.Call)
                            {
                                var t = lstInstruments.First(k => k.Strike == i.Strike && k.OptionType == Entities.OptionType.Put && k.BaseContract == i.BaseContract && k.DaysToMate==i.DaysToMate);
                                lstTest.Add(new ViewModel.DeskViewModel(i,t,(double)i.Strike,(DateTime)i.MaturityDate));
                            }
                        }
                    }
                    ));
            
        }

        private void OptionsCalculator_Closed(object sender, EventArgs e)
        {
            mainLog.Info("Disconnect from datastream on FormClose");
            ConnectionManager.Disconect();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxConnectionStatus.Text == "Disconnected")
            {
                mainLog.Info("Try to Connect to datastream");
                btnConnect.Content = "Disconnect";
                dataThread = new Thread(ConnectionManager.Connect);
                dataThread.Start();
                //ConnectionManager.Connect();
            }

            if (textBoxConnectionStatus.Text == "Connected")
            {
                mainLog.Info("Try to Disconnect from datastream");
                btnConnect.Content = "Connect";
                ConnectionManager.Disconect();
                textBoxConnectionStatus.Text = "Disconnected";
            }
        }

        private void OptionsCalculator_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataManager.Init();
                lstPortfolios = new BindingList<Entities.Portfolio>(DataManager.Portfolios);
                lstInstruments = new BindingList<Entities.Instrument>(DataManager.Instruments);
                lstTest=new BindingList<ViewModel.DeskViewModel>();

                ConnectionManager.OnAccount += DataManager.UpdateAccount;
                ConnectionManager.OnNewInstrument += DataManager.AddInstrument;
                ConnectionManager.OnInstrument += DataManager.UpdateInstrument;
                ConnectionManager.OnPosition += DataManager.UpdatePosition;
                ConnectionManager.OnConnected += new Action<string>(ConnectionManager_OnConnected);

                dgrPortfolios.ItemsSource = lstPortfolios;
                dgrODesk.ItemsSource = lstInstruments;
                dgrTest.ItemsSource = lstTest;
            }
            catch (Exception exp)
            {
                mainLog.Error("Error on DataManager or ConnectionManager initialization {0}",exp.Message);
            }
        }

    }
}
