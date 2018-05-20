using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyApplication.Service
{
    using Http;
    using com.force.json;

    class Service
    {
        private HttpClient httpClient = new HttpClient();

        public String[] getTrainingNames(int trainingType)
        {
            String result = httpClient.Url(Constant.SERVIER_GET_TRAINING_NAME_LIST_URL)
                .Param("type", trainingType.ToString())
                .Post();
            Utils.LogUtil.log(result);

            JSONArray names = new JSONArray(result);
            String[] namesArray = new String[names.Length()];

            for(int i = 0; i < names.Length(); i++)
            {
                namesArray[i] = names.GetString(i);
            }

            return namesArray;
        }

        public void uploadTrainingRecord(String trainingName, int trainingType, List<int> timesUsed, List<bool> result)
        {
            LoginService loginService = LoginService.getInstance();
            if (!loginService.IsLogin)
            {
                Utils.LogUtil.log("没有登录");
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

            String resStr = httpClient.Url(Constant.SERVER_TRAINING_RECORD_UPLOAD_URL)
                .Param("userId", loginService.UserId.ToString())
                .Param("trainingName", trainingName)
                .Param("trainingType", trainingType.ToString())
                .Param("timesUsed", timesUsedArray.ToString())
                .Param("result", resultArray.ToString())
                .Post();

            Utils.LogUtil.log(resStr);

        }

        public bool uploadPosture(String name, BitmapSource img, double[][] data, String mark)
        {
            byte[] array = Utils.ImageUtil.BitmapSourceToByteArray(img);
            String resStr = httpClient.Url(Constant.SERVER_UPLOAD_IMG_URL).Upload(array, "img", "img.jpg");
            JSONObject resObj = new JSONObject(resStr);
            bool success = bool.Parse(resObj.GetString("success"));

            if (!success)
            {
                Utils.LogUtil.log(resStr);
                return false;
            }
            String imgName = resObj.GetString("data");
            resStr = httpClient.Url(Constant.SERVER_POSTURE_UPLOAD_URL)
                .Param("name", name)
                .Param("picPath", imgName)
                .Param("data", Utils.CommonUtil.arrayToString(data))
                .Param("mark", "")
                .Post();
            Utils.LogUtil.log(resStr);
            resObj = new JSONObject(resStr);
            return bool.Parse(resObj.GetString("success"));
        }


        public JSONObject queryPosture(int id)
        {
            try
            {
                String resStr = this.httpClient.Url(Constant.SERVER_POSTURE_QUERY_URL)
                    .Param("id", id.ToString())
                    .Post();
                Utils.LogUtil.log(resStr);
                JSONObject obj = new JSONObject(resStr);
                return obj.GetJSONObject("data");
            } catch(Exception e)
            {
                Utils.LogUtil.log(e.ToString());
                return null;
            }
        }

        public JSONObject queryTraining(String trainingName)
        {
            try
            {
                String resStr = httpClient.Url(Constant.SERVIER_GET_TRAINING_QUERY_URL)
                .Param("name", trainingName)
                .Post();
                Utils.LogUtil.log(resStr);
                JSONObject obj = new JSONObject(resStr);
                return obj.GetJSONObject("data");
            } catch(Exception ex)
            {
                Utils.LogUtil.log(ex.ToString());
                return null;
            }
            
        }
    }
}
