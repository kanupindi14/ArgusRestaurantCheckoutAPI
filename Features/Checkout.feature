Feature: Restaurant Checkout Calculation

  Scenario: Happy flow , Verify that the API correctly calculates the total bill for a group order
    Given a group of 4 places an order before 19:00 
    When they order 4 starters, 4 mains, and 4 drinks
    Then the API should return the correct total bill

  Scenario: Happy flow , Verify that the API correctly calculates the total bill for a group order for order after 19:00
    Given a group of 4 places an order after 19:00 
    When they order 4 starters, 4 mains, and 4 drinks
    Then the API should return the correct total bill
