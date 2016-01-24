using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using TaipeiOMG.Models;

namespace TaipeiOMG.Controllers
{
    public class EstimateTimeController : ApiController
    {
        static EstimateTimeController()
        {
            HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>)UpdateEstimateTime);
        }

        private static void UpdateEstimateTime(CancellationToken cancellationToken)
        {
            lock (busInfos)
            {
                if (busInfos.Count == 0)
                {
                    MemoryStream uncompressed = Utilities.GetUnzipDataStream(URL);
                    IList<JToken> jsonInfos = ParseJson(uncompressed);
                    uncompressed.Close();

                    foreach (JToken jsonInfo in jsonInfos)
                    {
                        BusInfo bus = JsonConvert.DeserializeObject<BusInfo>(jsonInfo.ToString());
                        if (!busInfos.ContainsKey(bus.RouteID))
                        {
                            busInfos.Add(bus.RouteID, new List<BusInfo>());
                        }
                        busInfos[bus.RouteID].Add(bus);
                    }                
                }
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(60000);
                MemoryStream uncompressed = Utilities.GetUnzipDataStream(URL);
                IList<JToken> jsonInfos = ParseJson(uncompressed);
                uncompressed.Close();

                lock (busInfos)
                {
                    busInfos.Clear();
                    foreach (JToken jsonInfo in jsonInfos)
                    {
                        BusInfo bus = JsonConvert.DeserializeObject<BusInfo>(jsonInfo.ToString());
                        if (!busInfos.ContainsKey(bus.RouteID))
                        {
                            busInfos.Add(bus.RouteID, new List<BusInfo>());
                        }
                        busInfos[bus.RouteID].Add(bus);
                    }
                }
            }            
        }

        private static IList<JToken> ParseJson(MemoryStream uncompressed)
        {
            string jsonText = null;
            using (StreamReader sr = new StreamReader(uncompressed))
            {
                jsonText = sr.ReadToEnd();
            }
            JObject json = JObject.Parse(jsonText);
            IList<JToken> jsonInfos = json["BusInfo"].Children().ToList();
            return jsonInfos;
        }
        public static readonly string URL = "http://data.taipei/bus/EstiamteTime";
        private volatile static Dictionary<string, List<BusInfo>> busInfos = new Dictionary<string, List<BusInfo>>();
        public List<EstimateTimeModel> Get(string id)
        {
            string routeId = RouteController.GetBusRouteId(id);
            List<EstimateTimeModel> results = new List<EstimateTimeModel>();
            lock (busInfos)
            {
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
