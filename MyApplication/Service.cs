using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Service
{
    using Http;
    using com.force.json;

    class Service
    {
        private HttpClient httpClient = new HttpClient();

        public String[] getTrainingNames()
        {
            String result = httpClient.Url(Constant.SERVIER_GET_TRAINING_NAME_LIST_URL)
                .Post();

            JSONArray names = new JSONArray(result);
            String[] namesArray = new String[names.Length()];

            for(int i = 0; i < names.Length(); i++)
            {
                namesArray[i] = names.GetString(i);
            }

            return namesArray;
        }

        public void uploadTrainingRecord(String trainingName, List<int> timesUsed, List<bool> result)
        {
            LoginService loginService = LoginService.getInstance();
            if (!loginService.IsLogin)
            {
                return;
            }

            JSONArray timesUsedArray = new JSONArray();
            foreach(int time in timesUsed)
            {
                timesUsedArray.Put(time);
            }

            JSONArray resultArray = new JSONArray();
            foreach(bool res in result)
            {
                resultArray.Put(res ? 1 : 0);
            }

            String resStr = httpClient.Url(Constant.SERVER_TRAINING_RECORD_UPDATE_URL)
                .Param("username", loginService.Username)
                .Param("trainingName", trainingName)
                .Param("timesUsed", timesUsedArray.ToString())
                .Param("result", resultArray.ToString())
                .Post();

            Utils.LogUtil.log(resStr);

        }


    }
}
