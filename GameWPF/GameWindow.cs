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
    public partial class GameWindow : TabControl
    {
        private readonly Explore _explore;
        private readonly int _mvHeight;
        private readonly int _mvWidth;
        private Button[,] _elements;
        private int _counter;
        private readonly TabItem _mainTab;

        public GameWindow(Map basicMap)
        {
            Background = MapParameters.unvisitedColor;
            _mainTab = new();
            Items.Add(_mainTab);
            _mainTab.Header = basicMap.Name;
            _explore = new(basicMap);
            MapSymbol[,] mapVisual = _explore.Map.GetVisual();
            _mvHeight = mapVisual.GetLength(0);
            _mvWidth = mapVisual.GetLength(1);
            _elements = new Button[_mvHeight, _mvWidth];
            FillElementsFromMapVisual(mapVisual, ref _elements);
            Grid grid = FillGridFromElements(_elements, BuildEmptyGrid(_mvHeight, _mvWidth));
            _mainTab.Content = grid;
        }

        private static Grid BuildEmptyGrid(int mvHeight, int mvWidth)
        {
            int wallSize = MapParameters.wallSize;
            int cellSize = MapParameters.cellSize;

            Grid mapGrid = new();
            mapGrid.ShowGridLines = false;
            mapGrid.Height = 2 * ((mvHeight * (cellSize + wallSize)) + wallSize);
            mapGrid.Width = 2 * ((mvWidth * (cellSize + wallSize)) + wallSize);

            GridLength wallHeight = new(wallSize, GridUnitType.Star);
            GridLength cellWidth = new(cellSize, GridUnitType.Star);

            ColumnDefinitionCollection columns = mapGrid.ColumnDefinitions;
            RowDefinitionCollection rows = mapGrid.RowDefinitions;

            for (int i = 0; i < mvHeight; i++)
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

            return mapGrid;
        }

        private void FillElementsFromMapVisual(MapSymbol[,] mapVisual, ref Button[,] elements, bool update = false)
        {
            for (int i = 0; i < _mvHeight; i++)
            {
                for (int j = 0; j < _mvWidth; j++)
                {
                    if (!update)
                    {
                        elements[i, j] = new Button() { BorderThickness = new Thickness(0, 0, 0, 0)};
                    }

                    UpdateButtonFromSymbol(mapVisual[i, j], ref elements[i, j]);
                }
            }
        }

        private static void UpdateButtonFromSymbol(MapSymbol symbol, ref Button button)
        {
            button.Background = MapParameters.GetColor(symbol);

            Ellipse ellipse = new();

            if (symbol == MapSymbol.Player)
            {
                ellipse.Width = MapParameters.cellSize;
                ellipse.Height = MapParameters.cellSize * 2;
                ellipse.Fill = MapParameters.playerColor;
                button.Content = ellipse;
            }
            else if (symbol == MapSymbol.Hole)
            {
                ellipse.Width = MapParameters.cellSize * 1.75;
                ellipse.Height = MapParameters.cellSize * 1.75;
                ellipse.Fill = MapParameters.wallColor;
                button.Content = ellipse;
            }
            else if (symbol == MapSymbol.Start)
            {
                ellipse.Width = MapParameters.cellSize * 0.5;
                ellipse.Height = MapParameters.cellSize * 0.5;
                ellipse.Fill = MapParameters.startColor;
                button.Content = ellipse;
            }
            else if (symbol == MapSymbol.DiodeLeft)
            {
                ellipse.Width = MapParameters.wallSize * 6;
                ellipse.Height = MapParameters.cellSize * 2;
                ellipse.Fill = MapParameters.visitedColor;
                ellipse.HorizontalAlignment = HorizontalAlignment.Left;
                button.Content = ellipse;
            }
            else if (symbol == MapSymbol.DiodeRight)
            {
                ellipse.Width = MapParameters.wallSize * 6;
                ellipse.Height = MapParameters.cellSize * 2;
                ellipse.Fill = MapParameters.visitedColor;
                ellipse.HorizontalAlignment = HorizontalAlignment.Right;
                button.Content = ellipse;
            }
            else if (symbol == MapSymbol.DiodeUp)
            {
                ellipse.Width = MapParameters.cellSize * 2;
                ellipse.Height = MapParameters.wallSize * 6;
                ellipse.Fill = MapParameters.visitedColor;
                ellipse.VerticalAlignment = VerticalAlignment.Top;
                button.Content = ellipse;
            }
            else if (symbol == MapSymbol.DiodeDown)
            {
                ellipse.Width = MapParameters.cellSize * 2;
                ellipse.Height = MapParameters.wallSize * 6;
                ellipse.Fill = MapParameters.visitedColor;
                ellipse.VerticalAlignment = VerticalAlignment.Bottom;
                button.Content = ellipse;
            }
            else
            {
                button.Content = null;
            }
        }

        private Grid FillGridFromElements(UIElement[,] elements, Grid grid)
        {
            for (int i = 0; i < _mvHeight; i++)
            {
                for (int j = 0; j < _mvWidth; j++)
                {
                    UIElement element = elements[i, j];
                    grid.Children.Add(element);
                    Grid.SetRow(element, i);
                    Grid.SetColumn(element, j);
                }
            }
            return grid;
        }

        public void ArrowAction(object sender, RoutedEventArgs eventArgs)
        {
            switch (((KeyEventArgs)eventArgs).Key)
            {
                case Key.Left:
                    _explore.Step(Direction.Left);
                    break;
                case Key.Right:
                    _explore.Step(Direction.Right);
                    break;
                case Key.Up:
                    _explore.Step(Direction.Up);
                    break;
                case Key.Down:
                    _explore.Step(Direction.Down);
                    break;
                default:
                    break;
            }

            List<Map> exploredMaps = _explore.ExploredMaps;

            if (exploredMaps.Count != _counter)
            {
                _counter++;
                Map lastMap = _explore.ExploredMaps.Last();
                TabItem newTab = new() { Header = lastMap.Name };
                Items.Add(newTab);
                MapSymbol[,] lastMapVisual = lastMap.GetVisual();
                Button[,] elements = new Button[_mvHeight, _mvWidth];
                FillElementsFromMapVisual(lastMapVisual, ref elements, false);
                newTab.Content = FillGridFromElements(elements, BuildEmptyGrid(_mvHeight, _mvWidth));
            }

            FillElementsFromMapVisual(_explore.Map.GetVisual(), ref _elements, true);
        }
    }
}
