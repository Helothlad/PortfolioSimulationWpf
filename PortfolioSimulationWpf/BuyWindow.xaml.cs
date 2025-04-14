using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PortfolioSimulationWpf
{
    /// <summary>
    /// Interaction logic for BuyWindow.xaml
    /// </summary>
    public partial class BuyWindow : Window
    {
        public decimal AvailableCash { get; set; }
        public int BuyQuantity { get; set; }
        public Asset Asset { get; set; }
        public decimal TotalCost => BuyQuantity * Asset.CurrentPrice;
        public BuyWindow(Asset asset)
        {
            
            InitializeComponent();
            BuyQuantity = 1;
            this.Asset = asset;
            if (Application.Current.MainWindow.DataContext is ViewModel vm)
            {
                AvailableCash = vm.Cash;
            }
            this.DataContext = this;                
        }

        private void IncreaseClick(object sender, RoutedEventArgs e)
        {
            BuyQuantity++;
            Refresh();
        }

        private void DecreaseClick(object sender, RoutedEventArgs e)
        {
            if(BuyQuantity > 1) 
            {
                BuyQuantity--;
                Refresh();
            }
        }
        private void Refresh()
        {
            // Refresh bindings manually
            this.DataContext = null;
            this.DataContext = this;
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext is ViewModel vm)
            {
                decimal cost = TotalCost;

                if (vm.Cash >= cost)
                {
                    vm.Cash -= cost;

                    // Calculate new average entry price
                    decimal totalCostBasis = (Asset.AverageEntryPrice * Asset.Quantity) + cost;
                    Asset.Quantity += BuyQuantity;
                    Asset.AverageEntryPrice = totalCostBasis / Asset.Quantity;

                    vm.NotifyTotalsChanged();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Insuffient funds to complete this purchase.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


    }
}
