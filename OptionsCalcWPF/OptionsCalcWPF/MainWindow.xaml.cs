﻿using System;
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
        BindingList<ViewModel.DeskViewModel> allViewModels;
        BindingList<ViewModel.DeskViewModel> lstDesc;
        List<string> listBaseContract;
        Dictionary<string, List<DateTime>> MatDates2BaseContracts;
        Thread dataThread;
        Logger mainLog;
        private bool _IsConnected=false;

        public MainWindow()
        {
            InitializeComponent();
            mainLog = LogManager.GetLogger("Main");
        }

        private void ConnectionManager_OnConnected(string obj)
        {
            labelConnectionStatus.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        _IsConnected = true;
                        labelConnectionStatus.Content = "Connected";
                        mainLog.Info("Connected to datastream");
                        dgrODesk.Items.Refresh();
                        foreach (var i in lstInstruments)
                        {
                            if (i.Type == Entities.InstrumentType.Option && i.OptionType==Entities.OptionType.Call)
                            {
                                var t = lstInstruments.First(k => k.Strike == i.Strike && k.OptionType == Entities.OptionType.Put && k.BaseContract == i.BaseContract && k.DaysToMate==i.DaysToMate);
                                allViewModels.Add(new ViewModel.DeskViewModel(i,t,(double)i.Strike,(DateTime)i.MaturityDate));
                                if (!listBaseContract.Any(k => k == i.BaseContract))
                                {
                                    listBaseContract.Add(i.BaseContract);
                                }
                                if (!MatDates2BaseContracts.ContainsKey(i.BaseContract))
                                {
                                    MatDates2BaseContracts.Add(i.BaseContract, new List<DateTime>());
                                    MatDates2BaseContracts[i.BaseContract].Add((DateTime)i.MaturityDate);
                                }
                                else
                                {
                                    if (!MatDates2BaseContracts[i.BaseContract].Any(k => k == i.MaturityDate))
                                    {
                                        MatDates2BaseContracts[i.BaseContract].Add((DateTime)i.MaturityDate);
                                    }
                                }
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
            if (!_IsConnected)
            {
                mainLog.Info("Try to Connect to datastream");
                btnConnect.Content = "Disconnect";
                dataThread = new Thread(ConnectionManager.Connect);
                dataThread.Start();
                //ConnectionManager.Connect();
            }

            if (_IsConnected)
            {
                mainLog.Info("Try to Disconnect from datastream");
                btnConnect.Content = "Connect";
                ConnectionManager.Disconect();
                labelConnectionStatus.Content = "Disconnected";
            }
        }

        private void OptionsCalculator_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataManager.Init();
                lstPortfolios = new BindingList<Entities.Portfolio>(DataManager.Portfolios);
                lstInstruments = new BindingList<Entities.Instrument>(DataManager.Instruments);
                lstDesc = new BindingList<ViewModel.DeskViewModel>();
                allViewModels = new BindingList<ViewModel.DeskViewModel>();
                listBaseContract = new List<string>();
                MatDates2BaseContracts=new Dictionary<string,List<DateTime>>();

                ConnectionManager.OnAccount += DataManager.UpdateAccount;
                ConnectionManager.OnNewInstrument += DataManager.AddInstrument;
                ConnectionManager.OnInstrument += DataManager.UpdateInstrument;
                ConnectionManager.OnPosition += DataManager.UpdatePosition;
                ConnectionManager.OnConnected += new Action<string>(ConnectionManager_OnConnected);

                dgrPortfolios.ItemsSource = lstPortfolios;
                dgrODesk.ItemsSource = lstInstruments;
                dgrDesc.ItemsSource = lstDesc;
                comboBoxBaseContract.ItemsSource = listBaseContract;
                //string a = comboBoxBaseContract.SelectedItem;
                //ComboBoxItem itm = (ComboBoxItem)comboBoxBaseContract.SelectedItem;
                //comboBoxMatDate.ItemsSource = comboBoxBaseContract.SelectedItem.Content;
                //comboBoxMatDate.ItemsSource = MatDates2BaseContracts[comboBoxBaseContract.Text];
                //comboBoxMatDate.DisplayMemberPath = MatDates2BaseContracts[comboBoxBaseContract.SelectedValue];
                
            }
            catch (Exception exp)
            {
                mainLog.Error("Error on DataManager or ConnectionManager initialization {0}",exp.Message);
            }
        }

        private void btnRefreshDesc_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
