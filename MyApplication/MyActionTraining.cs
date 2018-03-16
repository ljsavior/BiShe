using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Training
{
    using Data;
    using System.Windows.Media.Imaging;

    class MyActionTraining
    {
        private const int EachPositionTime = 60;

        private List<ActionData> actionList = new List<ActionData>();

        public int index {get; private set;}
        public int successCount { get; private set; }

        public int currentActionTime { get; private set; }

        public List<int> timeUsedList { get; private set; }
        public List<bool> resultList { get; private set; }

        public MyActionTraining()
        {
            index = -1;
            successCount = 0;
            timeUsedList = new List<int>();
            resultList = new List<bool>();
        }

        public void addActions(params ActionData[] actions)
        {
            foreach(ActionData action in actions)
            {
                actionList.Add(action);
            }
        }


        public Boolean isFinish()
        {
            return index == actionList.Count;
        }

        public void next()
        {
            index++;
            currentActionTime = EachPositionTime;
        }

        public void next(bool success)
        {
            if (!success && currentActionTime != 0)
            {
                return;
            }

            if (success)
            {
                successCount++;
            }

            this.resultList.Add(success);
            this.timeUsedList.Add(EachPositionTime - currentActionTime);

            index++;
            currentActionTime = EachPositionTime;
        }

        public ActionData getActionData()
        {
            return actionList[index];
        }

        public int countDown()
        {
            return --currentActionTime;
        }

        public int getProgess()
        {
            return 100 * index / actionList.Count;
        }
    }
}
