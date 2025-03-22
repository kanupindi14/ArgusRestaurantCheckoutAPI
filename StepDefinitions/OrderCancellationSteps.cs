using System;
using System.Globalization;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using RestaurantCheckoutTests.Common;
using RestaurantCheckoutTests.Hooks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace RestaurantCheckoutTests.StepDefinitions
{
    [Binding]
    public class OrderCancellationSteps
    {
        private int _groupSize;
        private int _starters;
        private int _mains;
        private int _drinks;
        private string _orderTime;
        private IAPIResponse _response;
        
        // Store per-person order details for reuse.
        private int _individualStarters;
        private int _individualMains;
        private int _individualDrinks;

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
            _orderTime = "18:30"; // default valid time
            _starters = 0;
            _mains = 0;
            _drinks = 0;
        }

        [When(@"they placed order (\d+) starters, (\d+) mains, and (\d+) drinks\.")]
        public async Task WhenTheyPlacedOrderItems(int starters, int mains, int drinks)
        {
            _starters = starters;
            _mains = mains;
            _drinks = drinks;
            await SendOrderAsync();
        }

        [Then(@"the API should return the correct total\.")]
        public async Task ThenTheAPIShouldReturnTheCorrectTotal()
        {
            await CheckoutVerification.VerifyOrderTotalsAsync(_response, _starters, _mains, _drinks, _orderTime);
        }

        [When(@"they cancel the entire order")]
        public async Task WhenTheyCancelTheEntireOrder()
        {
            _response = await TestHooks.ApiContext.PostAsync("/cancel-order",
                new APIRequestContextOptions { DataObject = new { } });
        }

        [Then(@"the API should return ""(.*)"" and total should be 0")]
        public async Task ThenTheAPIReturnsOrderCanceledAndZeroTotal(string expectedMessage)
        {
            var json = await _response.JsonAsync();
            string message = json.GetProperty("message").ToString();
            double orderTotal = Convert.ToDouble(json.GetProperty("order_total").ToString(), CultureInfo.InvariantCulture);

            Assert.AreEqual(expectedMessage, message, "Cancellation message mismatch");
            Assert.AreEqual(0, orderTotal, "Order total should be 0 after cancellation");
        }

        [When(@"each of them placed order (\d+) starters, (\d+) mains, and (\d+) drink\.")]
        public async Task WhenEachOfThemPlacedOrderItems(int starters, int mains, int drink)
        {
            // Store individual order details for reuse in cancellation
            _individualStarters = starters;
            _individualMains = mains;
            _individualDrinks = drink;
            // Calculate totals based on the current group size.
            _starters = _groupSize * starters;
            _mains = _groupSize * mains;
            _drinks = _groupSize * drink;
            await SendOrderAsync();
        }

        [When(@"(\d+) person cancels their order")]
        public async Task WhenAPersonCancelsTheirOrder(int personsCanceling)
        {
            // Reduce the group size by the number of persons cancelling.
            _groupSize -= personsCanceling;
            // Recalculate totals using the stored per-person order values.
            _starters = _groupSize * _individualStarters;
            _mains = _groupSize * _individualMains;
            _drinks = _groupSize * _individualDrinks;
            await SendOrderAsync();
        }

        [Then(@"the API total should calculate and return the final bill correctly")]
        public async Task ThenTheAPIFinalBillShouldBeCalculatedCorrectly()
        {
            await CheckoutVerification.VerifyOrderTotalsAsync(_response, _starters, _mains, _drinks, _orderTime);
        }
    }
}
