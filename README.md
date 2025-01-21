# Microservices-Based Web API Project

## Overview
This project implements a microservices-based web API consisting of four services:

1. **Customer Service**: Manages customer information.
2. **Contract Service**: Handles contracts associated with customers.
3. **Price Service**: Stores and retrieves product prices.
4. **Order Service**: Processes customer orders and calculates total prices based on product type, customer contracts (if any), and the effective price for the given date.

## Functional Specifications
- If a customer has a contract, the price is determined by the contract and the product type.
- If a customer does not have a contract, the price is determined by the product type only.
- All prices are valid from a specific date, so the calculation must use the price effective on the order date.

---

## Setup Instructions

### Prerequisites
1. Ensure you have an MSSQL Server instance running.
2. Install .NET SDK (version 8.0).
3. Docker installed and configured.

### Step 1: Adjust Connection Strings
Update the connection strings in the following files for each microservice:
- `Program.cs`
- `appsettings.json`
- `DbContext`

Ensure they point to your MSSQL Server instance.

### Step 2: Database Setup
Run the following command for each microservice to create the required tables and databases (using the Entity Framework Core Code-First approach):

```bash
 dotnet ef database update
```

### Step 3: Start RabbitMQ
The microservices use RabbitMQ for communication. To set up RabbitMQ, run:

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4.0-management
```
### Step 4: Run Each Microservice

1. **Navigate & Start Service**:
   - Open a terminal and change to the microservice directory, e.g.:
     ```bash
     cd path/to/CustomerService
     dotnet run
     ```
   - Repeat for each service: `CustomerService`, `ContractService`, `PriceService`, and `OrderService`.

2. **Verify Startup**:
   - Check the console for successful startup messages and ensure each service listens on its configured port.
---

## Usability Instructions

### Create Customers and Contracts
1. **Create Customers**:
   - Create two customers using the Customer Service API.
   - Example payload:
     ```json
     {
       "firstName": "John",
       "lastName": "Doe",
       "email": "johndoe@example.com",
       "vatNumber": "123456789",
       "companyName": "Doe Inc."
     }
     ```

2. **Create a Contract**:
   - Assign a contract with a discount rate to one of the customers.
   - Example payload:
     ```json
     {
       "customerId": "guid",
       "discountRate": 0.10
     }
     ```

3. **Error Handling**:
   - Try using invalid IDs to test error responses and interactions between services.

### Manage Products and Prices
1. **Add New Products**:
   - Add a new product using the Price Service API.
   - Example payload:
     ```json
     {
       "name": "Product1",
       "description": "Description of Product1"
     }
     ```

2. **Set Multiple Price Ranges**:
   - Define multiple price ranges for the product based on effective dates.
   - Example payload:
     ```json
     {
       "value": 99.99,
       "validFrom": "2024-01-01",
       "validTo": "2024-12-31",
       "productId": "guid"
     }
     ```

### Create and Test Orders
1. **Create a New Order**:
   - Create an order with the specified effective date or leave it blank to default to the current date/time.
   - Example payload:
     ```json
     {
       "quantity": 2,
       "customerId": "guid",
       "effectiveDate": "2025-01-21",
       "productId": "guid"
     }
     ```

2. **Verify Price Calculation**:
   - Check how the price is calculated for the newly created order.
   - Ensure the calculation follows the rules for contracts and product types.

---

## Technologies Used
- **C#**: For microservice implementation.
- **Entity Framework Core**: Code-first approach for database management.
- **MSSQL**: For database operations.
- **RabbitMQ**: For inter-service communication.
- **OpenAPI/OData**: For API definition and documentation.
