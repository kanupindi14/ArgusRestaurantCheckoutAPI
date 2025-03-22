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
    public class CheckoutSteps
    {
        private int _groupSize;
        protected int _starters;
        protected int _mains;
        protected int _drinks;
        protected string _orderTime;
        protected IAPIResponse _response;

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

        [Given(@"a group of (\d+) places an order before 19:00")]
        public void GivenAGroupOfPlacesAnOrderBefore1900(int groupSize)
        {
            _groupSize = groupSize;
            _orderTime = "18:30"; // sample time before 19:00
            _starters = 0;
            _mains = 0;
            _drinks = 0;
        }

        [Given(@"a group of (\d+) places an order after 19:00")]
        public void GivenAGroupOfPlacesAnOrderAfter1900(int groupSize)
        {
            _groupSize = groupSize;
            _orderTime = "20:00"; // sample time after 19:00
            _starters = 0;
            _mains = 0;
            _drinks = 0;
        }

        [When(@"they order (\d+) starters, (\d+) mains, and (\d+) drinks")]
        public async Task WhenTheyOrderItems(int starters, int mains, int drinks)
        {
            _starters = starters;
            _mains = mains;
            _drinks = drinks;
            await SendOrderAsync();
        }

        [Then(@"the API should return the correct total bill")]
        public async Task ThenTheAPIShouldReturnTheCorrectTotalBill()
        {
            await CheckoutVerification.VerifyOrderTotalsAsync(_response, _starters, _mains, _drinks, _orderTime);
        }
    }
}
