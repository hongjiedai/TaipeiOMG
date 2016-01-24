using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaipeiOMG.Models;

namespace TaipeiOMG.Controllers
{
    public class EstimateTimeController : ApiController
    {
        public readonly string URL = "http://data.taipei/bus/EstiamteTime";
        private static Dictionary<string, List<BusInfo>> busInfos = new Dictionary<string, List<BusInfo>>();
        public List<EstimateTimeModel> Get(string id)
        {
            MemoryStream uncompressed = Utilities.GetUnzipDataStream(URL);
            string jsonText = null;
            using (StreamReader sr = new StreamReader(uncompressed))
            {
                jsonText = sr.ReadToEnd();
            }
            JObject json = JObject.Parse(jsonText);
            IList<JToken> jsonInfos = json["BusInfo"].Children().ToList();

            foreach (JToken jsonInfo in jsonInfos)
            {
                BusInfo bus = JsonConvert.DeserializeObject<BusInfo>(jsonInfo.ToString());
                if (!busInfos.ContainsKey(bus.RouteID))
                {
                    busInfos.Add(bus.RouteID, new List<BusInfo>());
                }
                busInfos[bus.RouteID].Add(bus);
            }

            string routeId = RouteController.GetBusRouteId(id);
            List<EstimateTimeModel> results = new List<EstimateTimeModel>();
            if (routeId != null && busInfos.ContainsKey(routeId))
            {
                foreach (var info in busInfos[routeId])
                {
                    string zhName = info.StopID;
                    Stop stop = StopController.GetStopName(info.StopID);
                    if (stop != null)
                    {
                        zhName = stop.NameZh;
                    }
                    EstimateTimeModel model = new EstimateTimeModel(zhName, info.EstimateTime, info.GoBack);
                    results.Add(model);
                }
                return results;
            }
            return results;
        }

        public List<EstimateTimeModel> Get(string id, string goBack)
        {
            List<EstimateTimeModel> all = Get(id);
            return all.Where<EstimateTimeModel>(model => model.GoBack.Equals(goBack)).ToList<EstimateTimeModel>();
        }
        
        public class BusInfo
        {
            public string StopID { get; set; }
            public string GoBack { get; set; }
            public string RouteID { get; set; }
            public string EstimateTime { get; set; }
        }
    }
}
