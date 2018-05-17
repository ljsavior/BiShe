using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication
{
    class Constant
    {
        public const String BASE_PATH = "d:/MyApplication";

        public const String LOG_FILE_PATH = BASE_PATH + "/log/out.log";
        public const String LOG_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";


        public const String SIMLUATION_EXE_PATH = BASE_PATH + "/RealTimeSimulation/simulation.exe";

        public const String GAME_EXE_PATH = BASE_PATH + "/Race/Race.exe";


        public const String POSTURE_DATA_FILE_DIR_PATH = BASE_PATH + "/PostureData/";

        public const String ACTION_DATA_FILE_DIR_PATH = BASE_PATH + "/ActionData/";






        public const String SERVER_HOST = "http://39.105.38.80:8080";

        public const String SERVER_LOGIN_URL = SERVER_HOST + "/client/login.json";

        public const String SERVIER_GET_TRAINING_NAME_LIST_URL = SERVER_HOST + "/client/training/nameList.json";

        public const String SERVER_TRAINING_RECORD_UPLOAD_URL = SERVER_HOST + "/client/trainingRecord/upload.json";

        public const String SERVER_UPLOAD_IMG_URL = SERVER_HOST + "/uploadImage.json";

        public const String SERVER_POSTURE_UPLOAD_URL = SERVER_HOST + "/posture/addOrUpdate.json";

    }
}
