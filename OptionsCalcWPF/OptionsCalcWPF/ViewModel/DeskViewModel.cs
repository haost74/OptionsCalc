using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;

namespace OptionsCalcWPF.ViewModel
{
    class DeskViewModel:INotifyPropertyChanged
    {
        public DeskViewModel(Entities.Instrument Call, Entities.Instrument Put, double strike, DateTime MatDate)
        {
            this._Put = Put;
            this._Call = Call;
            this._Strike = strike;
            this._MaturityDate = MatDate;
            this._IsViewed = false;
        }
        
        private double _Strike;
        private bool _IsViewed;
        private Entities.Instrument _Call;
        private Entities.Instrument _Put;
        private DateTime _MaturityDate;

        public bool IsViewed { get { return this._IsViewed; }
            set {
                if (this._IsViewed != value)
                {
                    this._IsViewed = value;
                    NotifyPropertyChange("IsViewed");
                }
            }
        }
        public Entities.Instrument Call { get { return _Call; } }
        public Entities.Instrument Put { get { return _Put; } }
        public double Strike { get { return _Strike; } }
        public DateTime MaturityDate { get { return _MaturityDate; } }

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
