const express = require('express');
const app = express();

app.use(express.json());

app.post('/calculate-total', (req, res) => {
    const data = req.body;
    const starters = data.starters || 0;
    const mains = data.mains || 0;
    const drinks = data.drinks || 0;
    const orderTimeStr = data.order_time; // Expected format "HH:MM"

    // Error handling: reject zero quantity orders
    if (starters === 0 && mains === 0 && drinks === 0) {
        return res.status(400).json({ error: "Invalid order" });
    }
    // Reject negative quantities
    if (starters < 0 || mains < 0 || drinks < 0) {
        return res.status(400).json({ error: "Invalid quantity" });
    }

    let orderHour = null;
    if (orderTimeStr) {
        const timeParts = orderTimeStr.split(':');
        if (timeParts.length !== 2) {
            return res.status(400).json({ error: 'Invalid time format. Use HH:MM.' });
        }
        orderHour = parseInt(timeParts[0], 10);
        if (isNaN(orderHour)) {
            return res.status(400).json({ error: 'Invalid time format. Use HH:MM.' });
        }
    }

    // Set fixed costs.
    const costStarter = 4.00;
    const costMain = 7.00;
    const costDrink = 2.50;

    // Calculate food total (with 10% service charge).
    const foodTotal = ((starters * costStarter) + (mains * costMain)) * 1.1;

    // Calculate drinks total (apply 30% discount if before 19:00).
    let drinkPrice = costDrink;
    if (orderHour !== null && orderHour < 19) {
        drinkPrice = costDrink * 0.7;
    }
    const drinksTotal = drinks * drinkPrice;

    // Final order total.
    const orderTotal = foodTotal + drinksTotal;

    res.json({
        food_total: parseFloat(foodTotal.toFixed(2)),
        drinks_total: parseFloat(drinksTotal.toFixed(2)),
        order_total: parseFloat(orderTotal.toFixed(2))
    });
});

app.post('/cancel-order', (req, res) => {
    // Return cancellation response.
    res.json({
        message: "Order canceled",
        order_total: 0
    });
});

const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
