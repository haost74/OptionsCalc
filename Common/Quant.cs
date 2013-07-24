using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Common
{
    public static class Quant
    {
        
        public const double RiskFreeRate = 0;

        public const int YearLength = 365;

        public static double NormalDistribution(double value)
        {
            if (value > 6.0) { return (1); }
            if (value < -6.0) { return (0); }

            double b1 = 0.31938153;
            double b2 = -0.356563782;
            double b3 = 1.781477937;
            double b4 = -1.821255978;
            double b5 = 1.330274429;
            double p = 0.2316419;
            double c2 = 0.3989423;

            double a = Math.Abs(value);
            double t = 1.0 / (1.0 + a * p);
            double b = c2 * Math.Exp((-1.0 * value) * (value / 2.0));
            double n = ((((b5 * t + b4) * t + b3) * t + b2) * t + b1) * t;
            n = 1.0 - b * n;
            if (value < 0.0) { n = 1.0 - n; }
            return (n);
            

        }

        public static double NormalDistributionDensity(double value)
        {
            return (Math.Exp(-0.5 * value * value) / Math.Sqrt(2 * Math.PI));
        }

        public static double CalculateDelta(Entities.OptionType type, double settleprice, double strike, double volatility, int daystomate, double risk_free)
        {
            double pdaystomate = (double)daystomate / YearLength;
            double d1=(Math.Log(settleprice/strike)+volatility*volatility*0.5*pdaystomate)/(volatility*Math.Sqrt(pdaystomate));

            if (type == Entities.OptionType.Call)
            {
                return Math.Exp((-1 * risk_free * pdaystomate) * NormalDistribution(d1));
            }
            else if (type == Entities.OptionType.Put)
            {
                return -1.0 * Math.Exp(-1.0 * risk_free * pdaystomate) * NormalDistribution(-1.0 * d1);
            }
            else
            {
                return 0;
            }
	
        }

        public static double CalculateGamma(double settleprice, double strike, double volatility, int daystomate, double risk_free)
        {
            double pdaystomate = (double)daystomate / YearLength;
            return 100 * NormalDistributionDensity(Math.Log(settleprice / strike)) / (settleprice * volatility * Math.Sqrt(pdaystomate));
        }

        public static double CalculateVega(Entities.OptionType type, double settleprice, double strike, double volatility, int daystomate, double risk_free)
        {
            double pdaystomate = (double)daystomate / YearLength;
            double d1 = (Math.Log(settleprice / strike) + volatility * volatility * 0.5 * pdaystomate) / (volatility * Math.Sqrt(pdaystomate));
            
            return settleprice*NormalDistributionDensity(d1)*Math.Exp(-1*risk_free*pdaystomate)*Math.Sqrt(pdaystomate)/100;

        }

        public static double CalculateThetha(Entities.OptionType type, double settleprice, double strike, double volatility, int daystomate, double risk_free)
        {
            double pdaystomate = (double)daystomate / YearLength;
            double d1 = (Math.Log(settleprice / strike) + volatility * volatility * 0.5 * pdaystomate) / (volatility * Math.Sqrt(pdaystomate));
            double temp = settleprice * Math.Exp(-1 * risk_free * pdaystomate);
            double d2 = d1 - volatility * Math.Sqrt(pdaystomate);

            if (type == Entities.OptionType.Call)
            {
                return (-1 * (temp * NormalDistributionDensity(d1) * volatility) / (2 * Math.Sqrt(pdaystomate)) + risk_free * temp * NormalDistribution(d1) - risk_free * strike * temp * NormalDistribution(d2)) / Quant.YearLength;
            }
            else if (type == Entities.OptionType.Put)
            {
                return (-1 * (temp * NormalDistributionDensity(d1) * volatility) / (2 * Math.Sqrt(pdaystomate)) - risk_free * temp * NormalDistribution(-1 * d1) + risk_free * strike * temp * NormalDistribution(-1 * d2)) / Quant.YearLength;
            }
            else
            {
                return 0;
            }
        }

        public static double CalculateRho(Entities.OptionType type, double settleprice, double strike, double volatility, int daystomate, double risk_free)
        {
            double pdaystomate = (double)daystomate / YearLength;
            double d1 = (Math.Log(settleprice / strike) + volatility * volatility * 0.5 * pdaystomate) / (volatility * Math.Sqrt(pdaystomate));
            double d2 = d1 - volatility * Math.Sqrt(pdaystomate);

            if (type == Entities.OptionType.Call)
            {
                return pdaystomate * strike * Math.Exp(-1 * risk_free * pdaystomate) * NormalDistribution(d2) / 100;
            }
            else if (type == Entities.OptionType.Put)
            {
                return -1 * pdaystomate * strike * Math.Exp(-1 * risk_free * pdaystomate) * NormalDistribution(-1 * d2) / 100;
            }
            else
            {
                return 0;
            }
        }

        public static double CalculateTheorPrice()
        {
            throw new NotImplementedException();
        }
    }
}
