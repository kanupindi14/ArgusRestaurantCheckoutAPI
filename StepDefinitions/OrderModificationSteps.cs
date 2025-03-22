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
    [Scope(Feature = "Order Modification")]
    public class OrderModificationSteps
    {
        // Initial order details (with discount)
        private int _initialStarters;
        private int _initialMains;
        private int _initialDrinks;
        private string _initialOrderTime = "";

        // Additional joiners details
        private int _additionalPeople;
        private string _additionalOrderTime = string.Empty;

        // Aggregated order details
        private int _totalStarters;
        private int _totalMains;
        private int _totalDrinks;
        private string _finalOrderTime = string.Empty;
        private IAPIResponse _response = null!;

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

        // Bind the step for joiners. We use a [Given] binding for consistency.
        [When(@"(.*) more people join the group at ""(.*)""")]
        public void GivenMorePeopleJoinTheGroupAt(int additionalPeople, string orderTime)
        {
            _additionalPeople = additionalPeople;
            _additionalOrderTime = orderTime;
        }

        // Bind the step for additional order items (only mains and drinks) from joiners.
        [When(@"they order (\d+) mains and (\d+) drinks")]
        public async Task WhenTheyOrderMainsAndDrinks(int additionalMains, int additionalDrinks)
        {
            // For this step, we assume the joiners order no starters.
            // Update aggregated totals: initial order remains, plus additional joiners' items.
            _totalStarters = _initialStarters; // unchanged
            _totalMains = _initialMains + additionalMains;
            _totalDrinks = _initialDrinks + additionalDrinks;
            _finalOrderTime = _additionalOrderTime; // This will be "20:00"
            await SendAggregatedOrderAsync();
        }

        [Then(@"the API should calculate the final bill correctly by applying the discount only to drinks ordered before 19:00 and should return the correct total bill")]
        public async Task ThenTheAPIFinalBillShouldBeCalculatedCorrectly()
        {
            await CheckoutVerification.VerifyOrderTotalsAsync(_response, _totalStarters, _totalMains, _totalDrinks, _finalOrderTime);
        }
    }
}
