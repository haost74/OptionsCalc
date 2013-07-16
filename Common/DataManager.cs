using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using QuikConnectionManager;


namespace Common
{
    public static class DataManager
    {
        #region Collections
        public static BindingList<Entities.Instrument> Instruments { get; set; }
        public static BindingList<Entities.Position> Positions { get; set; }
        public static BindingList<Entities.Portfolio> Portfolios { get; set; }
        public static BindingList<Entities.Account> Accounts { get; set; }
        #endregion

        public static void Init()
        { 
            Instruments=new BindingList<Entities.Instrument>();
            Positions=new BindingList<Entities.Position>();
            Portfolios=new BindingList<Entities.Portfolio>();
            Accounts=new BindingList<Entities.Account>();
        }

        public static void AddInstrument(QuikConnectionManager.StaticInstrument obj)
        { 
            if (obj.InstrumentType=="Futures")
            {
                Entities.Instrument instr=new Entities.Instrument(obj.Id,obj.Code,obj.Class,Entities.InstrumentType.Futures,obj.BaseContract,obj.FullName);
                instr.DaysToMate = obj.DaysToMate;
                instr.MaturityDate = Convert.ToDateTime(obj.MaturityDate);
                Instruments.Add(instr);
            }
            else
            {
                Entities.Instrument instr=new Entities.Instrument(obj.Id,obj.Code,obj.Class,Entities.InstrumentType.Option,obj.FullName,obj.OptionType=="Call"?Entities.OptionType.Call:Entities.OptionType.Put,obj.Strike,obj.BaseContract);
                instr.DaysToMate = obj.DaysToMate;
                instr.MaturityDate = Convert.ToDateTime(obj.MaturityDate);
                Instruments.Add(instr);
            }
            
        }

        public static void UpdateInstrument(QuikConnectionManager.DynamicInstrument obj)
        {
            Entities.Instrument i = Instruments.First(k => k.Id == obj.Id);
            //if (i == null) { throw new Exception("Can`t find instrument to update"); }
            i.LastPrice = obj.LastPrice;
            i.Volatility = obj.Volatility;
            i.TheorPrice = obj.TheorPrice;
        }

        public static void UpdateAccount(QuikConnectionManager.Account obj)
        {
            try
            {
                Entities.Account a=Accounts.First(k => k.Id == obj.Id);
            }
            catch(SystemException e)
            {
                Accounts.Add(new Entities.Account(obj.Name, obj.Id));
            }
        }

        public static void UpdatePosition(QuikConnectionManager.Position obj)
        {
            try
            {
                Entities.Position p = Positions.First(k => k.AccountName == obj.AccountName && k.Instrument.Code == obj.SecurityCode);
                p.TotalNet = obj.TotalNet; 
            }
            catch (SystemException ex)
            {
                var pos = new Entities.Position(obj.AccountName, Instruments.First(k => k.Code == obj.SecurityCode), obj.TotalNet, 0, 0);
                Positions.Add(pos);
                if(pos.Instrument.Type==Entities.InstrumentType.Futures )
                {
                    try
                    {
                        Portfolios.First(k => k.BaseCode == pos.Instrument.Code).Positions.Add(pos);
                    }
                    catch (SystemException ex1)
                    {   // create new portfolio
                        Entities.Portfolio port = new Entities.Portfolio(pos.Instrument.Code, pos.AccountName);
                        port.Positions.Add(pos);
                        Portfolios.Add(port);
                    }
                    
                }
                else if (pos.Instrument.Type == Entities.InstrumentType.Option)
                {
                    try
                    {
                        Portfolios.First(k => k.BaseCode == pos.Instrument.BaseContract).Positions.Add(pos);
                    }
                    catch (SystemException ex2)
                    {   // create new portfolio
                        Entities.Portfolio port = new Entities.Portfolio(pos.Instrument.BaseContract, pos.AccountName);
                        port.Positions.Add(pos);
                        Portfolios.Add(port);
                    }
                }   
            }
        }

    }
}
