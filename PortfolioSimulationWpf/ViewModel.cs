using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace PortfolioSimulationWpf
{
    
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private decimal cash;
        private Asset? selectedAsset;
        private AssetType? selectedAssetTypeFilter;
        private bool isFilterEnabled;
        public BindingList<Asset> FilteredAssets { get; private set; }
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
        public AssetType? SelectedAssetTypeFilter
        {
            get => selectedAssetTypeFilter;
            set
            {
                selectedAssetTypeFilter = value;
                OnPropertyChanged();
                RefreshFilter();
            }
        }
        public bool IsFilterEnabled
        {
            get => isFilterEnabled;
            set
            {
                isFilterEnabled = value;
                OnPropertyChanged();
                RefreshFilter();
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
            this.FilteredAssets = new BindingList<Asset>(this.Assets.ToList());

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

            UpdateFilteredAssets();

            NotifyTotalsChanged();
        }
        private void RefreshFilter()
        {
            FilteredAssets.RaiseListChangedEvents = false;
            FilteredAssets.Clear();

            // Filter the assets based on the filter criteria and exclude those with 0 quantity
            IEnumerable<Asset> filtered = Assets.Where(a => a.Quantity > 0);  // Exclude assets with 0 quantity

            if (IsFilterEnabled)
            {
                if (SelectedAssetTypeFilter == null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show("Please select an asset type before enabling the filter");
                    });

                    IsFilterEnabled = false;
                }
                else
                {
                    filtered = filtered.Where(a => a.AssetType == SelectedAssetTypeFilter);
                }
            }

            foreach (var asset in filtered)
                FilteredAssets.Add(asset);

            FilteredAssets.RaiseListChangedEvents = true;
            FilteredAssets.ResetBindings();
        }
        public void UpdateFilteredAssets()
        {
            RefreshFilter();
        }
    }
}