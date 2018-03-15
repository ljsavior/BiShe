using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyApplication.Data
{
    class VectorSender
    {
        private volatile TcpListener listener;
        private volatile TcpClient client;
        private volatile NetworkStream netstream;

        private volatile bool state;

        private BinaryFormatter Formatter = new BinaryFormatter();

        public void start()
        {
            lock (this)
            {
                listener = new TcpListener(IPAddress.Any, 6001);
                listener.Start();
            }

            Thread accpectThread = new Thread(() => {
                setClient();
            });
            accpectThread.IsBackground = true;
            accpectThread.Start();
        }

        public void stop()
        {
            lock (this)
            {
                state = false;
                if (netstream != null)
                {
                    netstream.Close();
                }
                if(client != null)
                {
                    client.Close();
                }
            }
        }

        private void setClient()
        {
            TcpClient c = listener.AcceptTcpClient();
            Console.WriteLine(c == null);
            lock (this)
            {
                this.client = c;
                this.netstream = client.GetStream();
                this.state = true;
            }
        }


        public void send(float[] vector)
        {
            lock (this)
            {
                if (state)
                {
                    Formatter.Serialize(netstream, vector);
                    netstream.Flush();
                }
            }
        }

        public void send(double[][] vectors)
        {
            float[] data = parse(vectors);
            lock (this)
            {
                if (state)
                {
                    Formatter.Serialize(netstream, data);
                    netstream.Flush();
                }
            }
        }

        public bool getState()
        {
            lock (this)
            {
                return this.state;
            }
        }

        private float[] parse(double[][] vectors)
        {
            float[] data = new float[vectors.Length * vectors[0].Length];
            int index = 0;
            foreach(double[] vector in vectors)
            {
                foreach(double d in vector)
                {
                    data[index++] = (float)d;
                }
            }
            return data;
        }
    }
}
