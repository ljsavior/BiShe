using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Posture
{
    using Service;
    using System.IO;

    class PostureLoader
    {
        private Service service = new Service();

        public void Load(int id)
        {
            String dirPath = Constant.POSTURE_DATA_FILE_DIR_PATH + id + "/";
            if(!Directory.Exists(dirPath))
            {

            }


        }
    }
}
