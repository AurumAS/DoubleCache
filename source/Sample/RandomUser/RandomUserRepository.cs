using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RandomUser
{
    public class RandomUserRepository
    {
        private JsonSerializer serializer;
        private static readonly HttpClient HttpClient;

        static RandomUserRepository()
        {
            HttpClient = new HttpClient();
        }

        public RandomUserRepository()
        {
            serializer = new JsonSerializer();
        }

        public async Task<User> GetSingleDummyUser()
        {
            var users = await GetManyDummyUser(1);
            return users.FirstOrDefault();
        }

        public async Task<List<User>> GetManyDummyUser(int take)
        {
            var result = new List<User>();

            decimal batchCount = Math.Ceiling(((decimal)take / 5000));

            for(int i = 0; i < batchCount; i++)
            {
                result.AddRange(await GetBatch(take - i * 5000));
            }

            return result.Take(take).ToList();
        }
        private async Task<List<User>> GetBatch(int count)
        {
            if (count > 5000)
                count = 5000;
            string url = "http://api.randomuser.me/?results=" + count;

            using (var stream = await HttpClient.GetStreamAsync(url))
            using (var ms = new MemoryStream())
            //{
            //    stream.CopyTo(ms);
            //    ms.Position = 0;
            //    using (var fs = new FileStream("c:\\ert\\temp.json", FileMode.Create))
            //    {
            //        ms.CopyTo(fs);
            //        fs.Close();
            //    }
            //    ms.Position = 0;
            //    using (var sr = new StreamReader(ms))
            //}
            using (var sr = new StreamReader(stream))
            using (var jr = new JsonTextReader(sr))
                {
                    var response = serializer.Deserialize<RandomUserResponse>(jr);
                    return response.Results.ToList();
                }
            }
            
        } 
    }


