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
using Game;

namespace GameWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Grid GetGameWindow(Map map)
        {
            Grid mapGrid = new();
            mapGrid.ShowGridLines = true;
            mapGrid.Height = 200;
            mapGrid.Width = 200;

            MapSymbol[,] mapVisual = map.GetVisual();
            int mvHigh = mapVisual.GetLength(0);
            int mvWidth = mapVisual.GetLength(1);

            GridLength wallHeight = new(1.0, GridUnitType.Star);
            GridLength cellWidth = new(10.0, GridUnitType.Star);
            
            ColumnDefinitionCollection columns = mapGrid.ColumnDefinitions;
            RowDefinitionCollection rows = mapGrid.RowDefinitions;

            for (int i = 0; i < mvHigh; i++) 
            {
                if (i % 2 == 0)
                {
                    rows.Add(new RowDefinition() { Height = wallHeight });
                }
                else
                {
                    rows.Add(new RowDefinition() { Height = cellWidth });
                }
            }
            for (int j = 0; j < mvWidth; j++)
            {
                if (j % 2 == 0)
                {
                    columns.Add(new ColumnDefinition() { Width = wallHeight });
                }
                else
                {
                    columns.Add(new ColumnDefinition() { Width = cellWidth });
                }
            }

            for (int i = 0; i < mvHigh; i++) 
            {
                for (int j = 0; j < mvWidth; j++) 
                {
                    var element = new TextBox() { Text = mapVisual[i, j].ToName() };
                    mapGrid.Children.Add(element);
                    Grid.SetRow(element, i);
                    Grid.SetColumn(element, j);
                }
            }


            return mapGrid;
        }


        public MainWindow()
        {
            string directory = Environment.CurrentDirectory + "\\Maps\\";
            string mapName = $"maze{1}.txt";
            Map map = new Map(directory + mapName);

            var gm = GetGameWindow(map);


            AddChild(gm);

            InitializeComponent();
        }
    }
}
