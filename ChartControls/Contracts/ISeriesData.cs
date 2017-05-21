namespace ChartControls.Contracts
{
    public interface ISeriesData
    {
        long ValueX { get; set; }
        double[] Data { get; }
        string[] Labels { get; }
    }
}