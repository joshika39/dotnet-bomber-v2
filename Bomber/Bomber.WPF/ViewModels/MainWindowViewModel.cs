using Bomber.Game;
using Bomber.WPF.Map;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using Bomber.Game.Visuals.Views;

namespace Bomber.WPF.ViewModels
{
    public partial class MainWindowViewModel : IMainWindowViewModel
    {
        private readonly Gameplay _gamePlay;
        private string _currentTime = "asdasd";

        public IGameMapView MapView { get; }

        public string CurrentTime
        {
            get => _currentTime;
            // private set => SetProperty(ref _currentTime, value);
        }

        public MainWindowViewModel()
        {
            var mapView = new GameMapView();
            _gamePlay = new Gameplay();
            MapView = mapView;
        }

        [RelayCommand]
        private void OnKeyDown(KeyEventArgs e)
        {
            _gamePlay.HandleKeyPress(GetCharFromKey(e.Key));

        }

        [RelayCommand]
        private void OnOpen()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "BoB files (*.bob)|*.bob",
                InitialDirectory = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "joshik39", "Bomber", "maps")
            };
            if (!openDialog.ShowDialog() ?? false)
            {
                return;
            }

            MapView.Clear();
            _gamePlay.OpenMap(openDialog.FileName, MapView);
        }

        [RelayCommand]
        private void OnSave()
        {
            _gamePlay.SaveGame();
        }

        private enum MapType : uint
        {
            MapvkVkToVsc = 0x0,
        }

        [DllImport("user32.dll")]
        private static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        private static char GetCharFromKey(Key key)
        {
            var ch = ' ';

            var virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            var scanCode = MapVirtualKey((uint)virtualKey, MapType.MapvkVkToVsc);
            var stringBuilder = new StringBuilder(2);

            var result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                {
                    ch = stringBuilder[0];
                    break;
                }
                default:
                {
                    ch = stringBuilder[0];
                    break;
                }
            }
            return ch;
        }
    }
}
