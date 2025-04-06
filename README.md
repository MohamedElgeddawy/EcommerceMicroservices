# Order & Product API Documentation

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Order API](#order-api)
   - [Endpoints](#order-api-endpoints)
   - [Schemas](#order-api-schemas)
4. [Product API](#product-api)
   - [Endpoints](#product-api-endpoints)
   - [Schemas](#product-api-schemas)
5. [Images & Diagrams](#images--diagrams)

---

## Overview
This documentation provides an overview of the **Order API** and **Product API** services, their endpoints, and how they interact with each other.

---

## Architecture
Below is the sequence diagram illustrating how the Order API interacts with the Product API and the database.

### Order Processing Flow
![Architecture Diagram](Diagram.PNG)

- The client places an order.
- The **Order API** authenticates the user.
- It checks inventory via the **Product API**.
- If inventory is available, the order is created.
- If inventory is unavailable, an error is returned.

### Sequence Diagram
![Sequence Diagram](TP9DRW8n38NtEON52gHo0HQ8dzcCLBMqg0SmCww86XBgE4ZSlj4a8ZD1tH35_lpodfcD5sG95wEkyTITlg8Ls1jDLYQbsJltnB0zj3D03yDj2XpOzQ6J1vBlK-XuHgOUNlM9aUy3bqfJh4714teQW6QdL784otLwUGK7Wnsb2ypTSFGag8rVg5IDR99UmJ.png)

---

## Order API
The **Order API** manages customer orders. The following endpoints are available:

### Order API Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/Orders` | Retrieve all orders |
| **POST** | `/api/Orders` | Create a new order |
| **PUT** | `/api/Orders` | Update an existing order |
| **DELETE** | `/api/Orders` | Delete an order |
| **GET** | `/api/Orders/{id}` | Retrieve order by ID |
| **GET** | `/api/Orders/client/{clientId}` | Retrieve orders by client ID |
| **GET** | `/api/Orders/details/{orderId}` | Retrieve order details |

![Order API Swagger](OrderApiPNG.PNG)

### Order API Schemas
- **OrderDTO**: Defines the structure of an order request.
- **OrderDetailsDTO**: Contains details of an order.
- **Response**: Standard API response format.

---

## Product API
The **Product API** manages product inventory. The following endpoints are available:

### Product API Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/Products` | Retrieve all products |
| **POST** | `/api/Products` | Add a new product |
| **PUT** | `/api/Products` | Update product details |
| **DELETE** | `/api/Products` | Remove a product |
| **GET** | `/api/Products/{id}` | Retrieve product by ID |

![Product API Swagger](ProductApi.PNG)

### Product API Schemas
- **ProductDTO**: Defines the structure of a product request.
- **Response**: Standard API response format.

---

## Images & Diagrams
Below are the relevant images included in this documentation:
1. **Architecture Diagram**: `Diagram.PNG`
2. **Sequence Diagram**: `Sequence Diagram.png`
3. **Order API Swagger Documentation**: `OrderApiPNG.PNG`
4. **Product API Swagger Documentation**: `ProductApi.PNG`



