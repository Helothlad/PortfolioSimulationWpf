using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PortfolioSimulationWpf
{
    public enum AssetType
    {
        Stock,
        ETF,
        Crypto,
        Bond,
        Commodity
    }
    public class Asset : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string ticker;
        private AssetType assetType;
        private int quantity;
        private List<decimal> priceHisotry;
        public static Random rng;
        public decimal CurrentPrice
        {
            get => priceHisotry.LastOrDefault();
        } 
        public decimal TotalValue 
        {
            get => quantity * CurrentPrice;
        } 
        public Asset(string ticker, AssetType assetType, int quantity)
        {
            this.ticker= ticker;
            this.assetType = assetType;
            this.quantity = quantity;
            rng = new Random();
            priceHisotry = new List<decimal>()
            {
                new decimal(10.0),
            };
        }
        public List<decimal> PriceHisotry
        {
            get { return priceHisotry; }
            set 
            {
                if(priceHisotry != value)
                {
                    priceHisotry = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentPrice));
                    OnPropertyChanged(nameof(TotalValue));
                }
            }
        }
        public string Ticker
        {
            get { return ticker; }
            set { ticker = value; OnPropertyChanged(); }
        }
        public AssetType AssetType
        {
            get { return assetType; }
            set { assetType = value; OnPropertyChanged(); }
        }
        public int Quantity
        {
            get { return quantity; }
            set 
            { 
                if( quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalValue));
                }
                
            
            }
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName="") 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Simulate()
        {
            double percentChange = (rng.NextDouble() * 100) - 0.05;
            decimal lastPrice = CurrentPrice;
            decimal newPrice = lastPrice + (lastPrice * (decimal)percentChange);
            newPrice = Math.Round(newPrice, 2);

            if (newPrice < 1m)
                newPrice = 1m;

            priceHisotry.Add(newPrice);
        }
    }
}
