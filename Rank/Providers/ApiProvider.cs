using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rank.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Rank.Providers
{
    public class ApiProvider
    {
        const string AddressBase = @"https://api.dataforseo.com/";
        const string Login = "challenger31@rankactive.info";
        const string Password = "7EN4yW8nH";
        const string PartPathGetAllSearchEngine = "v2/cmn_se";
        const string PartPathGetLocation = "v2/cmn_locations";

        public async Task<ObservableCollection<string>> GetAllListSearchEngines()
        {
            ObservableCollection<string> searchEnginesList = new ObservableCollection<string>();

            var obj = await this.GetDeserializeObject(PartPathGetAllSearchEngine);

            if (obj.status == "error")
            {
                ///TODO:Add Log
               // Console.WriteLine($"error. Code: {obj.error.code} Message: {obj.error.message}");
            }
            else
            {
                foreach (var result in obj.results)
                {
                    string searchEngine = result.se_name;

                    if (!searchEnginesList.Contains(searchEngine.Split(new char[] { ' ' }).FirstOrDefault()))
                    {
                        searchEnginesList.Add(searchEngine);
                    }
                }
            }
            return searchEnginesList;
        }

        public async Task<ObservableCollection<string>> GetListLocation()
        {
            ObservableCollection<string> locationList = new ObservableCollection<string>();

            var obj = await this.GetDeserializeObject(PartPathGetLocation);

            if (obj.status == "error")
            {
                //Console.WriteLine($"error. Code: {obj.error.code} Message: {obj.error.message}");
            }
            else
            {
                foreach (var result in obj.results)
                {
                    string parentId = result.loc_id_parent;
                    string location = result.loc_name_canonical;

                    if (parentId == null)
                    {
                        if (!locationList.Contains(location))
                        {
                            locationList.Add(location);
                        }
                    }
                }
            }
            return locationList;
        }

        public async Task<IEnumerable<long>> RankTaskSettings(string searchEngin, string location, string webSite, IEnumerable<string> keyWords)
        {
            var httpClient = this.GetHttpClient();

            var rnd = new Random();
            var postObject = new Dictionary<int, object>();

            foreach (var keyWord in keyWords)
            {
                var keyIndex = rnd.Next(1, 30000000);
                if (!postObject.ContainsKey(keyIndex))
                {
                    postObject.Add(keyIndex, new
                    {
                        priority = 2,
                        site = webSite,
                        se_name = searchEngin,
                        se_language = "English",
                        loc_name_canonical = location,
                        key = keyWord
                    });
                }
            }

            List<long> tasks = new List<long>();
            var taskPostResponse = await httpClient.PostAsync("v2/rnk_tasks_post", new StringContent(JsonConvert.SerializeObject(new { data = postObject })));
            var obj = JsonConvert.DeserializeObject<dynamic>(await taskPostResponse.Content.ReadAsStringAsync());
            if (obj.status == "error")
            {
                //Console.WriteLine($"error. Code: {obj.error.code} Message: {obj.error.message}");
            }
            else
            {
                foreach (var result in obj.results)
                {
                    var res = (long)(((IEnumerable<dynamic>)result).First()).task_id;


                    tasks.Add(res);
                }
            }
            return tasks;
        }

        public async Task<dynamic> GetRankResult(long taskid)
        {
            dynamic rankInfo = default(dynamic);

            var httpClient = this.GetHttpClient();
            dynamic obj;

            do
            {
                var response = await httpClient.GetAsync($"v2/rnk_tasks_get/{taskid}");

                var contetnString = await response.Content.ReadAsStringAsync();
                obj = JsonConvert.DeserializeObject<dynamic>(contetnString);

                await Task.Delay(1000);
             
            } while (obj.results_count == 0);

            if (obj.status == "error")
            {
                //Console.WriteLine($"error. Code: {obj.error.code} Message: {obj.error.message}");
            }
            else if (obj.results_count != 0)
            {
                rankInfo = obj.results.organic;
            }
            else
            {
                // Console.WriteLine("no results");
            }
            return rankInfo;
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(AddressBase),

                //Instead of 'login' and 'password' use your credentials from https://my.dataforseo.com/login
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Login}:{Password}"))) }
            };
            return httpClient;
        }

        private async Task<HttpResponseMessage> GetResponse(string partPath)
        {
            var httpClient = this.GetHttpClient();
            var response = await httpClient.GetAsync(partPath);
            return response;
        }

        private async Task<dynamic> GetDeserializeObject(string partPath)
        {
            var httpClient = this.GetHttpClient();
            var response = await GetResponse(partPath);
            var obj = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            return obj;
        }
    }
}
