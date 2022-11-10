using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherSpiderSolution.Models;
using WeatherSpiderSolution.ReadModels;
using System.Net;
using System.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherSpiderSolution.Controllers;

public class WeatherController : Controller
{
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(ILogger<WeatherController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {

        return View();
    }

    [HttpGet]
    public IActionResult Weather(string station="72502", string begindate="2021-01-01", string enddate="2021-12-31")
    {
        JToken dataToken = GetData();

        IEnumerable<JToken> children = dataToken.Children();
        foreach(var ch in children)
            Console.WriteLine(ch);

        var tavg = GetChilds("tavg");
        var tmin = GetChilds("tmin");
        var tmax = GetChilds("tmax");
        var prcp = GetChilds("prcp");
        var snow = GetChilds("snow");
        var wdir = GetChilds("wdir");
        var wspd = GetChilds("wspd");

        var date = new List<Label>();
        foreach (var ch in children)
            date.Add(new Label((string)ch["date"]));

        return Ok(CreateWeatherDataRm());


        JToken GetData()
        {
            var url = $"https://meteostat.p.rapidapi.com/stations/monthly?station={station}&start={begindate}&end={enddate}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            request.Headers["X-RapidAPI-Key"] = "0f5e3ec321mshe20a41707387a8ap1828b9jsne2988a5e53ba";
            request.Headers["X-RapidAPI-Host"] = "meteostat.p.rapidapi.com";

            using var webResponse = request.GetResponse();

            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);

            var data = reader.ReadToEnd();

            JObject json = JObject.Parse(data);

            return json["data"];
        }

        List<Data> GetChilds(string childName)
        {
            List<Data> child = new List<Data>();
            foreach (var ch in children)
            {
                Console.WriteLine($"{childName} Child: " + ch[childName]);
                if (ch != null) 
                    child.Add(new Data((double)ch[childName]));
            }
            return child;
        }

        WeatherDataRm CreateWeatherDataRm()
        {
            return new WeatherDataRm(
                chart: new RadarChart(
                    caption: "Weather in New York",
                    subCaption: "Based on data collected last year",
                    numberPrefix: "",
                    theme: "fusion",
                    radarfillcolor: "#ffffff"
                    ),

                categories: new List<Category> {
                    new Category(
                        date
                        ),
                },

                dataset: new List<DataSet> {
                    new DataSet(
                        seriesname: "Temperature average",
                        tavg
                        ),
                    new DataSet(
                        seriesname: "Minimal temperature",
                        tmin
                        ),
                    new DataSet(
                        seriesname: "Maximum temperature",
                        tmax
                        )
                }

                );
        }

        

        
    }

    /*
    [HttpGet]
    public IActionResult Weather()
    {
        var url = "https://meteostat.p.rapidapi.com/stations/monthly?station=72502&start=2021-01-01&end=2021-12-31";

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        request.Headers["X-RapidAPI-Key"] = "0f5e3ec321mshe20a41707387a8ap1828b9jsne2988a5e53ba";
        request.Headers["X-RapidAPI-Host"] = "meteostat.p.rapidapi.com";

        using var webResponse = request.GetResponse();

        using var webStream = webResponse.GetResponseStream();

        using var reader = new StreamReader(webStream);
        var data = reader.ReadToEnd();


        JObject json = JObject.Parse(data);
        JToken dataToken = json["data"];

        IEnumerable<JToken> year = dataToken.Children();

        List<Data> dataForRadarChart = new List<Data>();
        foreach (var month in year)
        {
            dataForRadarChart.Add(new Data((double)month["tavg"]));
        }

        List<Data> dataForRadarChartMinTemp = new List<Data>();
        foreach (var month in year)
        {
            dataForRadarChartMinTemp.Add(new Data((double)month["tmin"]));
        }

        var rm = new WeatherDataRm(
                chart: new RadarChart(
                    caption: "Weather in New York",
                    subCaption: "Based on data collected last year",
                    numberPrefix: "",
                    theme: "fusion",
                    radarfillcolor: "#ffffff"
                    ),
                categories: new List<Category> {
                new Category(
                        new List<Label> {
                            new Label("Jan"),
                            new Label("Feb"),
                            new Label("Mar"),
                            new Label("Apr"),
                            new Label("May"),
                            new Label("Jun"),
                            new Label("Jul"),
                            new Label("Jug"),
                            new Label("Sep"),
                            new Label("Oct"),
                            new Label("Nov"),
                            new Label("Dec")
                        }
                        ),
                },

                dataset: new List<DataSet> {
                    new DataSet(
                        seriesname: "Temperature average",
                        dataForRadarChart
                        ),
                    new DataSet(
                        seriesname: "Minimal temperature",
                        dataForRadarChartMinTemp
                        )
                }
                );

        return Ok(rm);

    }
    */

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
