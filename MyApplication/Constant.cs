using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication
{
    class Constant
    {
        public const String LOG_FILE_PATH = "d:/MyApplication/log/out.log";
        public const String LOG_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";


        public const String SIMLUATION_EXE_PATH = "D:/MyApplication/RealTimeSimulation/simulation.exe";

        public const String GAME_EXE_PATH = "d:/MyApplication/Race/Race.exe";


        public const String ACTION_DATA_FILE_DIR_PATH = "d:/MyApplication/ActionData/";



        public const String SERVER_HOST = "http://39.105.38.80:8080";

        public const String SERVER_LOGIN_URL = SERVER_HOST + "/client/login.json";

        public const String SERVIER_GET_TRAINING_NAME_LIST_URL = SERVER_HOST + "/client/training/nameList.json";

        public const String SERVER_TRAINING_RECORD_UPDATE_URL = SERVER_HOST + "/client/trainingRecord/upload.json";

    }
}
