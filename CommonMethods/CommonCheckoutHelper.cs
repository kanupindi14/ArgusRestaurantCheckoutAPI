using System;

namespace RestaurantCheckoutTests.Common
{
    public static class CommonCheckoutHelper
    {
        // Calculate food total with a 10% service charge: (starters * £4 + mains * £7) * 1.1
        public static double CalculateExpectedFoodTotal(int starters, int mains)
        {
            double rawFood = (starters * 4.0) + (mains * 7.0);
            return rawFood * 1.1;
        }

        // Calculate drinks total: if orderTime is before 19:00 apply a 30% discount; otherwise full price
        public static double CalculateExpectedDrinksTotal(int drinks, string orderTime)
        {
            double drinkCost = 2.5;
            if (!string.IsNullOrEmpty(orderTime) && int.TryParse(orderTime.Split(':')[0], out int hour))
            {
                if (hour < 19)
                {
                    return (drinks * drinkCost) * 0.7;
                }
            }
            return drinks * drinkCost;
        }

        // Overall order total is the sum of the food and drinks totals
        public static double CalculateExpectedOrderTotal(int starters, int mains, int drinks, string orderTime)
        {
            return CalculateExpectedFoodTotal(starters, mains) + CalculateExpectedDrinksTotal(drinks, orderTime);
        }
    }
}
