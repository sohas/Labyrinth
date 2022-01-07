using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Game;


namespace GameWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string directory = Environment.CurrentDirectory + "\\Maps\\";
            string mapName = $"maze{1}.txt";
            Map map = new(directory + mapName);
            GameWindow gameWindow = new(map);
            Content = gameWindow;
            AddHandler(KeyDownEvent, new RoutedEventHandler(gameWindow.KeyAction));
            InitializeComponent();
        }
    }
}
