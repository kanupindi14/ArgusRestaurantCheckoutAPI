using System.Globalization;
using System.Text.Json;
using Microsoft.Playwright;

namespace RestaurantCheckoutTests.Common
{
    public static class CheckoutVerification
    {
        public static async Task VerifyOrderTotalsAsync(IAPIResponse response, int starters, int mains, int drinks, string orderTime)
        {
            double expectedFoodTotal = CommonCheckoutHelper.CalculateExpectedFoodTotal(starters, mains);
            double expectedDrinksTotal = CommonCheckoutHelper.CalculateExpectedDrinksTotal(drinks, orderTime);
            double expectedOrderTotal = expectedFoodTotal + expectedDrinksTotal;

            JsonElement? json = await response.JsonAsync();
            if (!json.HasValue)
            {
                Assert.Fail("Response JSON is null.");
            }

            double foodTotal = Convert.ToDouble(json.Value.GetProperty("food_total").ToString(), CultureInfo.InvariantCulture);
            double drinksTotal = Convert.ToDouble(json.Value.GetProperty("drinks_total").ToString(), CultureInfo.InvariantCulture);
            double orderTotal = Convert.ToDouble(json.Value.GetProperty("order_total").ToString(), CultureInfo.InvariantCulture);

            double tolerance = 0.01;
            Assert.That(foodTotal, Is.EqualTo(expectedFoodTotal).Within(tolerance), "Food total mismatch");
            Assert.That(drinksTotal, Is.EqualTo(expectedDrinksTotal).Within(tolerance), "Drinks total mismatch");
            Assert.That(orderTotal, Is.EqualTo(expectedOrderTotal).Within(tolerance), "Order total mismatch");
        }
    }
}
