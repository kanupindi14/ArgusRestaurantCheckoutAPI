# ArgusRestaurantCheckoutAPI

ArgusRestaurantCheckoutAPI is a Behavior-Driven Development (BDD) project for a restaurant checkout system. The project demonstrates how to implement a checkout system using a Node.js mock API and automate its behavior with SpecFlow, c#, and Playwright.

## Overview

This project implements a restaurant checkout system with the following business rules:
- **Starters** cost £4.00 each.
- **Mains** cost £7.00 each.
- **Drinks** cost £2.50 each, with a 30% discount if ordered before 19:00.
- A **10% service charge** is applied on food items (starters and mains).

BDD feature files (written in Gherkin) describe the expected behaviors. Automated tests validate the API and serve as living documentation for the system’s behavior.

## Assumptions

- The mock API is built with Node.js and Express and listens on port 5000 by default.
- Automated tests are implemented using SpecFlow, C#, and Playwright.
- The project targets .NET 6.0 (or a compatible version) for test automation.

## Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/kanupindi14/ArgusRestaurantCheckoutAPI
   cd ArgusRestaurantCheckoutAPI
   ```   
2. **Restore .NET Packages**

   ```bash
    dotnet restore
    ```
  
3. **Build the Project**

   ```bash
    dotnet build
    ```
## Running the Mock API

1. **Navigate to the API Folder**

  ```bash
cd MockAPI
```
2. **Install Node.js Dependencies**

  ```bash
 npm install
```
3. **Start the API Server**

  ```bash
  npm start
```
_The server will run on port 5000 by default._

## Running Tests
From the project root, run the tests

  ```bash
    dotnet test --logger "trx;LogFilePath=TestResults\myTestResults.trx"
  ```

## BDD Approach
### -  Feature Files:
Located in the Features folder, these Gherkin files describe the expected behavior of the checkout system in a language that is accessible to both technical and non-technical team members.

### -  Step Definitions:
Implemented in C#, the step definitions map the Gherkin steps to automated tests using SpecFlow and NUnit.

## Next Steps

### -  Implement Reporting:
In future iterations, integrate a reporting solution (such as SpecFlow+ LivingDoc or another reporting tool) to produce interactive HTML reports with test outcomes and traces.

### -  Enhance Middleware Logging:
Improve the Node.js API by adding middleware that logs each request and response in the console. Consider tagging each request with a unique identifier or scenario header for easier traceability.

### -  Add More Edge Cases & Load Testing:
_Expand the test suite to cover additional edge cases such as:_

- Invalid or missing input values.

- Concurrent orders from multiple users.

- Stress or load testing scenarios to assess system performance under heavy load.

_These enhancements will ensure the system’s robustness and scalability._

## Additional Notes

### Environment Requirements:

-  .NET SDK (preferably .NET 6.0 or a compatible version)

-  Node.js & npm

-  Playwright (for API tests)


