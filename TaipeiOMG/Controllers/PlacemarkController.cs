using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaipeiOMG.Controllers
{
    public class PlacemarkController : Controller
    {
        private static IList<Placemark> placemarks;
        public static IList<Placemark> GetPlacemarks(string fname)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}", fname));
            string jsonText = System.IO.File.ReadAllText(path).Replace("@id", "atId");
            JObject json = JObject.Parse(jsonText);
            IList<JToken> results = json["kml"]["Folder"]["Placemark"].Children().ToList();
            IList<Placemark> placemarks = new List<Placemark>();
            foreach (JToken result in results)
            {
                Placemark placemark = JsonConvert.DeserializeObject<Placemark>(result.ToString());
                string[] coordinates = placemark.LineString.Coordinates.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var coordinate in coordinates)
                {
                    string[] cs = coordinate.Split(',');
                    GeoCoordinate gc = new GeoCoordinate(Double.Parse(cs[1]), Double.Parse(cs[0]));
                    placemark.Coordinates.Add(gc);
                }
                placemarks.Add(placemark);
            }

            return placemarks;
        }

        // GET: Placemark
        public ActionResult Index()
        {
            if (placemarks == null)
            {
                placemarks = GetPlacemarks("GetRouteGeom");
            }

            return View();
        }
    }
}