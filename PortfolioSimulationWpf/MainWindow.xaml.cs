﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PortfolioSimulationWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            if(this.DataContext is ViewModel vm)
            {
                this.vm = vm;
            }
            else
            {
                this.vm = new ViewModel();
                this.DataContext = this.vm;
            }
        }

        private void SimulateClick(object sender, RoutedEventArgs e)
        {
            this.vm.SimulateDay();
        }
    }
}