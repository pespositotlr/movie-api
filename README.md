# Kredit Software Engineer Interview

## Overview

Kredit helps people out of debt. One significant step of climbing out of debt is eventually navigating people into settlement. 

The task is to create a payment calculator that lets a person calculate and theoretically split their settlement payments into 4 interest free installments, every two weeks. The first 25% is taken when the settlement payment starts, and the remaining 3 installments of 25% are automatically taken every 14 days. 

### The Challenge

You will build a core service for our business, a Settlement Calculator. 

### User Story

As a Kredit Customer, I would like to establish a payment plan spread over 6 weeks that splits the original charge evenly over 4 installments.

#### Implementation Details

The Code base has two projects
- The Kredit project which has been scaffolded with the standard angular template. The "Service" folder is the code that was added on by the Kredit team.
- The Kredit.Test project which has packages with xunit and a prewritten test.

The test has a few parts
- Updating the PaymentPlanFactory with the correct implemention per specs.
- Add any unit tests you deem necessary. Ensuring they pass.
- Setting up angular / .net REST API to have the screens work (see below).

### Acceptance Criteria (Factory Logic)
- Given it is the 1st of January, 2020
- When I create an order of $100.00
- And I select 4 Installments
- And I select a frequency of 14 days
- Then I should be charged these 4 installments
  - `01/01/2020   -   $25.00`
  - `01/15/2020   -   $25.00`
  - `01/29/2020   -   $25.00`
  - `02/12/2020   -   $25.00`

### Acceptance Criteria (Angular)
Update the angular front-end to have a form with two fields:
- Input for settlement amount
- Submit button

![Form](https://github.com/kreditfinancial/kredit-interview/raw/master/assets/one.png)

Form should talk restfully to the backend and return an appropriate thank-you response page with the scheduled list of payment dates and amounts.

![Thank You](https://github.com/kreditfinancial/kredit-interview/raw/master/assets/two.png)

- The form and thank you pages should have different url routes.
- Apply standard validation in areas you feel are needed.
- Tighten up the integration with a navigation link at the top.
- No unit tests are needed on the angular side.