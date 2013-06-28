using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OptionsCalc
{
    class Entities
    {
        
        [SerializableAttribute]
        public static enum OptionType { Call = 1, Put };

        [SerializableAttribute]
        public static enum InstrumentType { Futures, Index, Bond, Option, Stock}

        [Serializable]
        public class Instrument : INotifyPropertyChanged
        {
            public Instrument(int id, string sec_code, string cl, InstrumentType type, string full_name, OptionType? o_type, double? strike, int? basec)
            {
                this._Class = cl;
                this._Code = sec_code;
                this._FullName = full_name;
                this._Id = id;
                this._Type = type;
                if (o_type.HasValue) { this._OptionType = (OptionType)o_type; }
                if (strike.HasValue)
                {
                    if (this._OptionType != OptionType.Call || this._OptionType != OptionType.Put)
                    { 
                        throw new NotImplementedException();
                    } 
                    else this._Strike = (double)strike; 
                }
                if (basec.HasValue) { this._BaseContract = (int)basec; }
            }

            public Instrument(int id, string sec_code, string cl, InstrumentType type, string full_name):this(id,sec_code,cl,type,full_name,null,null,null)
            { 
                
            }

            private string _Code;
            private string _Class;
            private string _FullName;
            private InstrumentType _Type;
            private int _Id;
            private OptionType _OptionType;
            private double _Strike;
            private int _BaseContract;

            public string Code { get{return _Code;} }

            public string FullName { get { return _FullName; } }

            public string Class { get { return _Class; } }

            public int Id { get; }

            public InstrumentType Type { get; }

            public OptionType OptionType { get; }

            public double LastPrice { get; set;}
            
            public double Strike { get; }

            public double Volatility { get; set; }

            public double DaysToMate { get; set; }

            public double Delta 
            {
                get 
                {
                    if (this.Type == InstrumentType.Futures)
                    {
                        return 1;
                    }
                    else if (this.Type == InstrumentType.Option)
                    {
                        return Quant.CalculateDelta(this.OptionType, this.LastPrice, this.Strike, this.Volatility, this.DaysToMate, Quant.RiskFreeRate);
                    }
                    else return 0;
                }
            }


            public double Gamma
            {
                get
                {
                    if (this.Type == InstrumentType.Option)
                    {
                        return Quant.CalculateGamma(this.LastPrice, this.Strike, this.Volatility, this.DaysToMate, Quant.RiskFreeRate);
                    }
                    else return 0;
                }
            }

            public double Vega
            {
                get
                {
                    if (this.Type == InstrumentType.Option)
                    {
                        return Quant.CalculateVega(this.OptionType, this.LastPrice, this.Strike, this.Volatility, this.DaysToMate, Quant.RiskFreeRate);
                    }
                    else return 0;
                }
            }

            public double Thetha
            {
                get
                {
                    if (this.Type == InstrumentType.Option)
                    {
                        return Quant.CalculateThetha(this.OptionType, this.LastPrice, this.Strike, this.Volatility, this.DaysToMate, Quant.RiskFreeRate);
                    }
                    else return 0;
                }
            }

            public double Rho
            {
                get
                {
                    if (this.Type == InstrumentType.Option)
                    {
                        return Quant.CalculateRho(this.OptionType, this.LastPrice, this.Strike, this.Volatility, this.DaysToMate, Quant.RiskFreeRate);
                    }
                    else return 0;
                }
            }

            public override string ToString()
            {
                return base.ToString();
            }

            public int BaseContract { get { return _BaseContract; } }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        [Serializable]
        public class Account 
        {
            public Account(string name, int id)
            {
                this._Name = name;
                this._Id = id;
            }
            private string _Name;
            private int _Id;

            public string Name { get; }

            public int Id { get; }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        [Serializable]
        public class Position : INotifyPropertyChanged
        {
            public Position(Instrument instr):this(instr,0,0,0)
            {
             
            }
            public Position(Instrument instr, int tnet, int bq, int sq)
            {
                this._Instrument = instr;
                this.BuyQty = bq;
                this.SellQty = sq;
                this.TotalNet = tnet;
            }
            private Instrument _Instrument;

            public int TotalNet { get; set; }

            public int BuyQty { get; set; }

            public int SellQty { get; set; }

            public Instrument Instrument { get; }

            public override string ToString()
            {
                return base.ToString();
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        [Serializable]
        public class Portfolio : INotifyPropertyChanged
        {
            public Portfolio(Instrument instr, Account acc)
            {
                this._Base = instr;

                this._Account = acc;
            }

            public Portfolio(Instrument instr, Account acc, Position[] pos)
            {
                this._Base = instr;

                this._Account = acc;

                foreach (Position p in pos)
                {
                    this.Positions.Add(p);
                }

            }
            
            private double _Delta;
            private double _Gamma;
            private double _Vega;
            private double _Thetha;
            private double _Rho;
            private Instrument _Base;
            private Account _Account;

            public string Name { get; set; }

            public Instrument Base { get; }

            public Account Account { get; }

            private List<Position> Positions;

            public  double Delta { get { return _Delta; } }

            public double Gamma { get { return _Gamma; } }

            public double Vega { get { return _Vega; } }
            
            public double Thetha { get { return _Thetha; } }
            
            public double Rho { get { return _Rho; } }

            public void Refresh ()
            {
                foreach (Position pos in Positions)
                {
                    this._Delta += pos.TotalNet * pos.Instrument.GetDelta();
                    this._Gamma += pos.TotalNet * pos.Instrument.GetGamma();
                    this._Vega += pos.TotalNet * pos.Instrument.GetVega();
                    this._Thetha += pos.TotalNet * pos.Instrument.GetThetha();
                    this._Rho += pos.TotalNet * pos.Instrument.GetRho();
                }
                
            }


            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
