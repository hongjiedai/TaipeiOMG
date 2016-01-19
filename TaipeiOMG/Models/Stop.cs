using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaipeiOMG.Models
{
    public class Stop
    {
        /// <summary>
        /// 站牌代碼
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 所屬路線代碼 (主路線 ID)
        /// </summary>
        public string RouteId { get; set; }

        /// <summary>
        /// 中文名稱
        /// </summary>
        public string NameZh { get; set;}
        /// <summary>
        /// 英文名稱
        /// </summary>
        public string NameEn { get; set; }
        /// <summary>
        /// 於路線上的順序
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 上下車站別 （-1：可下車、0：可上下車、1：可上車）
        /// </summary>
        public string Pgp { get; set; }
        /// <summary>
        /// 去返程 （0：去程 / 1：返程 / 2：未知）
        /// </summary>
        public string GoBack { get; set; }
        
        /// <summary>
        /// 經度、緯度
        /// </summary>
        //public GeoCoordinate Coordinate { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 站位 ID
        /// </summary>
        public string StopLocationId { get; set; }
        /// <summary>
        /// 顯示用經度、緯度
        /// </summary>
        //public GeoCoordinate ShowCoordinate { get; set; }
        /// <summary>
        /// 向量角：0~359，預設為空白
        /// </summary>
        public string Vector { get; set; }
        /// <summary>
        /// 經度
        /// </summary>
        public string Longitude {get; set;}
        /// <summary>
        /// 緯度
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 顯示用經度
        /// </summary>
        public string ShowLon { get; set; }
        /// <summary>
        /// 顯示用緯度
        /// </summary>
        public string ShowLat { get; set; }
    }
}