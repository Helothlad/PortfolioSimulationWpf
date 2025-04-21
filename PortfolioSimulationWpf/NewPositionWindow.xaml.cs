using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PortfolioSimulationWpf
{
    public partial class NewPositionWindow : Window, INotifyPropertyChanged
    {
        private List<Asset> availableAssets;
        private Asset? selectedAsset;
        private int buyQuantity = 1;
        private decimal availableCash;

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly ViewModel viewModel;
        public NewPositionWindow(ViewModel vm)
        {
            InitializeComponent();
            DataContext = this;
            
            this.viewModel = vm;

            // Dummy data
            AvailableAssets = new List<Asset>
            {
                new Asset("AAPL", AssetType.Stock, 0),
                new Asset("MSFT", AssetType.Stock, 0),
                new Asset("GOOGL", AssetType.Stock, 0),
                new Asset("AMZN", AssetType.Stock, 0),
                new Asset("TSLA", AssetType.Stock, 0),
                new Asset("META", AssetType.Stock, 0),
                new Asset("NVDA", AssetType.Stock, 0),
                new Asset("BRK.A", AssetType.Stock, 0),
                new Asset("NFLX", AssetType.Stock, 0),
                new Asset("Austrian GVBond", AssetType.Bond, 0),
                new Asset("GOLD", AssetType.Commodity, 0)
            };

            AvailableCash = vm.Cash;

            AssetComboBox.ItemsSource = AvailableAssets;
        }
        public List<Asset> AvailableAssets
        {
            get => availableAssets;
            set
            {
                availableAssets = value;
                OnPropertyChanged();
            }
        }
        public Asset? SelectedAsset
        {
            get => selectedAsset;
            set
            {
                selectedAsset = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalCost));
            }
        }
        public int BuyQuantity
        {
            get => buyQuantity;
            set
            {
                if (value >= 1)
                {
                    buyQuantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalCost));
                }
            }
        }
        public decimal AvailableCash
        {
            get => availableCash;
            set
            {
                availableCash = value;
                OnPropertyChanged();
            }
        }
        public decimal TotalCost => SelectedAsset != null ? SelectedAsset.CurrentPrice * BuyQuantity : 0;
        private void IncreaseClick(object sender, RoutedEventArgs e)
        {
            BuyQuantity++;
        }
        private void DecreaseClick(object sender, RoutedEventArgs e)
        {
            if (BuyQuantity > 1)
            {
                BuyQuantity--;
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAsset == null)
            {
                MessageBox.Show("Please select a stock.");
                return;
            }

            if (TotalCost > availableCash)
            {
                MessageBox.Show($"You do not have enough cash.");
                return;
            }



            var existing = viewModel.Assets.FirstOrDefault(a => a.Ticker == SelectedAsset.Ticker);
            if (existing != null)
            {
                MessageBox.Show($"You already have a position in this stock, go on buy more");
                return;
            }
            else
            {
                viewModel.Assets.Add(new Asset(
                    SelectedAsset.Ticker,
                    SelectedAsset.AssetType,
                    BuyQuantity,
                    SelectedAsset.CurrentPrice));
                viewModel.Cash -= TotalCost;
                viewModel.UpdateFilteredAssets();
            }

            MessageBox.Show($"You bought {BuyQuantity} of {SelectedAsset.Ticker} for {TotalCost:C}.");
            Close();
        }
    }
}
