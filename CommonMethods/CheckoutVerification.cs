using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace RestaurantCheckoutTests.Common
{
    public static class CheckoutVerification
    {
        public static async Task VerifyOrderTotalsAsync(IAPIResponse response, int starters, int mains, int drinks, string orderTime)
        {
            double expectedFoodTotal = CommonCheckoutHelper.CalculateExpectedFoodTotal(starters, mains);
            double expectedDrinksTotal = CommonCheckoutHelper.CalculateExpectedDrinksTotal(drinks, orderTime);
            double expectedOrderTotal = expectedFoodTotal + expectedDrinksTotal;

            var json = await response.JsonAsync();
            double foodTotal = Convert.ToDouble(json.GetProperty("food_total").ToString(), CultureInfo.InvariantCulture);
            double drinksTotal = Convert.ToDouble(json.GetProperty("drinks_total").ToString(), CultureInfo.InvariantCulture);
            double orderTotal = Convert.ToDouble(json.GetProperty("order_total").ToString(), CultureInfo.InvariantCulture);

            double tolerance = 0.01;
            Assert.AreEqual(expectedFoodTotal, foodTotal, tolerance, "Food total mismatch");
            Assert.AreEqual(expectedDrinksTotal, drinksTotal, tolerance, "Drinks total mismatch");
            Assert.AreEqual(expectedOrderTotal, orderTotal, tolerance, "Order total mismatch");
        }
    }
}
