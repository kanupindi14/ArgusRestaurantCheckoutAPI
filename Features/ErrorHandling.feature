Feature: Checkout API - Handle Zero and Negative Quantities

  Scenario: Reject zero quantity order
    Given a group of 3 places an order
    When they order 0 starters, 0 mains, and 0 drink
    Then the API should return an error message "Invalid order"

  Scenario: Reject negative quantities in order
    Given a group of 2 places an order
    When they order -1 starters, 2 mains, and 1 drink
    Then the API should return an error message "Invalid quantity"
