using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioSimulationWpf
{
    internal class ViewModel : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public BindingList<Asset> Assets { get; private set; }
        public Asset? SelectedAsset { get; set; }
        public decimal Cash { get; private set; }
        public decimal TotalAssetsValue => Assets.Sum(a => a.TotalValue);
        public decimal TotalPnl => Assets.Sum(a => a.PnL);
        public decimal TotalValue => Cash + TotalAssetsValue;
        public ViewModel()
        {
            this.Assets = new BindingList<Asset>()
            {
                new Asset("AAPL", AssetType.Stock, 10),
                new Asset("BTC", AssetType.Crypto, 1),
                new Asset("SPY", AssetType.ETF, 5)
            };
            Cash = new decimal(3000);
        }
        public void SimulateDay()
        {
            foreach (var asset in Assets)
            {
                asset.Simulate();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalAssetsValue)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalPnl)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalValue)));
        }
    }
}
