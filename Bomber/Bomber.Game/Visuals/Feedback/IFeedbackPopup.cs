using GameFramework.Manager.State;

namespace Bomber.Game.Visuals.Feedback
{
    public interface IFeedbackPopup : IGameStateChangedListener
    {
        void DisplayError(string message, string title);
        void DisplaySuccess(string message, string title);
        void DisplayWarning(string message, string title);
        Task DisplayInfo(string message, string title);
    }
}
