using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Game;


namespace GameWPF
{
    class ConstructWindow : DockPanel
    {
        #region private fields

        private readonly int _mapHeight;
        private readonly int _mapWidth;

        private readonly Guess _guess;
        private readonly Button[,] _guessElements;
        private readonly Grid _guessGrid;
        private readonly Canvas _guessCanvas;
        private readonly StackPanel _guessStackPanel;
        private readonly Dictionary<(int, int), Line> _holeLines;

        private readonly TextBox _guessKeysTextBox;

        private int _holeRow;
        private int _holeColumn;
        private int _holeTargetRow;
        private int _holeTargetColumn;
        private bool _defineHole;

        #endregion

        #region public properties

        public Map GuessMap => _guess.Map;

        #endregion

        #region ctor

        public ConstructWindow(int height, int width, string name)
        {
            Name = "constructWindow";
            LastChildFill = true;
            Background = MapParameters.unvisitedColor;

            _mapHeight = height;
            _mapWidth = width;

            _guess = new(height, width, name);
            _guessElements = new Button[(_mapHeight * 2) + 1, (_mapWidth * 2) + 1];
            FillElementsFromMapVisual(_guess.MapVisual, _guessElements, false, true);
            _guessGrid = FillGridFromElements(_guessElements, BuildEmptyGrid((_mapHeight * 2) + 1, (_mapWidth * 2) + 1));
            _guessCanvas = new()
            {
                IsHitTestVisible = false,
                Width = _guessGrid.Width,
                Height = _guessGrid.Height
            };
            _guessGrid.Children.Add(_guessCanvas);

            _guessKeysTextBox = new()
            {
                Text = "Click :edit\nBcksp :explore",
                FontSize = MapParameters.cellSize * 2.5,
                Height = MapParameters.cellSize * 9,
                Width = Math.Max(144, _guessGrid.Width),
                TextAlignment = TextAlignment.Center,
                FontStretch = FontStretches.Expanded,
                Background = MapParameters.unvisitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new Thickness(0, 0, 0, 0),
                IsReadOnly = true,
                Focusable = false,
            };

            _guessStackPanel = new()
            {
                Width = _guessKeysTextBox.Width,
                Height = _guessGrid.Height + _guessKeysTextBox.Height
            };
            _guessStackPanel.Children.Add(_guessKeysTextBox);
            _guessStackPanel.Children.Add(_guessGrid);

            Children.Add(_guessStackPanel);

            _holeLines = new();

            _holeRow = -1;
            _holeColumn = -1;
            _holeTargetRow = -1;
            _holeTargetColumn = -1;
            _defineHole = false;

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
                    if (_defineHole)
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

        private void ClickToChangeCell(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

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

            if (_defineHole)
            {
                if (row % 2 == 1 && column % 2 == 1)
                {
                    _holeTargetRow = (row - 1) / 2;
                    _holeTargetColumn = (column - 1) / 2;

                    if (_holeRow == _holeTargetRow && _holeColumn == _holeTargetColumn) 
                    {
                        return;
                    }

                    _guess.SetHoleTarget(_holeRow, _holeColumn, _holeTargetRow, _holeTargetColumn);
                    SetHoleLine(_holeRow, _holeColumn, _holeTargetRow, _holeTargetColumn);
                    _defineHole = false;

                    Background = MapParameters.unvisitedColor;
                    _guessKeysTextBox.Background = MapParameters.unvisitedColor;
                    _guessKeysTextBox.Text = "Click :edit\nBcksp :explore";

                    return;
                }
                else
                {
                    return;
                }
            }

            MapSymbol newSymbol = GetNewSymbol(_guess.MapVisual[row, column]);
            _guess.ChangeMapFromMapVisualSymbol(row, column, newSymbol);

            if (newSymbol == MapSymbol.Hole)
            {
                _defineHole = true;
                _holeRow = (row - 1) / 2;
                _holeColumn = (column - 1) / 2;

                Background = MapParameters.startColor;
                _guessKeysTextBox.Background = MapParameters.startColor;
                _guessKeysTextBox.Text = "Choose target\nfor the hole";
            }
            else if (newSymbol == MapSymbol.Visited)
            {
                _holeRow = (row - 1) / 2;
                _holeColumn = (column - 1) / 2;

                if (_holeLines.ContainsKey((_holeRow, _holeColumn)))
                {
                    Line line = _holeLines[(_holeRow, _holeColumn)];
                    _guessCanvas.Children.Remove(line);
                }
            }

            FillElementsFromMapVisual(_guess.MapVisual, _guessElements, true, false);
        }

        private void SetHoleLine(int holeRow, int holeColumn, int holeTargetRow, int holeTargetColumn)
        {
            Brush brush = MapParameters.holeLineColor.Clone();
            brush.Opacity = 0.25;
            Line line = new()
            {
                Focusable = false,
                Stroke = brush,
                StrokeThickness = MapParameters.cellSize * 1.75,
                StrokeDashArray = new DoubleCollection(new List<double>() { 0.25, 0.25 }),
            };
            double kx = _guessCanvas.Width / (_mapWidth * (MapParameters.cellSize + MapParameters.wallSize) + MapParameters.wallSize);
            double ky = _guessCanvas.Height / (_mapHeight * (MapParameters.cellSize + MapParameters.wallSize) + MapParameters.wallSize);
            double x1 = kx * (holeColumn * (MapParameters.cellSize + MapParameters.wallSize) + MapParameters.wallSize + MapParameters.cellSize / 2.0);
            double y1 = ky * (holeRow * (MapParameters.cellSize + MapParameters.wallSize) + MapParameters.wallSize + MapParameters.cellSize / 2.0);
            double x2 = kx * (holeTargetColumn * (MapParameters.cellSize + MapParameters.wallSize) + MapParameters.wallSize + MapParameters.cellSize / 2.0);
            double y2 = ky * (holeTargetRow * (MapParameters.cellSize + MapParameters.wallSize) + MapParameters.wallSize + MapParameters.cellSize / 2.0);

            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;

            _holeLines[(holeRow, holeColumn)] = line;

            _guessCanvas.Children.Add(line);
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
                button.Click += ClickToChangeCell;
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

        private static Grid FillGridFromElements(UIElement[,] elements, Grid grid)
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

        #endregion
    }
}
