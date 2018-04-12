using System;
using System.Net;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using BingMapsRESTService;
using BingMapsRESTService.Common.JSON;

namespace BingREST
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = "Apx7keQxAKI8bRQohCxqN55WlFBLWMrBBNKJTjRp9qNQTVnzgONdIfiwN6tJ8NiR";
            string query = "1427 11th Ave, Seattle WA";
           
            Uri geocodeRequest = new Uri(string.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", query, key));

            GetResponse(geocodeRequest, (x) =>
            {
                Console.WriteLine(x.ResourceSets[0].Resources.Length + " result(s) found.");
                Console.ReadLine();
            });
        }

        private void GetPOSTResponse(Uri uri, string data, Action<Response> callback)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "text/plain;charset=utf-8";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Send the data.
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.BeginGetResponse((x) =>
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                {
                    if (callback != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                        callback(ser.ReadObject(response.GetResponseStream()) as Response);
                    }
                }
            }, null);
        }

        private static void GetResponse(Uri uri, Action<Response> callback)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (o, a) =>
            {
                if (callback != null)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                    callback(ser.ReadObject(a.Result) as Response);
                }
            };
            wc.OpenReadAsync(uri);
        }
    }
}
