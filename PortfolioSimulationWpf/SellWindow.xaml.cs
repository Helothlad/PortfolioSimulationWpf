using System;
using System.ComponentModel;
using System.Windows;

namespace PortfolioSimulationWpf
{
    public partial class SellWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int sellQuantity;
        public int SellQuantity
        {
            get => sellQuantity;
            set
            {
                if (sellQuantity != value)
                {
                    sellQuantity = value;
                    OnPropertyChanged(nameof(SellQuantity));
                    OnPropertyChanged(nameof(TotalGain));
                }
            }
        }

        public Asset Asset { get; set; }

        public int AvailableQuantity => Asset.Quantity;

        public decimal TotalGain => SellQuantity * Asset.CurrentPrice;

        public SellWindow(Asset asset)
        {
            InitializeComponent();
            Asset = asset;
            SellQuantity = 1;
            this.DataContext = this;
        }

        private void IncreaseClick(object sender, RoutedEventArgs e)
        {
            if (SellQuantity < Asset.Quantity)
            {
                SellQuantity++;
            }
        }

        private void DecreaseClick(object sender, RoutedEventArgs e)
        {
            if (SellQuantity > 1)
            {
                SellQuantity--;
            }
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext is ViewModel vm)
            {
                try
                {
                    vm.SellAsset(Asset, SellQuantity);
                    Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Sell Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
