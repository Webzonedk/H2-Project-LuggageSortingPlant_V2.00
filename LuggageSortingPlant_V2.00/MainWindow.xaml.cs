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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LuggageSortingPlant_V2._00
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         MainServer manager = new MainServer();

        public MainWindow()
        {
            InitializeComponent();
            manager = new MainServer();

            StartLuggageController();
        }

        private void StartLuggageController()
        {
        LuggageController luggageController = new LuggageController();
            luggageController.luggageCreated += OnLuggageCreated;
        }



        public void OnLuggageCreated(object sender, EventArgs e)//Event Listener
        {

            //EventListener
            if (e is LuggageEvent)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    lbl_luggageInQueue.Content = ((LuggageEvent)e).Count.ToString();
                }));
            }
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            manager.RunSimulation();
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
