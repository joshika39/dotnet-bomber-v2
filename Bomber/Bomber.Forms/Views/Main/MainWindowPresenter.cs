using Bomber.Game.Game.Map;
using GameFramework.Board;

namespace Bomber.Forms.Views.Main
{
    internal class MainWindowPresenter : IMainWindowPresenter
    {
        private readonly IBoardService _boardService;

        public MainWindowPresenter(IBoardService boardService)
        {
            _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
        }

        public void SaveMap()
        {
            _boardService.GetActiveMap<GameMap>()?.SaveProgress();
        }
    }
}
