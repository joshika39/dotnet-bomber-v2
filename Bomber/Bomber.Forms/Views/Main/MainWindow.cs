using Bomber.Forms.Map;
using Bomber.Game;

namespace Bomber.Forms.Views.Main
{
    public sealed partial class MainWindow : Form, IMainWindow
    {
        public IMainWindowPresenter Presenter { get; }

        public MainWindow(IMainWindowPresenter presenter)
        {
            Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            var mapControl = new GameMapView();
            Controls.Add(mapControl);
        }
    }
}
