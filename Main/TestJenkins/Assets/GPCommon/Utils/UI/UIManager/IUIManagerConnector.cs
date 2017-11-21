namespace GPCommon
{
	public interface IUIManagerConnector
	{
		IUIElement CreateUI(string className);
		void Show(IUIElement ui);
		void Hide(IUIElement ui);
	}
}
