using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
namespace Lab07
{
    class Program
    {
        static void Main(string[] args)
        {
            double T = 1;
            double application_intensity = 20;
            double service_intensity = 5;
            double P0;
            double Pn;
            double Q;
            double A;
            double k;
            int pool_count = 10;
            double r;
            double r_real;
            Server server = new Server(pool_count, service_intensity);
            Client client = new Client(server);
            var sw = new Stopwatch();
            sw.Start();
            int count = 15;
            for (int i = 0; i < count; i++)
            {
                client.Run(i);
                Thread.Sleep(TimeSpan.FromSeconds(1 / application_intensity));

            }
            while (true)
            {
                if (server.NotWork())
                    break;
            }
            sw.Stop();
            T = (double)sw.ElapsedMilliseconds / 1000;

            r = calculate_r(application_intensity, service_intensity);
            P0 = calculate_P0(pool_count, r);
            Pn = calculate_Pn(pool_count, r, P0);
            Q = 1 - Pn;
            A = application_intensity * Q;
            k = A / service_intensity;
            Console.WriteLine("requestCount= " + server.requestCount);
            Console.WriteLine("ProcrssedCount= " + server.processedCount);
            Console.WriteLine("RejectedCount= " + server.rejectedCount);
            Console.WriteLine("application_intensity=" + r);
            Console.WriteLine("r=" + r);
            Console.WriteLine("P0=" + P0);
            Console.WriteLine("Pn=" + Pn);
            Console.WriteLine("Q=" + Q);
            Console.WriteLine("A=" + A);
            Console.WriteLine("k= " + k);
            Console.WriteLine("/////////");

            double lyamda = server.requestCount / T;
            double nu = server.processedCount / (T * pool_count);

            r_real = calculate_r(lyamda, nu);
            double P0_real = calculate_P0(pool_count, r_real);
            double Pn_real = calculate_Pn(pool_count, r_real, P0_real); ;
            double Q_real = 1 - Pn_real;
            double A_real = lyamda * Q_real;
            double k_real = A_real / nu;
            Console.WriteLine("r=" + r_real);
            Console.WriteLine("P0=" + P0_real);
            Console.WriteLine("Pn=" + Pn_real);
            Console.WriteLine("Q=" + Q_real);
            Console.WriteLine("A=" + A_real);
            Console.WriteLine("k= " + k_real);

            WriteFile(server.requestCount, server.processedCount, server.rejectedCount, r, P0, Pn, Q, A, k, r_real, P0_real, Pn_real, Q_real, A_real, k_real);
        }
        static double calculate_r(double lyamda, double nu)
        {
            return lyamda / nu;
        }

        static double calculate_P0(int pool_count, double r)
        {
            double P0 = 0;
            for (int i = 0; i <= pool_count; i++)
            {
                P0 = P0 + Math.Pow(r, i) / Factorial(i);
            }
            return Math.Pow(P0, -1);
        }

        static double calculate_Pn(int pool_count, double r, double P0)
        {
            double Pn = Math.Pow(r, pool_count) / Factorial(pool_count) * P0;
            return Pn;
        }

        static double Factorial(double n)
        {
            double factorial = 1;
            for (int i = 1; i <= n; i++)
                factorial = factorial * i;
            return factorial;
        }

        static void WriteFile(double requestCount, double ProcessedCount, double RejectedCount, double r, double P0, double Pn, double Q, double A, double k, double r_real, double P0_real, double Pn_real, double Q_real, double A_real, double k_real)
        {
            string file_name = "../../../results.txt";
            FileStream fileStream = new FileStream(file_name, FileMode.Append);
            string text = "RequestCount=" + requestCount + "\nProcessedCount=" + ProcessedCount + "\nRejectedCount=" + RejectedCount +
                "\n Teoretic:\n" + "P0=" + P0 + "\n Pn=" + Pn + "\n Q=" + Q + "\n A=" + A + "\n k=" + k +
                "\n Practic:\n" + "P0=" + P0_real + "\n Pn=" + Pn_real + "\n Q=" + Q_real + "\n A=" + A_real + "\n k=" + k_real + "\n ************** \n\n\n";
            byte[] t = Encoding.Default.GetBytes(text);
            fileStream.Write(t, 0, t.Length);
            fileStream.Close();
        }
    }
}
