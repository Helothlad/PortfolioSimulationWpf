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
                if (SellQuantity <= Asset.Quantity)
                {
                    decimal saleProceeds = SellQuantity * Asset.CurrentPrice;
                    decimal costBasis = SellQuantity * Asset.AverageEntryPrice;
                    decimal realized = saleProceeds - costBasis;

                    Asset.Quantity -= SellQuantity;
                    Asset.RealizedPnL += realized;

                    // No need to adjust averageEntryPrice when selling

                    vm.Cash += saleProceeds;
                    vm.NotifyTotalsChanged();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("You don't own that many units.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
