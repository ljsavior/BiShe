using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Http
{
    using System.Net.Http;

    class HttpClient
    {
        private System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

        private String url = null;

        private List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

        public HttpClient Url(String url)
        {
            this.url = url;
            return this;
        }

        public HttpClient Param(String name, String value)
        {
            paramList.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        public String Post()
        {
            HttpResponseMessage response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;

            String result = response.Content.ReadAsStringAsync().Result;

            reset();

            return result;
        }

        private void reset()
        {
            url = null;
            paramList.Clear();
        }

    }
}
