using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

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
    public static class EnumHelper
    {
        public static Array AssetTypes => Enum.GetValues(typeof(AssetType));
    }

    public class Asset : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string ticker;
        private AssetType assetType;
        private int quantity;
        private List<decimal> priceHistory;
        private decimal realizedPnL = 0;
        private decimal averageEntryPrice;

        public static Random rng = new Random();

        public string Ticker
        {
            get => ticker;
            set { ticker = value; OnPropertyChanged(); }
        }

        public AssetType AssetType
        {
            get => assetType;
            set { assetType = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalValue));
                    OnPropertyChanged(nameof(PnL));
                    OnPropertyChanged(nameof(UnrealizedPnL));
                }
            }
        }

        public decimal CurrentPrice => priceHistory.LastOrDefault();

        public decimal TotalValue => Quantity * CurrentPrice;

        public List<decimal> PriceHistory
        {
            get => priceHistory;
            set
            {
                if (priceHistory != value)
                {
                    priceHistory = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentPrice));
                    OnPropertyChanged(nameof(TotalValue));
                    OnPropertyChanged(nameof(PnL));
                    OnPropertyChanged(nameof(UnrealizedPnL));
                }
            }
        }

        public decimal RealizedPnL
        {
            get => realizedPnL;
            set
            {
                if (realizedPnL != value)
                {
                    realizedPnL = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PnL));
                }
            }
        }

        public decimal AverageEntryPrice
        {
            get => averageEntryPrice;
            set
            {
                if (averageEntryPrice != value)
                {
                    averageEntryPrice = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PnL));
                    OnPropertyChanged(nameof(UnrealizedPnL));
                }
            }
        }

        public decimal EntryPrice => PriceHistory.FirstOrDefault();

        public decimal UnrealizedPnL => (CurrentPrice - AverageEntryPrice) * Quantity;

        public decimal PnL => RealizedPnL + UnrealizedPnL;

        public Asset(string ticker, AssetType assetType, int quantity)
        {
            this.ticker = ticker;
            this.assetType = assetType;
            this.quantity = quantity;
            decimal initialPrice = Math.Round((decimal)(rng.NextDouble() * (1000 - 10) + 10), 1);
            priceHistory = new List<decimal>() { initialPrice };
            this.averageEntryPrice = initialPrice;
        }
        public Asset(string ticker, AssetType assetType, int quantity, decimal initialPrice)
        {
            this.ticker = ticker;
            this.assetType = assetType;
            this.quantity = quantity;
            this.priceHistory = new List<decimal>() { initialPrice };
            this.averageEntryPrice = initialPrice;
        }

        public void Simulate()
        {
            double percentChange = (rng.NextDouble() * 0.10) - 0.05;
            decimal lastPrice = CurrentPrice;
            decimal newPrice = lastPrice + (lastPrice * (decimal)percentChange);
            newPrice = Math.Round(newPrice, 2);

            if (newPrice < 1m)
                newPrice = 1m;

            priceHistory.Add(newPrice);

            OnPropertyChanged(nameof(PriceHistory));
            OnPropertyChanged(nameof(CurrentPrice));
            OnPropertyChanged(nameof(TotalValue));
            OnPropertyChanged(nameof(PnL));
            OnPropertyChanged(nameof(UnrealizedPnL));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}