using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaipeiOMG.Models
{
    public class EstimateTimeModel
    {
        public EstimateTimeModel() { }
        public EstimateTimeModel(string zhName, string estimateTime, string goBack)
        {
            EstimateTime = estimateTime;
            GoBack = goBack;
            StopNameZh = zhName;
        }
        public string StopNameZh { get; set; }
        public string GoBack { get; set; }
        public string EstimateTime { get; set; }
    }
}