using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Game;

namespace GameWPF
{
    public partial class MainWindow : Window
    {
        #region private fields

        private const string MapDir = "Maps";
        private const int MapMinSize = 2;
        private const int MapMaxSize = 20;
        private readonly Grid _mainGrid;
        private readonly Menu _mainMenu;
        private readonly MenuItem _mapsToPlay;
        private ConstructWindow _constructWindow;
        private string _constructMapName;
        private int _constructMapHeight;
        private int _constructMapWidth;

        #endregion

        #region ctor

        public MainWindow()
        {
            Background = MapParameters.visitedColor;
            _mainGrid = new();
            Content = _mainGrid;
            _mainGrid.ShowGridLines = false;
            RowDefinitionCollection rows = _mainGrid.RowDefinitions;
            rows.Add(new RowDefinition() 
            { 
                Height = new(38, GridUnitType.Auto) 
            }) ;
            rows.Add(new RowDefinition());

            _mainMenu = new() 
            { 
                Height = 38, 
                VerticalAlignment = VerticalAlignment.Top, 
                FontSize = 17, 
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
                BorderBrush = MapParameters.visitedColor,
                Margin = new(0, 0, 0, 0),
                Padding = new(0, 0, 0, 0),
            };
            _mainGrid.Children.Add(_mainMenu);
            Grid.SetRow(_mainMenu, 0);
            Grid.SetColumn(_mainMenu, 0);

            _mapsToPlay = new()
            {
                Header = "Play",
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
                BorderBrush = MapParameters.visitedColor,
                Margin = new(4, 4, 4, 4),
                Padding = new(4, 4, 4, 4),
            };
            _mainMenu.Items.Add(_mapsToPlay);

            string[] files = Directory.GetFiles(MapDir);
            List<string> maps = files.Select(x => x[(MapDir.Length + 1)..]).ToList();

            foreach (string map in maps)
            {
                MenuItem mapToPlay = new()
                { 
                    Header = map, 
                    Name = map, 
                    Background = MapParameters.visitedColor,
                    Foreground = MapParameters.alertColor,
                    BorderThickness = new(0, 0, 0, 0),
                    BorderBrush = MapParameters.visitedColor,
                    Margin = new(0, 0, 0, 0),
                    Padding = new(4, 4, 4, 4),
                };
                mapToPlay.Click += ChooseMap;
                _mapsToPlay.Items.Add(mapToPlay);
            }

            MenuItem construct = new() 
            { 
                Header = "Construct",
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
                BorderBrush = MapParameters.visitedColor,
                Margin = new(4, 4, 4, 4),
                Padding = new(4, 4, 4, 4),
            };

            _mainMenu.Items.Add(construct);

            MenuItem newMap = new()
            {
                Header = "New map",
                Name = "NewMap",
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
                BorderBrush = MapParameters.visitedColor,
                Margin = new(0, 0, 0, 0),
                Padding = new(4, 4, 4, 4),
            };

            newMap.Click += InputMapName;
            construct.Items.Add(newMap);

            MenuItem saveMap = new()
            {
                Header = "Save map",
                Name = "SaveMap",
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
                BorderBrush = MapParameters.visitedColor,
                Margin = new(0, 0, 0, 0),
                Padding = new(4, 4, 4, 4),
            };

            saveMap.Click += SaveMap;
            construct.Items.Add(saveMap);

            MenuItem credits = new()
            {
                Header = "Credits",
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
                BorderBrush = MapParameters.visitedColor,
                Margin = new(4, 4, 4, 4),
                Padding = new(4, 4, 4, 4),
            };

            _mainMenu.Items.Add(credits);

            MenuItem info = new()
            {
                StaysOpenOnClick = true,
                Header = "©2022\nVadim Sakhanenko\nsv1311@gmail.com",
                FontSize = 12,
                Name = "Info",
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
                BorderBrush = MapParameters.visitedColor,
                Margin = new(0, 0, 0, 0),
                Padding = new(4, 4, 4, 4),
            };
            
            credits.Items.Add(info);

            TextBlock intro = new()
            {
                Width = 400,
                TextWrapping = TextWrapping.Wrap,

                Text = "It is the Labyrinth game.\n\n" +
                "You can choose one of many labyrinths and try to explore it. " +
                "There are some cells, walls, holes and diodes in every labyrinth. " +
                "Diode let pass only one way. You can walk with keyboard arrows. " +
                "When you get hole, it transfers you to another certain cell in the labyrinth. " +
                "Your journey saves on the new tab after every hole getting. " +
                "You can try drawing labyrinth when you have explored enough.\n\n" +
                "Also you can make your own labyrinth and save it.\n\n" +
                "Welcome!",

                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Justify,
                FontSize = 17,
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
                Margin = new(10, 10, 10, 10),
                Padding = new(10, 10, 10, 10),
            };

            _mainGrid.Children.Add(intro);
            Grid.SetRow(intro, 1);
            Grid.SetColumn(intro, 0);

            KeyDown += (o, a) =>
            {
                if (a.Key == Key.Escape)
                {
                    if (_mainGrid.Children.Count == 2)
                    {
                        _mainGrid.Children.RemoveAt(1);
                    }
                    _mainGrid.Children.Add(intro);
                    Grid.SetRow(intro, 1);
                    Grid.SetColumn(intro, 0);
                }
            };
        }

        #endregion

        #region private methods

        private static void Pack(Map map, string filename)
        {
            BinaryFormatter formatter = new();
            using (FileStream fs = new(@"Maps/" + filename, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, map);
            }
        }

        private static Map UnPack(string filename)
        {
            BinaryFormatter formatter = new();

            using (FileStream fs = new(@"Maps/" + filename, FileMode.OpenOrCreate))
            {
                return (Map)formatter.Deserialize(fs);
            }
        }

        private void ChooseMap(object sender, RoutedEventArgs e) 
        {
            _constructWindow = null;
            MenuItem mi = sender as MenuItem;
            Map map = UnPack((string)mi.Header);
            GameWindow gameWindow = new(map);
            if (_mainGrid.Children.Count == 2)
            {
                _mainGrid.Children.RemoveAt(1);
            }
            _mainGrid.Children.Add(gameWindow);
            Grid.SetRow(gameWindow, 1);
            Grid.SetColumn(gameWindow, 0);

            KeyDown += gameWindow.KeyAction;
        }

        private void InputMapName(object sender, RoutedEventArgs e)
        {
            TextBlock askNameBlock = new()
            {
                Text = "Input map name:",
                Padding = new(4, 4, 4, 4),
                Width = 200,
                Height = 40,
                TextAlignment = TextAlignment.Justify,
                FontSize = 17,
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
            };

            TextBox inputNameBox = new()
            {
                Padding = new(4, 4, 4, 4),
                Width = askNameBlock.Width,
                Height = askNameBlock.Height,
                TextAlignment = TextAlignment.Justify,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 17,
                Background = MapParameters.alertColor,
                BorderThickness = new(0, 0, 0, 0),
            };

            inputNameBox.KeyDown += (o, a) =>
            {
                if (a.Key == Key.Return)
                {
                    string message = CheckMapName(inputNameBox.Text);
                    if (message != "")
                    {
                        MessageBox.Show(message);
                    }
                    else
                    {
                        _constructMapName = inputNameBox.Text;
                        ChooseMapSize();
                    }
                }
            };

            StackPanel askPanel = new() 
            {
                Width = inputNameBox.Width,
                Height = askNameBlock.Height + inputNameBox.Height,
            };
            askPanel.Children.Add(askNameBlock);
            askPanel.Children.Add(inputNameBox);

            if (_mainGrid.Children.Count == 2)
            {
                _mainGrid.Children.RemoveAt(1);
            }

            _mainGrid.Children.Add(askPanel);
            Grid.SetRow(askPanel, 1);
            Grid.SetColumn(askPanel, 0);

            inputNameBox.Focus();

        }

        private static string CheckMapName(string mapName)
        {
            if (!mapName.All(x => char.IsLetterOrDigit(x)))
            {
                return "All symbols must be letters or digits.";
            }
            if (mapName.Length < 4 || mapName.Length > 20)
            {
                return "Length of mapname must be > 3 and < 21.";
            }

            string[] files = Directory.GetFiles(MapDir);
            List<string> maps = files.Select(x => x[(MapDir.Length + 1)..]).ToList();

            if (maps.Any(x => x == mapName))
            {
                return "There is a map with the same name. Choose another name for the map.";
            }

            return "";
        }

        private void ChooseMapSize() 
        {
            Grid chooseMapSizeGrid = new()
            {
                Height = 2 * (MapMaxSize - MapMinSize + 1) * MapParameters.cellSize,
                Width = 2 * (MapMaxSize - MapMinSize + 1) * MapParameters.cellSize,
                Background = MapParameters.unvisitedColor,
                ShowGridLines = false,
            };

            GridLength cellWidth = new(MapParameters.cellSize, GridUnitType.Star);

            ColumnDefinitionCollection columns = chooseMapSizeGrid.ColumnDefinitions;
            RowDefinitionCollection rows = chooseMapSizeGrid.RowDefinitions;

            for (int i = 0; i < (MapMaxSize - MapMinSize + 1); i++)
            {
                rows.Add(new RowDefinition() { Height = cellWidth });
                columns.Add(new ColumnDefinition() { Width = cellWidth });
            }

            TextBlock chooseMapSizeBlock = new()
            {
                Text = "choose map size by mouse",
                Padding = new(4, 4, 4, 4),
                Width = chooseMapSizeGrid.Width,
                Height = 40,
                TextAlignment = TextAlignment.Justify,
                FontSize = 17,
                Background = MapParameters.visitedColor,
                Foreground = MapParameters.alertColor,
            };

            for (int i = 0; i < (MapMaxSize - MapMinSize + 1); i++) 
            {
                for (int j = 0; j < (MapMaxSize - MapMinSize + 1); j++) 
                {
                    Button button = new() 
                    {
                        Background = MapParameters.unvisitedColor,
                        BorderThickness = new(1, 1, 1, 1),
                        BorderBrush = MapParameters.visitedColor,
                    };

                    button.MouseEnter += (o, a) =>
                    {
                        Button button = o as Button;
                        _constructMapHeight = Grid.GetRow(button) + MapMinSize;
                        _constructMapWidth = Grid.GetColumn(button) + MapMinSize;
                        chooseMapSizeBlock.Text = $"map size: {_constructMapHeight} x {_constructMapWidth}. click to choose";
                    };

                    button.Click += CooseMapSizeButtonClick;

                    chooseMapSizeGrid.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                }
            }

            StackPanel chooseMapSizePanel = new()
            {
                Height = chooseMapSizeGrid.Height + chooseMapSizeBlock.Height,
                Width = chooseMapSizeGrid.Width,
            };

            chooseMapSizePanel.Children.Add(chooseMapSizeBlock);
            chooseMapSizePanel.Children.Add(chooseMapSizeGrid);

            if (_mainGrid.Children.Count == 2)
            {
                _mainGrid.Children.RemoveAt(1);
            }

            _mainGrid.Children.Add(chooseMapSizePanel);
            Grid.SetRow(chooseMapSizePanel, 1);
            Grid.SetColumn(chooseMapSizePanel, 0);
        }

        private void CooseMapSizeButtonClick(object sender, RoutedEventArgs args)
        {
            ConstructMap();
        }

        private void ConstructMap() 
        {
            if (_mainGrid.Children.Count == 2)
            {
                _mainGrid.Children.RemoveAt(1);
            }

            _constructWindow = new(_constructMapHeight, _constructMapWidth, _constructMapName);
            _mainGrid.Children.Add(_constructWindow);
            Grid.SetRow(_constructWindow, 1);
            Grid.SetColumn(_constructWindow, 0);
        }

        private void SaveMap(object sender, RoutedEventArgs e)
        {
            if (_constructWindow != null)
            {
                Map constructMap = _constructWindow.GuessMap;
                string constructMapName = constructMap.Name;

                if (!constructMap.IsReachable) 
                {
                    MessageBox.Show("Not all cells are reachable. Change map!");
                    return;
                }

                Random rnd = new();

                int playerRow = rnd.Next(constructMap.Height);
                int playerColumn = rnd.Next(constructMap.Width);
                Cell cell = constructMap.Cells[playerRow, playerColumn];
                constructMap.MarkStart(playerRow, playerColumn);
                constructMap.TakePlayer(new(), playerRow, playerColumn);

                string[] files = Directory.GetFiles(MapDir);
                List<string> maps = files.Select(x => x[(MapDir.Length + 1)..]).ToList();

                Pack(constructMap, constructMapName);

                _mainGrid.Children.RemoveAt(1);
                _constructWindow = null;

                if (maps.Contains(constructMapName))
                {
                    return;
                }

                MenuItem newMap = new()
                {
                    Header = constructMapName,
                    Name = constructMapName,
                    Background = MapParameters.visitedColor,
                    Foreground = MapParameters.alertColor,
                    BorderThickness = new(0, 0, 0, 0),
                    BorderBrush = MapParameters.visitedColor,
                    Margin = new(0, 0, 0, 0),
                    Padding = new(4, 4, 4, 4),
                };
                newMap.Click += ChooseMap;
                _mapsToPlay.Items.Add(newMap);
            }
        }

        #endregion
    }
}
