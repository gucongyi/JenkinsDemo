namespace GPCommon
{
    public enum UIState
    {
        Hidden,
        Show,
    }

    public interface IUIElement
    {
        UIState State { get; set; }

        void OnLoadComplete();

        void OnSetup(params object[] args);

        void PlayShownSound();

        void OnWillShow();

        void OnShown();

        void PlayHidenSound();

        void OnWillHiden();

        void OnHidden();
    }
}