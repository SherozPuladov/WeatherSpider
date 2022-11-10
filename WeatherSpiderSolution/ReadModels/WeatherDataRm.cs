namespace WeatherSpiderSolution.ReadModels
{
    public record Label(string label);

    public record Data(double value);

    public record DataSet(string seriesname, List<Data> data);

    public record Category(List<Label> category);

    public record LineChart(
            string caption,
            string subcaption,
            string yaxisname,
            string xaxisname,
            string forceaxislimits,
            string pixelsperpoint,
            string pixelsperlabel,
            string compactdatamode,
            string dataseparator,
            string theme
            );

    public record RadarChart(
            string caption,
            string subCaption,
            string numberPrefix,
            string theme,
            string radarfillcolor
            );


    public record WeatherDataRm(
            RadarChart chart,
            List<Category> categories,
            List<DataSet> dataset
            );
}
