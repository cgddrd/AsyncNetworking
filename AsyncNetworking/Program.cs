using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            RunCalls();

            Console.ReadLine();

        }

        static void RunCalls()
        {
            Console.WriteLine("Starting RunCalls()...");

            Console.WriteLine("Starting Task1...");
            var task1 = networkClient.GetRequestAsync("?data={%22name%22:%22task1%22}&sleep=30&status=200")
                                     .ContinueWith(callback =>
                                     {
                                         Console.WriteLine($"Result (Task1) = {callback.Result}");
                                     });

            // If we use 'await' here, then the application will wait until we get the response back from the API.
            // Currently, it continues on (the await inside of 'GetRequestAsync' will wait for the response to come back). 

            Console.WriteLine("Starting Task2...");
            var task2 = networkClient.GetRequestAsync("?data={%22Hello%22:%22World%22}&sleep=5&status=500")
                            .ContinueWith(callback =>
                                {
                                    Console.WriteLine($"Result (Task2) = {callback.Result}");
                                });

            Console.WriteLine("Ending RunCalls()...");
        }

        
    }
}
