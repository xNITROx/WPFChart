namespace ChartControls.Contracts
{
    public interface ISeriesData
    {
        long ValueX { get; }
        double[] Data { get; }
        string[] Labels { get; }
    }
}