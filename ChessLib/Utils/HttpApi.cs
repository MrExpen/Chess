using ChessLib.Http.Responses;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Utils
{
    public static class HttpApi
    {
        private static readonly RestClient _restClient = new RestClient();
        public static int? CreateMatch(string url, string White, string Black)
        {
            RestRequest request = new RestRequest($"{url}/api/chess/creatematch");
            request.AddQueryParameter("whiteName", White);
            request.AddQueryParameter("blackName", Black);
            CreateMatchResponse response = null;
            do
            {
                response = JsonConvert.DeserializeObject<CreateMatchResponse>(_restClient.Execute(request).Content);
            } while (response is null);
            return response.Success ? response.MatchId : null;
        }
    }
}
