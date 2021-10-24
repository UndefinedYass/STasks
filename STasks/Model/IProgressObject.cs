namespace STasks.Model
{
    public interface IProgressObject
    {
        int CompletedCount { get; }
        bool IsComplete { get; }
        bool IsComplex { get; }
        bool IsDetermined { get; }
        int Percent { get; }
        float Ratio { get; }
        int TotalCount { get; }

        void RequireCompletion(bool value);
    }
}