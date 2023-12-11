using Bomber.Game;
using Bomber.Game.Visuals.Views;
using Bomber.Maui.Map;
using CommunityToolkit.Mvvm.Input;

#nullable enable

namespace Bomber.Maui.ViewModels
{
    public partial class MainPageViewModel
    {
        private Gameplay? _gameplay;
        public IGameMapView MapControl { get; set; } = new GameMapView();

        [RelayCommand]
        private async Task OnOpenMap()
        {
            _gameplay = new Gameplay();
            
            var customFileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                { DevicePlatform.Android, new[] { ".bob" } },
                { DevicePlatform.iOS, new[] { ".bob" } },
                { DevicePlatform.macOS, new[] { ".bob"  } },
                { DevicePlatform.WinUI, new[] { ".bob" } },
                { DevicePlatform.Unknown, new[] {".bob"  } },
            };
            var fileTypes = new FilePickerFileType(customFileTypes);
            
            var options = new PickOptions
            {
                PickerTitle = "Select a map",
                FileTypes = fileTypes
            };
            
            var result = await FilePicker.Default.PickAsync(options);
            if (result is null)
            {
                return;
            }
            
            _gameplay.OpenMap(result.FullPath, MapControl);
        }

        [RelayCommand]
        private void OnSaveMap()
        {
            _gameplay?.SaveGame();
        }

        [RelayCommand]
        private void OnLeftButton()
        {
            _gameplay?.HandleKeyPress('a');
        }
        
        [RelayCommand]
        private void OnRightButton()
        {
            _gameplay?.HandleKeyPress('d');
        }
        
        [RelayCommand]
        private void OnUpButton()
        {
            _gameplay?.HandleKeyPress('w');
        }
        
        [RelayCommand]
        private void OnDownButton()
        {
            _gameplay?.HandleKeyPress('s');
        }
        
        [RelayCommand]
        private void OnBombButton()
        {
            _gameplay?.HandleKeyPress('b');
        }

        [RelayCommand]
        private void OnDetonateBomb()
        {
            _gameplay?.HandleKeyPress('1');
        }
        
        [RelayCommand]
        private void OnPauseButton()
        {
            
        }
    }
}
