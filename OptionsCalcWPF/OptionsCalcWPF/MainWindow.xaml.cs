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
using System.Threading.Tasks;
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
        MySortableBindingList<ViewModel.DeskViewModel> lstDesc;
        List<string> listBaseContract;
        
        Dictionary<string, List<DateTime>> MatDates2BaseContracts;
        Thread dataThread;
        Task connectionTask;
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
                labelConnectionStatus.Content = "Trying to Connect to data server";
                try
                {
                    //connectionTask = new Task(ConnectionManager.Connect);
                    //connectionTask.ContinueWith(ExeptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                    //var c = new CancellationTokenSource();
                    //CancellationToken ct = c.Token;
                    //connectionTask.Start();
                    dataThread = new Thread(ConnectionManager.Connect);
                    dataThread.Start();
                    
                }
                catch (Exception ex)
                {
                    mainLog.Error(ex.Message);
                    labelConnectionStatus.Content = ex.Message;

                }
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
                lstDesc = new MySortableBindingList<ViewModel.DeskViewModel>();
                allViewModels = new BindingList<ViewModel.DeskViewModel>();
                listBaseContract = new List<string>();
                MatDates2BaseContracts=new Dictionary<string,List<DateTime>>();
                

                ConnectionManager.OnAccount += DataManager.UpdateAccount;
                ConnectionManager.OnNewInstrument += DataManager.AddInstrument;
                ConnectionManager.OnInstrument += DataManager.UpdateInstrument;
                ConnectionManager.OnPosition += DataManager.UpdatePosition;
                ConnectionManager.OnConnected += new Action<string>(ConnectionManager_OnConnected);
                ConnectionManager.OnError += new Action<string>(ConnectionManager_OnError);
                ConnectionManager.OnNewAccount+=DataManager.AddAccount;
                ConnectionManager.OnNewPosition += DataManager.AddPosition;

                dgrPortfolios.ItemsSource = lstPortfolios;
                //dgrODesk.ItemsSource = lstInstruments;
                dgrDesc.CanUserSortColumns = true;
                dgrDesc.ItemsSource = lstDesc;
                
                comboBoxBaseContract.ItemsSource = listBaseContract;
                comboBoxBaseContract.SelectionChanged += new SelectionChangedEventHandler(comboBoxBaseContract_SelectionChanged);
                
            }
            catch (Exception exp)
            {
                mainLog.Error("Error on DataManager or ConnectionManager initialization {0}",exp.Message);
            }
        }

        void ConnectionManager_OnError(string obj)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        mainLog.Error(obj);
                        btnConnect.Content = "Connect";
                        labelConnectionStatus.Content = obj;
                        dataThread.Abort();
                        
                        //var st = connectionTask.Status;
                        //connectionTask.Dispose();
                    }
                    ));
            
        }

        private void btnRefreshDesc_Click(object sender, RoutedEventArgs e)
        {
            //object date=comboBoxMatDate.SelectedItem;
            lstDesc.Clear();
            DateTime d = Convert.ToDateTime(comboBoxMatDate.SelectedItem.ToString());
            string basec=comboBoxBaseContract.SelectedItem.ToString();
            var lst = allViewModels.Where(k => k.MaturityDate == d && k.Call.BaseContract==basec);
            foreach (var elem in lst)
            {
                lstDesc.Add(elem);
            }
            
            dgrDesc.Items.Refresh();
        }

        private void comboBoxBaseContract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBoxBaseContract.SelectedItem == null) return;
                var ind = comboBoxBaseContract.SelectedIndex;
                
                if (ind!=-1)
                {
                    var basec = comboBoxBaseContract.SelectedItem.ToString();
                    Binding bind = new Binding();
                    bind.Source = MatDates2BaseContracts[basec];
                    comboBoxMatDate.SetBinding(ComboBox.ItemsSourceProperty, bind);
                }
            }
            catch(SystemException ex)
            {
                mainLog.Error(ex.Message);
            }
        }


        private void ExeptionHandler(Task task)
        {
            var exp = task.Exception;
            mainLog.Error(exp.Message);
            labelConnectionStatus.Content = exp.Message;
        }
    }
}
