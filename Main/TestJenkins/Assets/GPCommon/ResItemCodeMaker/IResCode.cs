namespace GPCommon
{
    public interface IResCode
    {
        string ResRoot { get; }
        string GetResPath(string name);
    }
}