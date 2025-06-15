# Taxually VAT Registration Solution - Daniel Radomirov

## Overview 

This document outlines the approach taken to refactor a VAT registration solution for Taxually.
When I refactored the API, my goal was to make the codebase cleaner, easier to extend, and fully testable. Here is what I changed and why:

## Why I chose a strategy-based design

I replaced the original switch-statement controller with a Strategy Pattern. Each country now has its own class that implements a common IRegistrationStrategy interface. This isolates country-specific logic, keeps the controller slim, and lets me add new countries later without touching existing code. 

| Country        | New strategy class            | What it does                                                        |
| -------------- | ----------------------------- | ------------------------------------------------------------------- |
| United Kingdom | `UkRegistrationStrategy`      | Sends a JSON payload with `IHttpPoster`.                            |
| France         | `FranceRegistrationStrategy`  | Builds a CSV file and queues it through `IQueuePublisher`.          |
| Germany        | `GermanyRegistrationStrategy` | Serialises an XML document and queues it through `IQueuePublisher`. |

## How I wired everything together

I introduced dependency injection so the framework creates all strategies and shared services for me: 

- `IHttpPoster`: Abstract wrapper around `TaxuallyHttpClient`
- `IQueuePublisher`: Abstract wrapper around `TaxuallyQueueClient`- 
- `RegistrationResolver`: iterates over `IEnumerable<IRegistrationStrategy` to find the right strategy at runtime. 

All of these are registered in the `Program.cs`, so the controller only depends on abstractions.

## What I added to make API safer

- `Robust argument checking` – the API now throws explicit exceptions for null or empty values.

- `Graceful error handling` – unsupported country codes return clear NotSupportedExceptions.

## How I made the solution test-friendly 

Every external dependency is behind an interface, so I wrote unit test with XUnit + Moq that: 

- Verify that the UK strategy calls the correct endpoint.

- Check that French and German strategies generate valid CSV/XML and queue the right payload.

- Ensure the RegistrationResolver picks the correct strategy or throws when none is available.

## Room for growth 

Because of the new architecture I can now add a new country by: 

1. Creating a new strategy class that implements `IRegistrationStrategy`.
2. Registering it in `Program.cs`.
3. Writing its unit test. 

No existing code needs to change, exactly what SOLID principles aim for. 

## Conclusion
The refactored solution is cleaner, more modular, and easier to maintain. The Strategy Pattern allows for straightforward extension with new countries, while dependency injection keeps the codebase flexible and testable. The added safety features ensure that the API behaves predictably, making it robust against invalid input. Overall, this approach aligns with best practices in software design and prepares the solution for future growth.

Because the exercise is intentionally small-scale, I decided not to split the code into multiple class-library projects.
Instead, I kept everything inside a single project and used logical namespaces—Domain, Application, and Infrastructure—to reflect Clean-Architecture boundaries.

This approach still gives me:

- SOLID separation of concerns – business rules live in Domain, use-case orchestration in Application, and technology-specific code in Infrastructure.

- Full testability – every external dependency is abstracted behind an interface, so I can mock it from xUnit + Moq without cross-project references.

- -Minimal overhead – one build, one csproj, faster iteration and simpler setup for reviewers.

## Potential enhancements

1. Implement a layered project layout with separate class libraries for Domain, Application, and Infrastructure. This would further enforce separation of concerns and make the solution more modular.
2. Persist registration status in a database. 
3. Add FluentValidation for rich input checks. 
4. Introduce structured logging.
5. Set up integrations test & CI/CD pipeline.