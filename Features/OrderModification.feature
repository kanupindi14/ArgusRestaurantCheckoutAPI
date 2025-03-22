Feature: Order Modification

  Scenario: Apply 30% discount for drinks ordered before 19:00
    Given a group of 2 places an order at "18:30"
    When they order 1 starter, 2 mains, and 2 drinks
    Then the API should apply a 30% discount on the drinks and should return the correct total bill

  Scenario: Add late joiners after 19:00
    Given a group of 2 places an order at "18:30"
    When they order 1 starter, 2 mains, and 2 drinks
    And 2 more people join the group at "20:00"
    And they order 2 mains and 2 drinks
    Then the API should calculate the final bill correctly, applying the discount only to drinks ordered before 19:00 and should return the correct total bill
