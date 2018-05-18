using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Posture
{
    using Service;
    using System.IO;
    using Utils;
    using com.force.json;
    using Http;

    class PostureLoader
    {
        private Service service = new Service();

        private HttpClient client = new HttpClient();

        public Posture Load(int id)
        {
            if(!dataExistOnDisk(id))
            {
                deleteData(id);
                downData(id);
            }
            return loadDataFromDisk(id);
        }

        private String getDirPath(int id)
        {
            return Constant.POSTURE_DATA_FILE_DIR_PATH + id + "/";
        }

        private String getDataFilePath(int id)
        {
            return Constant.POSTURE_DATA_FILE_DIR_PATH + id + "/data";
        }

        private String getImgFilePath(int id)
        {
            return Constant.POSTURE_DATA_FILE_DIR_PATH + id + "/img.jpg";
        }


        private bool dataExistOnDisk(int id)
        {
            String dirPath = getDirPath(id);
            String dataFilePath = getDataFilePath(id);
            String imgFilePath = getImgFilePath(id);

            return Directory.Exists(dirPath) && File.Exists(dataFilePath) && File.Exists(imgFilePath);
        }

        private void deleteData(int id)
        {
            String dirPath = getDirPath(id);
            String dataFilePath = getDataFilePath(id);
            String imgFilePath = getImgFilePath(id);

            if(File.Exists(dataFilePath))
            {
                File.Delete(dataFilePath);
            }
            if (File.Exists(imgFilePath))
            {
                File.Delete(imgFilePath);
            }
            if(Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath);
            }
        }

        private bool downData(int id)
        {
            try
            {
                String dirPath = getDirPath(id);
                String dataFilePath = getDataFilePath(id);
                String imgFilePath = getImgFilePath(id);

                Directory.CreateDirectory(dirPath);

                JSONObject posObj = service.queryPosture(id);
                String imgPath = Constant.SERVER_HOST + "/getImage/" + posObj.GetString("picPath");
                String data = posObj.GetString("data");

                client.downloadImg(imgPath, imgFilePath);

                using (StreamWriter sw = new StreamWriter(dataFilePath, true))
                {
                    sw.WriteLine(data);
                    sw.Flush();
                }

                return true;
            } catch(Exception e)
            {
                LogUtil.log(e.ToString());
                return false;
            }
        }



        private Posture loadDataFromDisk(int id)
        {
            try
            {
                String dataFilePath = getDataFilePath(id);
                String imgFilePath = getImgFilePath(id);

                double[][] vectors;
                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    vectors = CommonUtil.stringToDoubleArray2(sr.ReadLine());
                }
                return new Posture(PostureType.Both, vectors, imgFilePath);

            } catch(Exception e)
            {
                LogUtil.log(e.ToString());
                return null;
            }
        }


    }
}
