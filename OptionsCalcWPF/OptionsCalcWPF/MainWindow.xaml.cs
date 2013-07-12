using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



namespace OptionsCalcWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TestData data;
        BindingList<Entities.Portfolio> lstPortfolios;
        BindingList<Entities.Instrument> lstInstruments;
        DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            data = new TestData();
            data.Connect();
            lstPortfolios = new BindingList<Entities.Portfolio>(data.Portfolios);
            lstInstruments = new BindingList<Entities.Instrument>(data.Instruments);
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimer;
            _timer.Interval = new TimeSpan(0,0,2);
            _timer.Start();
            dgrPortfolios.ItemsSource = lstPortfolios;
            dgrODesk.ItemsSource = lstInstruments;
        }

        void OnTimer(object s, EventArgs e)
        {
            data.updateData();
        }
    }
}
