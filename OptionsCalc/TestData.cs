using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Timers;



namespace OptionsCalc
{
    class TestData
    {
        private List<Entities.Instrument> _instrumentslist=new List<Entities.Instrument>();
        private BindingList<Entities.Position> _positionslist=new BindingList<Entities.Position>();
        private List<Entities.Portfolio> _portfolioslist= new List<Entities.Portfolio>();

        public List<Entities.Instrument> Instruments{ get{return this._instrumentslist;}}
        public List<Entities.Portfolio> Portfolios{ get{return this._portfolioslist;}}
        public BindingList<Entities.Position> Positions{ get{return this._positionslist;}}
        private static Random rnd = new Random();

        public void Connect()
        { 
            // do smthng
            var basec=new Entities.Instrument(1,"UXU2","FUTUX",Entities.InstrumentType.Futures,"UX-9.13");
            basec.MaturityDate = new DateTime(2013,6,15);
            TimeSpan diff = basec.MaturityDate - DateTime.Now;
            basec.DaysToMate = (int)diff.TotalDays;
            this._instrumentslist.Add(basec);
            var opt1 = new Entities.Instrument(2, "UX000850BU3", "OPTUX", Entities.InstrumentType.Option, "UX 850 Put", Entities.OptionType.Put, 850, basec.Id);
            opt1.DaysToMate = 15;
            var opt2 = new Entities.Instrument(3, "UX000900BI3", "OPTUX", Entities.InstrumentType.Option, "UX 900 Call", Entities.OptionType.Call, 900, basec.Id);
            opt2.DaysToMate = 15;
            var opt3 = new Entities.Instrument(4, "UX000950BI3", "OPTUX", Entities.InstrumentType.Option, "UX 950 Call", Entities.OptionType.Call, 950, basec.Id);
            opt3.DaysToMate = 15;
            this._instrumentslist.Add(opt1);
            this._instrumentslist.Add(opt2);
            this._instrumentslist.Add(opt3);
            
            this._positionslist.Add(new Entities.Position(basec,-25,0,0));
            this._positionslist.Add(new Entities.Position(_instrumentslist.FirstOrDefault(i=>i.Id==2),10,2,4));
            this._positionslist.Add(new Entities.Position(_instrumentslist.FirstOrDefault(i=>i.Id==3),-5,2,4));
            this._positionslist.Add(new Entities.Position(_instrumentslist.FirstOrDefault(i=>i.Id==4),2,2,4));

            var acc=new Entities.Account("my_acc",2);
            this._portfolioslist.Add(new Entities.Portfolio(basec.Id,acc,this._positionslist));
            this._portfolioslist.ElementAt(0).Name = "Test portfolio";
            
        }
        public void Start() {  }

        public void updateData() 
        {
            var id = rnd.Next(1, 4);
            this._instrumentslist.ElementAt(id).Volatility = rnd.NextDouble();
            this._instrumentslist.ElementAt(id).LastPrice = rnd.NextDouble() * 5;
            this._positionslist.ElementAt(id).TotalNet = rnd.Next(0, 10);
            NotifyOnUpdate();
            
        }
        public void Disconnect()
        { 
            //do smthng to disconect
        }
        public event EventHandler OnUpdate;
        private void NotifyOnUpdate()
        {
            EventHandler t = OnUpdate;
            if (t != null)
            {
                t(this, EventArgs.Empty);
            }
        }
    }
}
