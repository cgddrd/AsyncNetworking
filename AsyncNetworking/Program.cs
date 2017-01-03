using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncNetworking
{
    class Program
    {

        static Networking networkClient;

        static void Main(string[] args)
        {
            //Some examples of online REST API mocking tools:
            // 1. http://www.fakeresponse.com
            // 2. http://slowwly.robertomurray.co.uk
            // 3. https://getsandbox.com/pricing
            // 4. http://www.mocky.io
            // 5. https://www.mockable.io

            networkClient = new Networking("http://www.fakeresponse.com/api");

            Task.Factory.StartNew(RunHistoricalUploads, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            //RunCalls();

            RunWatcher();

            Console.ReadLine();

        }

        static void RunHistoricalUploads()
        {

            Console.WriteLine("Beginning historical uploads.");

            var random = new Random();

            for (int i=0; i<10; i++) 
            {
                var sleepTime = random.Next(0, 2) == 1 ? random.Next(0, 20) : 0;
                //var sleepTime = random.Next(0, 20);
                //var sleepTime = 0;

                Console.WriteLine($"Beginning historical upload: {i} (sleep time = {sleepTime} secs).");

                networkClient.GetRequestAsync($"?data={{%22name%22:%22upload{i}%22}}&sleep={sleepTime}&status=200")
                                    .ContinueWith(callback =>
                                     {
                                         Console.WriteLine($"Completed historical upload: {i}.");
                                     });

            }


            Console.WriteLine("Completed historical uploads.");

        }

        static void RunCalls()
        {
            Console.WriteLine("Starting RunCalls()...");

            Console.WriteLine("Starting Task1...");
            var task1 = networkClient.GetRequestAsync("?data={%22name%22:%22task1%22}&sleep=10&status=200&meta=false")
                                     .ContinueWith(callback =>
                                     {
                                         Console.WriteLine($"Result (Task1) = {callback.Result}");
                                     });

            // If we use 'await' here, then the application will wait until we get the response back from the API.
            // Currently, it continues on (the await inside of 'GetRequestAsync' will wait for the response to come back). 

            Console.WriteLine("Starting Task2...");
            var task2 = networkClient.GetRequestAsync("?data={%22name%22:%22task2%22}&sleep=0&status=200&meta=false")
                            .ContinueWith(callback =>
                                {
                                    Console.WriteLine($"Result (Task2) = {callback.Result}");
                                });

            Console.WriteLine("Ending RunCalls()...");
        }

        static void RunWatcher()
        {
            var random = new Random();
            int jobNo = 1;

            while(true)
            {

                var sleep = random.Next(0, 20000);
                Console.WriteLine($"Waiting for {TimeSpan.FromMilliseconds(sleep).Seconds} secs...");
                Thread.Sleep(sleep);

                Console.WriteLine($"Beginning watcher job no: {jobNo}.");

                var task2 = networkClient.GetRequestAsync($"?data={{%22name%22:%22job{jobNo}%22}}&sleep=0&status=200&meta=false")
                            .ContinueWith(callback =>
                            {
                                Console.WriteLine($"Result Job No: {jobNo}.");
                            });

                //Console.WriteLine($"Finished watcher job no: {jobNo}.");

                jobNo++;

            }
        }

        
    }
}
