using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using QuikConnectionManager;
using NLog;


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
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void Init()
        { 
            Instruments=new BindingList<Entities.Instrument>();
            Positions=new BindingList<Entities.Position>();
            Portfolios=new BindingList<Entities.Portfolio>();
            Accounts=new BindingList<Entities.Account>();
        }

        public static void AddInstrument(QuikConnectionManager.StaticInstrument obj)
        {
            if (Instruments.Any(k=> k.Id==obj.Id))
            {
                //var i=Instruments.First(k => k.Id == obj.Id);
                log.Warn("Trying to add existing instrument Id={0} Code={1} Class={2} ",obj.Id,obj.Code,obj.Class);

            }
            else
            {
                if (obj.InstrumentType == "Futures")
                {
                    Entities.Instrument instr = new Entities.Instrument(obj.Id, obj.Code, obj.Class, Entities.InstrumentType.Futures, obj.BaseContract, obj.FullName);
                    instr.DaysToMate = obj.DaysToMate;
                    instr.MaturityDate = Convert.ToDateTime(obj.MaturityDate).Date;
                    Instruments.Add(instr);
                }
                else
                {
                    Entities.Instrument instr = new Entities.Instrument(obj.Id, obj.Code, obj.Class, Entities.InstrumentType.Option, obj.FullName, obj.OptionType == "Call" ? Entities.OptionType.Call : Entities.OptionType.Put, obj.Strike, obj.BaseContract);
                    instr.DaysToMate = obj.DaysToMate;
                    instr.MaturityDate = Convert.ToDateTime(obj.MaturityDate).Date;
                    Instruments.Add(instr);
                }
                log.Trace("New instrument added Id={0} Code={1} Class={2}",obj.Id,obj.Code,obj.Class);
            }
        }

        public static void UpdateInstrument(QuikConnectionManager.DynamicInstrument obj)
        {
            try
            {
                var i = Instruments.First(k => k.Id == obj.Id);
                i.LastPrice = obj.LastPrice;
                i.Volatility = obj.Volatility;
                i.TheorPrice = obj.TheorPrice;
                i.BestAsk = obj.Ask;
                i.BestAskVolume = obj.AskVol;
                i.BestBid = obj.Bid;
                i.BestBidVolume = obj.BidVol;
            }
            catch (SystemException e)
            {
                log.Error("Update for unknown instrument Id={0}",obj.Id);
            }
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
                log.Info("New account added Id={0} Name={1}",obj.Id,obj.Name);
            }
        }

        public static void UpdatePosition(QuikConnectionManager.Position obj)
        {
            if(Positions.Any(k=>k.AccountName==obj.AccountName && k.Instrument.Code==obj.SecurityCode))
            {
                Entities.Position p = Positions.First(k => k.AccountName == obj.AccountName && k.Instrument.Code == obj.SecurityCode);
                p.TotalNet = obj.TotalNet; 
            }
            else
            {
                Entities.Instrument instr;
                try
                {
                    instr = Instruments.First(k => k.Code == obj.SecurityCode);
                    var pos = new Entities.Position(obj.AccountName, instr, obj.TotalNet, 0, 0);
                    Positions.Add(pos);
                    log.Info("New position added. SecCode={0} Account={1}", obj.SecurityCode, obj.AccountName);
                    if (pos.Instrument.Type == Entities.InstrumentType.Futures)
                    {
                        if(Portfolios.Any(k=>k.BaseCode==pos.Instrument.Code))
                        {
                            Portfolios.First(k => k.BaseCode == pos.Instrument.Code).Positions.Add(pos);
                            log.Info("Futures Position added to existing portfolio.");
                        }
                        else
                        {   // create new portfolio
                            Entities.Portfolio port = new Entities.Portfolio(pos.Instrument.Code, pos.AccountName);
                            port.Positions.Add(pos);
                            Portfolios.Add(port);
                            log.Info("New portfolio created. Name={0} Base={1} Account={2}", port.Name, port.BaseCode, port.Account);
                        }

                    }
                    else if (pos.Instrument.Type == Entities.InstrumentType.Option)
                    {
                        if(Portfolios.Any(k=> k.BaseCode==pos.Instrument.BaseContract))
                        {
                            Portfolios.First(k => k.BaseCode == pos.Instrument.BaseContract).Positions.Add(pos);
                            log.Info("Option Position added to existing portfolio.");
                        }
                        else
                        {   // create new portfolio
                            Entities.Portfolio port = new Entities.Portfolio(pos.Instrument.BaseContract, pos.AccountName);
                            port.Positions.Add(pos);
                            Portfolios.Add(port);
                            log.Info("New portfolio created. Name={0} Base={1} Account={2}", port.Name, port.BaseCode, port.Account);
                        }
                    }
                }
                catch (SystemException exep)
                {
                    log.Error("Try to add position for unknown instrument {0} {1} {2}",obj.SecurityCode,obj.AccountName,obj.TotalNet);
                    log.Error(exep.Message);
                }
                   
            }
        }

    }
}
