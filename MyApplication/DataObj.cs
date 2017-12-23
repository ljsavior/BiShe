namespace MyApplication.Data
{
    class DataObj : System.MarshalByRefObject
    {
        private static System.Collections.Concurrent.BlockingCollection<double[][]> messageQueue = new System.Collections.Concurrent.BlockingCollection<double[][]>();

        public void send(double[][] data)
        {
            messageQueue.Add(data);
        }

        public double[][] receive()
        {
            return messageQueue.Take();
        }
        
        public int clear()
        {
            int count = messageQueue.Count;
            for(int i = 0; i < count; i++)
            {
                messageQueue.Take();
            }
            return count;
        }
    }
}