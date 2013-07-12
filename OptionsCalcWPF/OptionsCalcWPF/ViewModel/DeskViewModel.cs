using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OptionsCalcWPF.ViewModel
{
    class DeskViewModel:INotifyPropertyChanged
    {
        public DeskViewModel(int CallId, int PutId)
        {
            this.PId = PutId;
            this.CId = CallId;
            DataManager.Instriments.Where(Entities.Instrument i => i.Id==this.PId).PropertyChanged+=OnPutChange;
            DataManager.Instriments.Where(Entities.Instrument i => i.Id==this.CId).PropertyChanged+=OnCallChange;
        }
        private void OnPutChange(object sender, PropertyChangedEventArgs e)
        { 
            switch(e.PropertyName)
            {
                case("LastPrice") : this._PLastPrice=sender.LastPrice;
            }
        }
        private double _Strike;
        private double _CBid;
        private double _CAsk;
        private double _PBid;
        private double _PAsk;
        private double _Volatility;
        private double _PLastPrice;
        private double _CLastPrice;
        private int CId;
        private int PId;

        public double Strike { get { return _Strike; } }
        public double Volatility { get { return _Volatility; }
           /*
            set {
                if (value != this._Volatility)
                {
                    this._Volatility = value;
                    NotifyPropertyChange("Volatility");
                }
            }
             */
        }
        public double PLastPrice { get { return _PLastPrice; }
            set {
                if (value != this._PLastPrice)
                {
                    this._PLastPrice = value;
                    NotifyPropertyChange("PLastPrice");
                }
            }
        }


        private void NotifyPropertyChange(string name)
        {
            PropertyChangedEventHandler t = PropertyChanged;
            if (t!= null)
            { 
                t(this, new PropertyChangedEventArgs( name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
