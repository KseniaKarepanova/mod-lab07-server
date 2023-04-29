using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lab07
{
    class Server
    {
        struct PoolRecord
        {
            public Thread thread; // объект потока
            public bool in_use; // флаг занятости
        }
        object threadLock = new object();
        PoolRecord[] pool;
        public int requestCount;
        public int processedCount;
        public int rejectedCount;
        public double intensity;

        public Server(int pool_count, double intensity)
        {
            this.intensity = intensity;
            pool = new PoolRecord[pool_count];
        }

        public void proc(object sender, ProcEventArgs e)
        {
            lock (threadLock)
            {
                requestCount++;
                for (int i = 0; i < pool.Length; i++)
                {
                    if (!pool[i].in_use)
                    {
                        pool[i].in_use = true;
                        pool[i].thread = new Thread(new ParameterizedThreadStart(Answer));
                        pool[i].thread.Start(i);
                        processedCount++;
                        Console.WriteLine("Заявка с номером: " + e.id + " принята");

                        return;
                    }
                }
                rejectedCount++;
                Console.WriteLine("Заявка с номером: " + e.id + " отклонена");

            }
        }

        public bool NotWork()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].in_use)
                    return false;
            }
            return true;
        }

        private void Answer(object? id)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1 / intensity));
            pool[(int)id].in_use = false;
        }

    }
}
