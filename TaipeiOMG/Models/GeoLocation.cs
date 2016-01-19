using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TaipeiOMG.Models
{
    public class GeoLocation
    {
        public string LocationName { get; set; }
        public string City { get; set; }
        public Location Location { get; set; }
    }
}