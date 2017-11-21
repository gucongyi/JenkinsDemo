namespace GPCommon
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}