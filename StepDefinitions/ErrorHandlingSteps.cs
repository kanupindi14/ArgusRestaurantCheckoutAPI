using System;
using System.Globalization;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using RestaurantCheckoutTests.Hooks;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.Json;

namespace RestaurantCheckoutTests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Checkout API - Handle Zero and Negative Quantities")]
    public class ErrorHandlingSteps
    {
        private int _groupSize;
        private int _starters;
        private int _mains;
        private int _drinks;
        protected string _orderTime = string.Empty;
        protected IAPIResponse _response = null!;

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
            JsonElement? json = await _response.JsonAsync();
            if (!json.HasValue)
            {
                Assert.Fail("Response JSON is null.");
            }
            string error = json.Value.GetProperty("error").GetString() ?? string.Empty;
            Assert.That(error, Is.EqualTo(expectedError), "Error message mismatch");
        }
    }
}
