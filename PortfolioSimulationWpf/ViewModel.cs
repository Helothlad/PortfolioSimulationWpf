using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PortfolioSimulationWpf
{
    
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private decimal cash;
        private Asset? selectedAsset;

        public BindingList<Asset> Assets { get; private set; }

        public Asset? SelectedAsset
        {
            get => selectedAsset;
            set
            {
                if (selectedAsset != value)
                {
                    selectedAsset = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Cash
        {
            get => cash;
            set
            {
                if (cash != value)
                {
                    cash = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalValue));
                }
            }
        }

        public decimal TotalAssetsValue => Assets.Sum(a => a.TotalValue);

        public decimal TotalRealizedPnL => Assets.Sum(a => a.RealizedPnL);

        public decimal TotalUnrealizedPnL => Assets.Sum(a => a.UnrealizedPnL);

        public decimal TotalValue => Cash + TotalAssetsValue;

        public ViewModel()
        {
            this.Assets = new BindingList<Asset>()
            {
                new Asset("AAPL", AssetType.Stock, 10),
                new Asset("BTC", AssetType.Crypto, 1),
                new Asset("SPY", AssetType.ETF, 5)
            };
            Cash = 3000m;

            this.Assets.ListChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalAssetsValue));
                OnPropertyChanged(nameof(TotalRealizedPnL));
                OnPropertyChanged(nameof(TotalUnrealizedPnL));
                OnPropertyChanged(nameof(TotalValue));
            };
        }

        public void SimulateDay()
        {
            foreach (var asset in Assets)
            {
                asset.Simulate();
            }
            NotifyTotalsChanged();
        }

        public void NotifyTotalsChanged()
        {
            OnPropertyChanged(nameof(TotalAssetsValue));
            OnPropertyChanged(nameof(TotalRealizedPnL));
            OnPropertyChanged(nameof(TotalUnrealizedPnL));
            OnPropertyChanged(nameof(TotalValue));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SellAsset(Asset asset, int quantityToSell)
        {
            if (quantityToSell <= 0 || quantityToSell > asset.Quantity)
                throw new ArgumentException("Invalid quantity to sell.");

            decimal proceeds = quantityToSell * asset.CurrentPrice;
            decimal costBasis = quantityToSell * asset.AverageEntryPrice;
            decimal realizedPnL = proceeds - costBasis;

            asset.Quantity -= quantityToSell;
            asset.RealizedPnL += realizedPnL;
            Cash += proceeds;

            if (asset.Quantity == 0)
            {
                Assets.Remove(asset);
            }

            NotifyTotalsChanged();
        }
    }
}