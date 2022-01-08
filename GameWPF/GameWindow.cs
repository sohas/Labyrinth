﻿using System;
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
        #region private fields

        private readonly int _mapHeight;
        private readonly int _mapWidth;

        private readonly Explore _explore;
        private readonly Button[,] _exploringElements;
        private readonly Grid _exploringGrid;
        private readonly StackPanel _exploringStackPanel;
        private readonly TabItem _exploringTab;

        private readonly Guess _guess;
        private readonly Button[,] _guessElements;
        private readonly Grid _guessGrid;
        private readonly StackPanel _guessStackPanel;
        private readonly TabItem _guessTab;

        private readonly TextBox _exploringKeysTextBox;
        private readonly TextBox _guessKeysTextBox;

        private int _holeRow;
        private int _holeColumn;
        private int _holeTargetRow;
        private int _holeTargetColumn;
        private bool _defineHoLe;

        private int _counter;

        private bool _win;

        #endregion

        #region ctor

        public GameWindow(Map basicMap)
        {
            //Items.Clear();
            Name = "gameWindow";

            _mapHeight = basicMap.Height;
            _mapWidth = basicMap.Width;

            _explore = new(basicMap);
            _exploringElements = new Button[(_mapHeight * 4) - 1, (_mapWidth * 4) - 1];
            FillElementsFromMapVisual(_explore.MapVisual, _exploringElements);
            _exploringGrid = FillGridFromElements(_exploringElements, BuildEmptyGrid((_mapHeight * 4) - 1, (_mapWidth * 4) - 1));
            _exploringTab = new() { Header = basicMap.Name };

            _exploringKeysTextBox = new()
            {
                Text = "→, ←, ↓, ↑ to move, * to guess",
                FontSize = MapParameters.cellSize * 3,
                Height = MapParameters.cellSize * 6,
                Width = Math.Max(350, _exploringGrid.Width),
                TextAlignment = TextAlignment.Center,
                FontStretch = FontStretches.Expanded,
                Background = MapParameters.unvisitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new Thickness(0, 0, 0, 0),
                IsReadOnly = true,
                Focusable = false,
            };

            _exploringStackPanel = new() { Width = _exploringKeysTextBox.Width, Height = _exploringGrid.Height + _exploringKeysTextBox.Height };
            _exploringTab.Content = _exploringStackPanel;
            _exploringStackPanel.Children.Add(_exploringKeysTextBox);
            _exploringStackPanel.Children.Add(_exploringGrid);
            Items.Add(_exploringTab);

            _guess = new(basicMap);
            _guessElements = new Button[(_mapHeight * 2) + 1, (_mapWidth * 2) + 1];
            FillElementsFromMapVisual(_guess.MapVisual, _guessElements, false, true);
            _guessGrid = FillGridFromElements(_guessElements, BuildEmptyGrid((_mapHeight * 2) + 1, (_mapWidth * 2) + 1));
            _guessTab = new() { Header = _guess.Map.Name };

            _guessKeysTextBox = new()
            {
                Text = "Click to change\nBackspase to explore",
                FontSize = MapParameters.cellSize * 2.5,
                Height = MapParameters.cellSize * 9,
                Width = Math.Max(200, _guessGrid.Width),
                TextAlignment = TextAlignment.Center,
                FontStretch = FontStretches.Expanded,
                Background = MapParameters.unvisitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new Thickness(0,0,0,0),
                IsReadOnly = true,
                Focusable = false,
            };

            _guessStackPanel = new() { Width = _guessKeysTextBox.Width, Height = _guessGrid.Height + _guessKeysTextBox.Height };
            _guessTab.Content = _guessStackPanel;
            _guessStackPanel.Children.Add(_guessKeysTextBox);
            _guessStackPanel.Children.Add(_guessGrid);

            _holeRow = -1;
            _holeColumn = -1;
            _holeTargetRow = -1;
            _holeTargetColumn = -1;
            _defineHoLe = false;

            Background = MapParameters.unvisitedColor;
        }

        #endregion

        #region private methods

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

        private MapSymbol GetNewSymbol(MapSymbol symbol) 
        {
            switch (symbol) 
            {
                case MapSymbol.WallAbsentVertical:
                    return MapSymbol.WallPresentVertical;
                case MapSymbol.WallPresentVertical:
                    return MapSymbol.DiodeLeft;
                case MapSymbol.DiodeLeft:
                    return MapSymbol.DiodeRight;
                case MapSymbol.DiodeRight:
                    return MapSymbol.WallAbsentVertical;
                case MapSymbol.WallAbsentHorizontal:
                    return MapSymbol.WallPresentHorizontal;
                case MapSymbol.WallPresentHorizontal:
                    return MapSymbol.DiodeUp;
                case MapSymbol.DiodeUp:
                    return MapSymbol.DiodeDown;
                case MapSymbol.DiodeDown:
                    return MapSymbol.WallAbsentHorizontal;
                case MapSymbol.Visited:
                    if (_defineHoLe)
                    {
                        return symbol;
                    }
                    else 
                    {
                        return MapSymbol.Hole;
                    }
                case MapSymbol.Hole:
                    return MapSymbol.Visited;
                default:
                    return symbol;
            }
        }

        private void ClickWall(object sender, RoutedEventArgs args)
        {
            if (_win) 
            {
                return;
            }

            Button button = sender as Button;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            if (_defineHoLe)
            {
                if (row % 2 == 1 && column % 2 == 1)
                {
                    _holeTargetRow = (row - 1) / 2;
                    _holeTargetColumn = (column - 1) / 2;
                    _guess.SetHoleTarget(_holeRow, _holeColumn, _holeTargetRow, _holeTargetColumn); 
                    _defineHoLe = false;

                    Background = MapParameters.unvisitedColor;
                    _guessKeysTextBox.Background = MapParameters.unvisitedColor;
                    _guessKeysTextBox.Text = "Click to change\nBackspase to explore";

                    if (_guess.Equity) 
                    {
                        Win();
                    }

                    return;
                }
                else 
                {
                    return;
                }
            }

            if (
                (row % 2 == 0 && column % 2 == 0) ||
                row == 0 ||
                row == _mapHeight * 2 ||
                column == 0 ||
                column == _mapWidth * 2 ||
                false
                )
            {
                return;
            }

            MapSymbol newSymbol = GetNewSymbol(_guess.MapVisual[row, column]);
            _guess.ChangeMapFromMapVisualSymbol(row, column, newSymbol);

            if (newSymbol == MapSymbol.Hole)
            {
                _defineHoLe = true;
                _holeRow = (row - 1) / 2;
                _holeColumn = (column - 1) / 2;

                Background = MapParameters.startColor;
                _guessKeysTextBox.Background = MapParameters.startColor;
                _guessKeysTextBox.Text = "Chose target cell\nfor the hole";
            }
            
            FillElementsFromMapVisual(_guess.MapVisual, _guessElements, true, false);

            if (!_defineHoLe && _guess.Equity)
            {
                Win();
            }
        }

        private void Win() 
        {
            Background = MapParameters.playerColor;
            _guessKeysTextBox.Background = MapParameters.playerColor;
            _guessKeysTextBox.Foreground = MapParameters.startColor;
            _guessKeysTextBox.Text = "YOU\nWIN!";
            _guessKeysTextBox.FontWeight = FontWeights.Heavy;
            _win = true;
        }

        private void UpdateButtonFromSymbol(MapSymbol symbol, Button button, bool isGuess = false)
        {
            button.Background = MapParameters.GetColor(symbol);
            button.Content = null;
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

            if (isGuess)
            {
                button.Click += ClickWall;
            }

            button.Focusable = false;
        }

        private void FillElementsFromMapVisual(MapSymbol[,] mapVisual, Button[,] elements, bool update = false, bool isGuess = false)
        {
            int height = mapVisual.GetLength(0);
            int width = mapVisual.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (!update)
                    {
                        elements[i, j] = new Button() { BorderThickness = new Thickness(0, 0, 0, 0) };
                    }

                    UpdateButtonFromSymbol(mapVisual[i, j], elements[i, j], isGuess);
                }
            }
        }

        private Grid FillGridFromElements(UIElement[,] elements, Grid grid)
        {
            int height = elements.GetLength(0);
            int width = elements.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    UIElement element = elements[i, j];
                    grid.Children.Add(element);
                    Grid.SetRow(element, i);
                    Grid.SetColumn(element, j);
                }
            }
            return grid;
        }

        private void TryDrawMap() 
        {
            if (!Items.Contains(_guessTab))
            {
                Items.Add(_guessTab);
            }

            SelectedItem = _guessTab;
        }

        #endregion

        #region internal methods

        internal void KeyAction(object sender, RoutedEventArgs eventArgs)
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
                case Key.Multiply:
                    TryDrawMap();
                    break;
                case Key.Back:
                    SelectedItem = _exploringTab;
                    break;
                default:
                    break;
            }

            List<Map> exploredMaps = _explore.ExploredMaps;

            if (exploredMaps.Count != _counter)
            {
                _counter++;

                Map lastMap = _explore.ExploredMaps.Last();
                MapSymbol[,] lastMapVisual = lastMap.GetVisual();
                Button[,] elements = new Button[(_mapHeight * 4) - 1, (_mapWidth * 4) - 1];
                FillElementsFromMapVisual(lastMapVisual, elements, false);

                TabItem newTab = new() { Header = lastMap.Name};
                newTab.Content = FillGridFromElements(elements, BuildEmptyGrid((_mapHeight * 4) - 1, (_mapWidth * 4) - 1));
                Items.Add(newTab);
                
                Items.Remove(_exploringTab);

                bool guessPresent = false;

                if (Items.Contains(_guessTab)) 
                {
                    Items.Remove(_guessTab);
                    guessPresent = true;
                }
                
                Items.Add(_exploringTab);
                
                if (guessPresent)
                {
                    Items.Add(_guessTab);
                }
                
                SelectedItem = _exploringTab;
            }

            FillElementsFromMapVisual(_explore.Map.GetVisual(), _exploringElements, true);
        }

        #endregion
    }
}
