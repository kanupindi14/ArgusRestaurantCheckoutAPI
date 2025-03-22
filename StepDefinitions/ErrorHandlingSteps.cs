using System;
using System.Globalization;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using RestaurantCheckoutTests.Hooks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace RestaurantCheckoutTests.StepDefinitions
{
    [Binding]
    public class ErrorHandlingSteps
    {
        private int _groupSize;
        private int _starters;
        private int _mains;
        private int _drinks;
        private string _orderTime;
        private IAPIResponse _response;

        private async Task SendOrderAsync()
        {
            var payload = new
            {
                starters = _starters,
                mains = _mains,
                drinks = _drinks,
                order_time = _orderTime
            };

            _response = await TestHooks.ApiContext.PostAsync("/calculate-total",
                new APIRequestContextOptions { DataObject = payload });
        }

        [Given(@"a group of (\d+) places an order")]
        public void GivenAGroupOfPlacesAnOrder(int groupSize)
        {
            _groupSize = groupSize;
            _orderTime = "18:30"; // a valid default time
        }

        [When(@"they order (\-?\d+) starters, (\-?\d+) mains, and (\-?\d+) drink")]
        public async Task WhenTheyOrderInvalidQuantities(int starters, int mains, int drinks)
        {
            _starters = starters;
            _mains = mains;
            _drinks = drinks;
            await SendOrderAsync();
        }

        [Then(@"the API should return an error message ""(.*)""")]
        public async Task ThenTheAPIShouldReturnAnErrorMessage(string expectedError)
        {
            var json = await _response.JsonAsync();
            string error = json.GetProperty("error").ToString();
            Assert.AreEqual(expectedError, error, "Error message mismatch");
        }
    }
}
