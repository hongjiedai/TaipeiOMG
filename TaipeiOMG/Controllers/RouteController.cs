using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace TaipeiOMG.Controllers
{
    [EnableCors(origins: "http://taipeiomg.azurewebsites.net/", headers: "*", methods: "*")]
    public class RouteController : ApiController
    {
        private static Dictionary<string, string> busName2RouteId;
        private static Dictionary<string, List<BusInfo>> busInfos;
        public static string GetBusRouteId(string name)
        {
            if (busName2RouteId.ContainsKey(name))
            {
                return busName2RouteId[name];
            }
            return null;
        }

        // GET api/Route
        static RouteController()
        {
            busInfos = new Dictionary<string, List<BusInfo>>();
            busName2RouteId = new Dictionary<string, string>();
            string path = System.Web.HttpContext.Current.Server.MapPath(string.Format("~/App_Data/GetRoute"));
            string jsonText = System.IO.File.ReadAllText(path);
            JObject json = JObject.Parse(jsonText);
            IList<JToken> results = json["BusInfo"].Children().ToList();
            
            foreach (JToken result in results)
            {
                BusInfo bus = JsonConvert.DeserializeObject<BusInfo>(result.ToString());
                if (!busInfos.ContainsKey(bus.NameZh))
                {
                    busInfos.Add(bus.NameZh, new List<BusInfo>());
                }
                busInfos[bus.NameZh].Add(bus);
                busName2RouteId[bus.NameZh] = bus.Id;
            }
        }
        public Controllers.BusInfo Get(string id)
        {
            Controllers.BusInfo busInfo = new Controllers.BusInfo();
            if (busInfos.ContainsKey(id))
            {
                busInfo.RoadMapUrl = busInfos[id].First<BusInfo>().RoadMapUrl;
                if (busInfos[id].Count == 1)
                {
                    busInfo.BackFirstBusTime = busInfos[id].First<BusInfo>().BackFirstBusTime;
                    busInfo.BackLastBusTime = busInfos[id].First<BusInfo>().BackLastBusTime;
                    busInfo.GoFirstBusTime = busInfos[id].First<BusInfo>().GoFirstBusTime;
                    busInfo.GoLastBusTime = busInfos[id].First<BusInfo>().GoLastBusTime;
                    busInfo.HolidayBackFirstBusTime = busInfos[id].First<BusInfo>().HolidayBackFirstBusTime;
                    busInfo.HolidayBackLastBusTime = busInfos[id].First<BusInfo>().HolidayBackLastBusTime;
                    busInfo.HolidayGoFirstBusTime = busInfos[id].First<BusInfo>().HolidayGoFirstBusTime;
                    busInfo.HolidayGoLastBustime = busInfos[id].First<BusInfo>().HolidayGoLastBustime;
                }
                else
                {
                    foreach (var bus in busInfos[id])
                    {
                        if (!IsTimeLessThan(busInfo.BackFirstBusTime, bus.BackFirstBusTime))
                        {
                            busInfo.BackFirstBusTime = bus.BackFirstBusTime;
                        }
                        if (!IsTimeLargerThan(busInfo.BackLastBusTime, bus.BackLastBusTime))
                        {
                            busInfo.BackLastBusTime = bus.BackLastBusTime;
                        }
                        if (!IsTimeLessThan(busInfo.GoFirstBusTime, bus.GoFirstBusTime))
                        {
                            busInfo.GoFirstBusTime = bus.GoFirstBusTime;
                        }
                        if (!IsTimeLargerThan(busInfo.GoLastBusTime, bus.GoLastBusTime))
                        {
                            busInfo.GoLastBusTime = bus.GoLastBusTime;
                        }
                        if (!IsTimeLessThan(busInfo.HolidayBackFirstBusTime, bus.HolidayBackFirstBusTime))
                        {
                            busInfo.HolidayBackFirstBusTime = bus.HolidayBackFirstBusTime;
                        }
                        if (!IsTimeLargerThan(busInfo.HolidayBackLastBusTime, bus.HolidayBackLastBusTime))
                        {
                            busInfo.HolidayBackLastBusTime = bus.HolidayBackLastBusTime;
                        }
                        if (!IsTimeLessThan(busInfo.HolidayGoFirstBusTime, bus.HolidayGoFirstBusTime))
                        {
                            busInfo.HolidayGoFirstBusTime = bus.HolidayGoFirstBusTime;
                        }
                        if (!IsTimeLargerThan(busInfo.HolidayGoLastBustime, bus.HolidayGoLastBustime))
                        {
                            busInfo.HolidayGoLastBustime = bus.HolidayGoLastBustime;
                        }
                    }
                }
            }
            return busInfo;
        }

        private bool IsTimeLargerThan(string t1, string t2)
        {
            if (t1 == null)
            {
                return false;
            }
            if (t2 == null)
            {
                return true;
            }
            if (t1.Equals(t2))
            {
                return false;
            }
            int h1 = Int32.Parse(t1.Substring(0, 2));
            int h2 = Int32.Parse(t2.Substring(0, 2));
            if (h1 > h2)
            {
                return true;
            }
            int m1 = Int32.Parse(t1.Substring(2, 2));
            int m2 = Int32.Parse(t2.Substring(2, 2));
            if (m1 > m2)
            {
                return true;
            }
            return false;
        }

        private bool IsTimeLessThan(string t1, string t2)
        {
            if (t1 == null)
            {
                return false;
            }
            if (t2 == null)
            {
                return true;
            }
            if (t1.Equals(t2))
            {
                return false;
            }
            int h1 = Int32.Parse(t1.Substring(0, 2));
            int h2 = Int32.Parse(t2.Substring(0, 2));
            if (h1 < h2)
            {
                return true;
            }
            int m1 = Int32.Parse(t1.Substring(2, 2));
            int m2 = Int32.Parse(t2.Substring(2, 2));
            if (m1 < m2)
            {
                return true;
            }
            return false;
        }

        public class BusInfo
        {
            public string Id { get; set; }

            public string ProviderId { get; set; }
            public string ProviderName { get; set; }
            public string NameZh { get; set; }
            public string NameEn { get; set; }

            public string PathAttributeId { get; set; }
            public string PathAttributeName { get; set; }
            public string PathAttributeEname { get; set; }
            public string BuildPeriod;
            public string DepartureZh;
            public string DepartureEn;
            public string DestinationZh;
            public string DestinationEn;
            public string RealSequence;
            public string Distance;
            public string GoFirstBusTime;
            public string BackFirstBusTime;
            public string GoLastBusTime;
            public string BackLastBusTime;
            public string PeakHeadway;
            public string OffPeakHeadway;
            public string BusTimeDesc;
            public string HeadwayDesc;
            public string HolidayBusTimeDesc;
            public string HolidayGoFirstBusTime;
            public string HolidayBackFirstBusTime;
            public string HolidayGoLastBustime;
            public string HolidayBackLastBusTime;
            public string HolidayPeakHeadway;
            public string HolidayOffPeakHeadway;
            public string HolidayHeadwayDesc;
            public string SegmentBufferZh;
            public string SegmentBufferEn;
            public string TicketPriceDescriptionZh;
            public string TicketPriceDescriptionEn;
            public string RoadMapUrl;
        }
    }
}
