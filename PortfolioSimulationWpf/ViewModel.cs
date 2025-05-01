using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PortfolioSimulationWpf
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private decimal _cash;
        private Asset? _selectedAsset;
        private AssetType? _selectedAssetTypeFilter;
        private bool _isFilterEnabled;

        public BindingList<Asset> FilteredAssets { get; }
        public BindingList<Asset> Assets { get; }
        public BindingList<Asset> SoldAssets { get; }

        public Asset? SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                if (_selectedAsset != value)
                {
                    _selectedAsset = value;
                    OnPropertyChanged();
                }
            }
        }

        public AssetType? SelectedAssetTypeFilter
        {
            get => _selectedAssetTypeFilter;
            set
            {
                if (_selectedAssetTypeFilter != value)
                {
                    _selectedAssetTypeFilter = value;
                    OnPropertyChanged();
                    RefreshFilter();
                }
            }
        }

        public bool IsFilterEnabled
        {
            get => _isFilterEnabled;
            set
            {
                if (_isFilterEnabled != value)
                {
                    _isFilterEnabled = value;
                    OnPropertyChanged();
                    RefreshFilter();
                }
            }
        }

        public decimal Cash
        {
            get => _cash;
            set
            {
                if (_cash != value)
                {
                    _cash = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalValue));
                }
            }
        }

        public decimal TotalAssetsValue => Assets.Sum(a => a.TotalValue);
        public decimal TotalUnrealizedPnL => Assets.Sum(a => a.UnrealizedPnL);
        public decimal TotalValue => Cash + TotalAssetsValue;

        // Realized PnL is tracked by sold assets' (SalePrice - AverageEntryPrice) * Quantity
        public decimal TotalRealizedPnL => SoldAssets.Sum(a =>
            (a.CurrentPrice - a.AverageEntryPrice) * a.Quantity);

        public ViewModel()
        {
            Assets = new BindingList<Asset>
            {
                new Asset("AAPL", AssetType.Stock, 10),
                new Asset("BTC", AssetType.Crypto, 1),
                new Asset("SPY", AssetType.ETF, 5)
            };

            SoldAssets = new BindingList<Asset>();
            FilteredAssets = new BindingList<Asset>(Assets.ToList());
            Cash = 3000m;

            Assets.ListChanged += (s, e) => UpdateTotals();
            SoldAssets.ListChanged += (s, e) => UpdateTotals();
        }

        public void SellAsset(Asset asset, int quantityToSell)
        {
            if (quantityToSell <= 0 || quantityToSell > asset.Quantity)
                throw new ArgumentException("Invalid quantity to sell.");

            // Create a copy of the asset for the sold portion
            var soldAsset = new Asset(
                asset.Ticker,
                asset.AssetType,
                quantityToSell,
                asset.AverageEntryPrice)
            {
                PriceHistory = new List<decimal>(asset.PriceHistory)
            };

            // Update the original asset
            asset.Quantity -= quantityToSell;
            asset.AverageEntryPrice = asset.Quantity > 0 ?
                asset.AverageEntryPrice : 0; // Reset if fully sold

            // Add to sold assets
            SoldAssets.Add(soldAsset);

            // Update cash
            Cash += quantityToSell * asset.CurrentPrice;

            // If fully sold, remove from assets
            if (asset.Quantity == 0)
            {
                Assets.Remove(asset);
            }

            UpdateTotals();
            RefreshFilter();
        }

        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(TotalAssetsValue));
            OnPropertyChanged(nameof(TotalUnrealizedPnL));
            OnPropertyChanged(nameof(TotalRealizedPnL));
            OnPropertyChanged(nameof(TotalValue));
        }

        private void RefreshFilter()
        {
            var filtered = Assets.AsEnumerable();

            if (IsFilterEnabled && SelectedAssetTypeFilter != null)
            {
                filtered = filtered.Where(a => a.AssetType == SelectedAssetTypeFilter);
            }

            FilteredAssets.Clear();
            foreach (var asset in filtered)
            {
                FilteredAssets.Add(asset);
            }
        }
        public void NotifyTotalsChanged()
        {
            OnPropertyChanged(nameof(TotalAssetsValue));
            OnPropertyChanged(nameof(TotalRealizedPnL));
            OnPropertyChanged(nameof(TotalUnrealizedPnL));
            OnPropertyChanged(nameof(TotalValue));
        }
        public void UpdateFilteredAssets()
        {
            RefreshFilter();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SimulateDay()
        {
            foreach (var asset in Assets)
            {
                asset.Simulate();
            }
            UpdateTotals();
        }
    }
}