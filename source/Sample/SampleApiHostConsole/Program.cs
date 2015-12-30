using CacheSample;
using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

namespace CachedAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                Console.WriteLine("API running at " + baseAddress);
                Console.ReadLine();
            }
        }
    }
}
