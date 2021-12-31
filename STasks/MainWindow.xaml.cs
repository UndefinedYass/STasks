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

namespace STasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = STasks.ViewModel.MainViewModel.Instance;
        }

        private void Explorer_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DockPanel my_dockPanel = sender as DockPanel;
            AdornerLayer al = AdornerLayer.GetAdornerLayer(my_dockPanel);
            al.Add(new View.Adorners.ResizeDockedGridAdorner(Explorer, View.Adorners.ResizeDockedGridAdornerType.left));
            al.Add(new View.Adorners.ResizeDockedGridAdorner(STATS, View.Adorners.ResizeDockedGridAdornerType.top));

        }


       
    }
}
