Feature: Order Cancellation

  Scenario: Cancel entire order
    Given a group of 4 places an order
    When they placed order 4 starters, 4 mains, and 4 drinks.
    Then the API should return the correct total.
    When they cancel the entire order
    Then the API should return "Order canceled" and total should be 0

  Scenario: Cancel partial order 
    Given a group of 4 places an order
    When each of them placed order 2 starters, 1 mains, and 1 drink. 
    Then the API should return the correct total bill. 
    When 1 person cancels their order 
    Then the API total should calculate and return the final bill correctly
