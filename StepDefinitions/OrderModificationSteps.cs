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
    public class OrderModificationSteps
    {
        // Initial order details (with discount)
        private int _initialStarters;
        private int _initialMains;
        private int _initialDrinks;
        private string _initialOrderTime;

        // Additional order details (for joiners after discount period)
        private int _additionalPeople;
        private string _additionalOrderTime;

        // Aggregated order details
        private int _totalStarters;
        private int _totalMains;
        private int _totalDrinks;
        private string _finalOrderTime;
        private IAPIResponse _response;

        private async Task SendAggregatedOrderAsync()
        {
            var payload = new
            {
                starters = _totalStarters,
                mains = _totalMains,
                drinks = _totalDrinks,
                order_time = _finalOrderTime
            };

            _response = await TestHooks.ApiContext.PostAsync("/calculate-total",
                new APIRequestContextOptions { DataObject = payload });
        }

        [Given(@"a group of (\d+) places an order at ""(.*)""")]
        public void GivenAGroupOfPlacesAnOrderAt(int groupSize, string orderTime)
        {
            // For the initial order (discount applies)
            _initialOrderTime = orderTime;
            _initialStarters = 0;
            _initialMains = 0;
            _initialDrinks = 0;
        }

        [When(@"they order (\d+) starter, (\d+) mains, and (\d+) drinks")]
        public async Task WhenTheyOrderInitialItems(int starters, int mains, int drinks)
        {
            _initialStarters = starters;
            _initialMains = mains;
            _initialDrinks = drinks;

            // Initialize aggregated totals from the initial order.
            _totalStarters = _initialStarters;
            _totalMains = _initialMains;
            _totalDrinks = _initialDrinks;
            _finalOrderTime = _initialOrderTime;

            await SendAggregatedOrderAsync();
        }

        [Then(@"the API should apply a 30% discount on the drinks and should return the correct total bill")]
        public async Task ThenTheAPIShouldApplyDiscountOnInitialOrder()
        {
            await CheckoutVerification.VerifyOrderTotalsAsync(_response, _initialStarters, _initialMains, _initialDrinks, _initialOrderTime);
        }

        // Dynamic step for additional joiners with optional order items.
        [Given(@"(\d+) more people join the group at ""(.*)""")]
        public void GivenMorePeopleJoinTheGroupAt(int additionalPeople, string orderTime)
        {
            _additionalPeople = additionalPeople;
            _additionalOrderTime = orderTime;
        }

        // Optional parameters using named groups for starters, mains, and drinks.
        [When(@"they order(?:\s*(?<starters>\d+)\s*starters)?(?:[,\s]*(?<mains>\d+)\s*mains)?(?:[,\s]*(?<drinks>\d+)\s*drinks)?")]
        public async Task WhenTheyOrderAdditionalItems(string starters, string mains, string drinks)
        {
            int additionalStarters = 0;
            int additionalMains = 0;
            int additionalDrinks = 0;
            
            if (!string.IsNullOrEmpty(starters))
                additionalStarters = int.Parse(starters);
            if (!string.IsNullOrEmpty(mains))
                additionalMains = int.Parse(mains);
            if (!string.IsNullOrEmpty(drinks))
                additionalDrinks = int.Parse(drinks);

            // Aggregate the order: add additional items to the initial order.
            _totalStarters = _initialStarters + additionalStarters;
            _totalMains = _initialMains + additionalMains;
            _totalDrinks = _initialDrinks + additionalDrinks;
            // Final order time is that of the additional joiners.
            _finalOrderTime = _additionalOrderTime;

            await SendAggregatedOrderAsync();
        }

        [Then(@"the API should calculate the final bill correctly, applying the discount only to drinks ordered before 19:00 and should return the correct total bill")]
        public async Task ThenTheAPIFinalBillShouldBeCalculatedCorrectly()
        {
            await CheckoutVerification.VerifyOrderTotalsAsync(_response, _totalStarters, _totalMains, _totalDrinks, _finalOrderTime);
        }
    }
}
