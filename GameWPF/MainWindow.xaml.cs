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
using System.Text.Json;
using System.Text.Json.Serialization;

using Game;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameWPF
{
    public partial class MainWindow : Window
    {
        private Map _currentMap;
        private int _height;
        private int _width;
        private const string MapDir = "Maps";
        private readonly Grid _mainGrid;

        public MainWindow()
        {
            _mainGrid = new();
            Content = _mainGrid;
            _mainGrid.ShowGridLines = false;
            RowDefinitionCollection rows = _mainGrid.RowDefinitions;
            rows.Add(new RowDefinition() { Height = new(20, GridUnitType.Auto) }) ;
            rows.Add(new RowDefinition());

            Menu mainMenu = new() { Height = 20, VerticalAlignment = VerticalAlignment.Top, FontSize = 13 };
            _mainGrid.Children.Add(mainMenu);
            Grid.SetRow(mainMenu, 0);
            Grid.SetColumn(mainMenu, 0);

            MenuItem chooseMap = new() { Header = "Choose map to play" };
            mainMenu.Items.Add(chooseMap);

            string[] files = Directory.GetFiles(MapDir);
            List<string> maps = files.Select(x => x[(MapDir.Length + 1)..]).ToList();

            foreach (var map in maps)
            {
                MenuItem mi = new() { Header = map, Name = map };
                mi.Click += ChooseMapName;
                chooseMap.Items.Add(mi);
            }

            MenuItem construction = new() { Header = "Construction" };
            mainMenu.Items.Add(construction);
            
            InitializeComponent();
        }

        private void Puck(Map map, string filename)
        {
            BinaryFormatter formatter = new();
            using (FileStream fs = new(@"Maps/" + filename, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, map);
            }
        }

        private Map UnPuck(string filename)
        {
            BinaryFormatter formatter = new();

            using (FileStream fs = new(@"Maps/" + filename, FileMode.OpenOrCreate))
            {
                return (Map)formatter.Deserialize(fs);
            }
        }

        private void ChooseSize(object sender, RoutedEventArgs e)
        {

        }

        private void ChooseMapName(object sender, RoutedEventArgs e) 
        {
            MenuItem mi = sender as MenuItem;
            Map map = UnPuck((string)mi.Header);
            GameWindow gameWindow = new(map);
            if (_mainGrid.Children.Count == 2)
            {
                _mainGrid.Children.RemoveAt(1);
            }
            _mainGrid.Children.Add(gameWindow);
            Grid.SetRow(gameWindow, 1);
            Grid.SetColumn(gameWindow, 0);
            AddHandler(KeyDownEvent, new RoutedEventHandler(gameWindow.KeyAction));
        }

        private void Construct(object sender, RoutedEventArgs e)
        {

        }
    }
}
