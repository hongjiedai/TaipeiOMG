using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;

namespace TaipeiOMG.Controllers
{
    public class Utilities
    {
        public static MemoryStream GetUnzipDataStream(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            MemoryStream uncompressed = new MemoryStream();
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using (GZipStream gzStream = new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress, false))
                {
                    gzStream.CopyTo(uncompressed);
                }
            }
            uncompressed.Position = 0;
            return uncompressed;
        }
    }
}