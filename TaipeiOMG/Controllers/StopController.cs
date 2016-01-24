using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TaipeiOMG.Models;

namespace TaipeiOMG.Controllers
{
    public class StopController : ApiController
    {
        public readonly static string URL = "http://data.taipei/bus/Stop";
        private static Dictionary<string, Stop> stops;
        static StopController()
        {
            MemoryStream uncompressed = Utilities.GetUnzipDataStream(URL);
            uncompressed.Close();
            string jsonText = null;
            using (StreamReader sr = new StreamReader(uncompressed))
            {
                jsonText = sr.ReadToEnd();
            }
            JObject json = JObject.Parse(jsonText);
            IList<JToken> results = json["BusInfo"].Children().ToList();
            stops = new Dictionary<string, Stop>();
            foreach (JToken result in results)
            {
                Stop stop = JsonConvert.DeserializeObject<Stop>(result.ToString());
                //GeoCoordinate gc = new GeoCoordinate(Double.Parse(stop.Longitude), Double.Parse(stop.Latitude));
                //stop.Coordinate = gc;
                //gc = new GeoCoordinate(Double.Parse(stop.ShowLon), Double.Parse(stop.ShowLat));
                //stop.ShowCoordinate = gc;
                stops.Add(stop.Id, stop);
            }
        }

        public static Stop GetStopName(string stopId)
        {
            if (stops.ContainsKey(stopId))
            {
                return stops[stopId];
            }
            return null;
        }
        // GET: Stop
        public Stop Get(string id)
        {
            Stop stop = GetStopName(id);
            if (stop != null)
            {
                return stop;
            }
            return new Stop();
        }
    }
}