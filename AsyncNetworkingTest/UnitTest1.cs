using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncNetworking;
using RichardSzalay.MockHttp;
using System.Net;
using System.Threading.Tasks;

namespace AsyncNetworkingTest
{
    [TestClass]
    public class UnitTest1
    {
        private Networking client;
        private MockHttpMessageHandler mockHandler;

        [TestInitialize]
        public void Initialize()
        {
            mockHandler = new MockHttpMessageHandler();
        }

        [TestMethod]
        //[ExpectedException(typeof(WebException))]
        // We have to set the return type to 'Task' in order for MSTest to corrrectly identify it as a test method.
        public async Task TestMethod1Async()
        {

            mockHandler.Expect("http://localhost/api/test/1")
                       .Respond(HttpStatusCode.OK);

            client = new Networking(mockHandler, "http://localhost/api/");

            var response = await client.MakeRequestAsync("test/1");

            Assert.IsTrue(response.IsSuccessStatusCode, "An unsuccessful status code has been returned.");

            //await AssertThrows<Exception>(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            mockHandler.VerifyNoOutstandingExpectation();

        }

        // Tests whether the supplied Task throws an exception. 
        private async Task AssertThrows<TException>(Task task) where TException : Exception
        {
            try
            {
                await task;
                Assert.Fail("Expected exception of type: " + typeof(TException));
            } catch (TException)
            {
                // Exception expected. Do nothing.
            }
        }
    }
}
