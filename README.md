# CafeLokaal - SaaS Platform for Cafe Order Processing Analytics

## 1. Overview

CafeLokaal is a multi-tenant SaaS platform that enables cafes to integrate their POS systems and visualize order processing analytics and blockages. The platform consists of a secure .NET 9.0 Web A### 10.1 Terminology

- **Cafe** = Tenant organization in the multi-tenant system
- **Cafe Owner** = Authenticated user with access to their organization's data only
- **Organization** = Business entity (cafe) that owns order data
- **POS** = Point of Sale system that generates order data
- **Blockage** = Delay or bottleneck in order processing workflow
- **Order Step** = Individual stage in the order processing pipeline

### 10.2 Cafe Data Modelkend hosted on Azure and an Angular 18 frontend. The system is designed to be scalable, GDPR-compliant, and supports multiple tenant cafes with organization-based data isolation.

---

## 2. User Roles

### 2.1 Cafe Owner

- Can register and log in via the landing page.
- Can view dashboard analytics for their cafe.
- Can retrieve API access tokens.
- Can manage their own data (view, export, delete per GDPR).

### 2.2 Admin (Internal)

- Can manage tenant onboarding.
- Can view system-wide analytics.
- Can process GDPR data deletion requests manually (if required).

---

## 3. Functional Modules

### 3.1 Landing Page

- Display marketing content and features.
- Call-to-action for creating an account.

### 3.2 Authentication & Authorization

- Powered by Azure AD B2C.
- Supports login, signup, and logout flows.
- Uses MSAL (Angular) and Microsoft.Identity.Web (.NET).
- Role-based access control (CafeOwner, Admin).

### 3.3 Dashboard

- **Rate Limiting:** Maximum 30 requests per minute per cafe for the dashboard data endpoint. Exceeding this limit will result in a `429 Too Many Requests` response.

- Secure area available post-login.

- Includes a **'Sync Data'** button on the dashboard.

  - When clicked, this triggers a call to the `POST /api/posdata` endpoint with the latest data from the connected POS system.
  - This allows manual syncing of POS data to CafeLokaal.
  - A success/failure toast or message should be displayed to the user.

- Visualizes order processing blockages:

- **Rate Limiting:** Maximum 30 requests per minute per cafe for the dashboard data endpoint. Exceeding this limit will result in a `429 Too Many Requests` response.

- Secure area available post-login.

- Visualizes order processing blockages:

  - Delays in POS entry.
  - Queue build-ups.
  - Failed orders.

- Charts powered by Angular libraries like Chart.js or ngx-charts.

- Frontend fetches data from the following endpoint:

```http
GET /api/orders HTTP/1.1
Host: api.cafelokaal.com
Authorization: Bearer {access_token}
```

- **Response:** A JSON array of Cafe Order Models for the authenticated user's organization.

```json
[
  {
    "organizationId": "12345",
    "organizationName": "CafeExample",
    "orderId": "123e4567-e89b-12d3-a456-426614174000",
    "orderStep": "OrderReceived",
    "processTime": 120,
    "processDate": "2025-08-19T08:30:00Z"
  },
  {
    "organizationId": "12345", 
    "organizationName": "CafeExample",
    "orderId": "123e4567-e89b-12d3-a456-426614174000",
    "orderStep": "OrderPrepared",
    "processTime": 300,
    "processDate": "2025-08-19T08:32:00Z"
  }
]
```

- Backend uses the authenticated user's email to lookup their organization via the UserAccess table, ensuring multi-tenancy and data isolation.

- **Error Responses:**

  - `401 Unauthorized`: Access token missing or invalid.
  - `403 Forbidden`: User does not have permission to access the data.
  - `500 Internal Server Error`: Unexpected backend failure.

### 3.4 POS Data Integration (Planned)

- **Rate Limiting:** Maximum 60 requests per minute per cafe. Exceeding this limit will result in a `429 Too Many Requests` response.

**Note**: The POS data integration endpoints are currently planned but not yet implemented. The system currently supports manual data seeding for testing purposes through the `/api/orders/seed` endpoint.

#### Planned Integration Instructions for Cafes

To integrate your POS system with CafeLokaal:

1. **Obtain Access Token**

   - Log into your CafeLokaal dashboard.
   - Navigate to the **API Token Access Page**.
   - Copy your `access_token`. This will be used in the Authorization header of your requests.

2. **Prepare Data Format**

   - Your POS system should generate order data in the format defined in **section 7.2 Cafe Order Model**.
   - Each order must include:
     - Unique `orderId`
     - `orderStates` with timestamps for:
       - `orderReceived`
       - `orderPrepared`
       - `orderServed`

3. **Send Data to CafeLokaal**

   - Use the following API endpoint:

   ```http
   POST /api/posdata HTTP/1.1
   Host: api.cafelokaal.com
   Authorization: Bearer {access_token}
   Content-Type: application/json
   ```

   - **Example request body:**

   ```json
   {
     "orders": [
       {
         "orderId": "123e4567-e89b-12d3-a456-426614174000",
         "orderStates": {
           "orderReceived": {
             "startTimestamp": "2025-07-22T08:30:00Z",
             "endTimestamp": "2025-07-22T08:31:00Z"
           },
           "orderPrepared": {
             "startTimestamp": "2025-07-22T08:31:30Z",
             "endTimestamp": "2025-07-22T08:35:00Z"
           },
           "orderServed": {
             "startTimestamp": "2025-07-22T08:36:00Z",
             "endTimestamp": "2025-07-22T08:40:00Z"
           }
         }
       }
     ]
   }
   ```

4. **Handle Responses**

   - Successful request: `200 OK`
   - Error responses:
     - `400 Bad Request`: Invalid or missing fields in request payload.
     - `401 Unauthorized`: Missing or invalid access token.
     - `403 Forbidden`: Token valid but lacks permission for the resource.
     - `500 Internal Server Error`: Server-side processing failure.



### 3.5 API Token Access Page

- Displays access token with scopes and expiry.
- Supports OAuth2 client credentials flow if required.
- Uses MSAL to retrieve tokens securely.

### 3.6 GDPR Compliance Features

- `GET /api/user-data`: View personal and cafe-specific data.
- `DELETE /api/user-data`: Request deletion of all user-related data.
- `GET /api/user-data/export`: Export data in JSON or CSV.
- Consent banner for EULA

---

## 4. System Requirements

### 4.1 Frontend (Angular 18)

- Angular 18 with TypeScript 5.4.5
- Azure MSAL integration (@azure/msal-angular 3.1.0, @azure/msal-browser 3.28.1)
- Angular Material for UI components
- Chart.js (4.5.0) with chartjs-adapter-date-fns for data visualization
- Hosted on Azure Static Web Apps
- CORS configured for secure communication with API

### 4.2 Backend (.NET 9.0)

- .NET 9.0 Web API
- Hosted on Azure App Service with Docker support
- Uses Microsoft.Identity.Web for JWT token validation
- Entity Framework Core with MySQL (Pomelo provider)
- Multi-tenant data isolation using organization-based filtering
- Application Insights integration for telemetry
- Azure Key Vault integration for secure configuration management

#### Current API Endpoints

- `GET /api/orders` - Retrieve orders for authenticated user's organization
- `POST /api/orders/seed` - Create dummy orders for testing (requires organization name)
- `POST /api/admin/useraccess` - Create user access mapping (admin endpoint)
- `GET /api/admin/useraccess` - Retrieve user access information
- `GET /api/health` - Basic health check
- `GET /api/health/db` - Database connectivity health check

#### Key Dependencies

- Microsoft.Identity.Web (3.11.0) - Azure AD authentication
- Entity Framework Core (8.0.4) - Data access
- Pomelo.EntityFrameworkCore.MySql (8.0.0) - MySQL provider
- Azure.Identity & Azure.Security.KeyVault.Secrets - Azure integration
- Microsoft.ApplicationInsights.AspNetCore - Telemetry

### 4.3 Azure Services & Infrastructure

- **Database**: MySQL hosted on Azure (using Entity Framework Core with Pomelo provider)
- **Compute**: Azure App Service for API hosting with Docker support
- **Frontend**: Azure Static Web Apps for Angular application
- **Authentication**: Azure AD B2C with Microsoft Identity Web integration
- **Monitoring**: Azure Application Insights for telemetry and performance monitoring
- **Security**: Azure Key Vault for secrets management
- **Infrastructure as Code**: Bicep templates for Azure resource provisioning

#### Infrastructure Components

- API App Service with Basic (B1) tier
- MySQL database for multi-tenant data storage
- Web App for Angular frontend hosting
- Resource group organization for environment management

---

## 5. Non-Functional Requirements

### 5.1 Scalability

- Horizontally scalable API and database
- Caching for dashboard queries
- Async POS data handling

### 5.2 Security

- HTTPS enforced
- Role-based access control
- Secure token validation and expiry handling
- Encrypted data at rest and in transit

### 5.3 GDPR Compliance

- Data minimization
- Right to access/export/delete
- Audit logging
- Consent management

---

## 6. Future Enhancements

- Cafe-specific feature flags
- Multi-region sharding of databases
- Data lake integration for historical analytics
- Admin panel for tenant management

---

## 10. Appendix

### 10.1 Terminology
- Cafe Owner = Authenticated user with access to their cafe’s data only
- POS = Point of Sale system
- Blockage = Delay or failure in order processing

### 7.1 Cafe Data Model

Each cafe in the system includes the following information:

- **CafeId** (GUID): Unique identifier for the cafe
- **TenantId** (string): Identifier used for multi-tenancy
- **Primary Contact** (object):
  - **First Name** (string)
  - **Last Name** (string)
  - **Email** (string)
  - **Address** (string)
  - **Telephone Number** (string)

### 10.3 Cafe Order Model (Current Implementation)

The current implementation uses a simplified order model that tracks individual order steps:

- **OrderId** (string): Unique identifier for the order
- **OrganizationId** (string): Unique identifier for the cafe/organization
- **OrganizationName** (string): Display name of the organization
- **OrderStep** (OrderStep enum): Current step in the order process
  - `OrderReceived`: Order has been received by the system
  - `OrderPrepared`: Order has been prepared
  - `OrderServed`: Order has been served to customer
  - `Unknown`: Default/error state
- **ProcessTime** (int): Time taken for this step in seconds/minutes
- **ProcessDate** (DateTime): Timestamp when this step was completed

**Note**: The current implementation differs from the original specification which included nested order states with start/end timestamps. The current model tracks individual steps with process times, which provides a simpler data structure for analytics while still enabling blockage detection and performance analysis.

### 10.4 User Access Model

The platform uses a UserAccess model for multi-tenant access control:

- **Email** (string): User's email address (from Azure AD claims)
- **OrganizationName** (string): Name of the organization the user belongs to
- **SubscriptionId** (string): Azure subscription identifier for the organization

This model enables organization-based data isolation, ensuring users can only access data for their assigned organization.

---

## 8. Project Structure

### 8.1 Solution Organization

```
CafeLokaal.sln                    # Main solution file
├── CafeLokaal.Api/               # .NET 9.0 Web API
│   ├── Controllers/              # API controllers
│   │   ├── OrdersController.cs   # Order management endpoints
│   │   ├── AdminController.cs    # Administrative endpoints
│   │   └── HealthController.cs   # Health check endpoints
│   ├── Data/                     # Data access layer
│   │   ├── CafeLokaalContext.cs  # Entity Framework DbContext
│   │   ├── DBContextResolver.cs  # Multi-tenant context resolution
│   │   ├── OrderRepository.cs    # Order data operations
│   │   └── UserAccessRepository.cs # User access management
│   ├── Models/                   # Data models
│   │   ├── CafeOrder.cs         # Order entity model
│   │   └── UserAccess.cs        # User access entity model
│   ├── Dockerfile               # Docker containerization
│   └── docker-compose.yml       # Local development setup
├── CafeLokaal.Web/              # Angular 18 frontend
│   ├── src/app/                 # Angular application
│   │   ├── core/                # Core services and guards
│   │   └── features/            # Feature modules
│   └── package.json             # Frontend dependencies
└── Infra/                       # Infrastructure as Code
    ├── API/                     # API infrastructure
    ├── Databases/               # Database setup
    └── Webapp/                  # Frontend hosting
```

### 8.2 Key Architecture Decisions

- **Multi-tenancy**: Organization-based data isolation using UserAccess table
- **Authentication**: Azure AD B2C with JWT bearer tokens
- **Database**: MySQL with Entity Framework Core and Pomelo provider
- **Frontend**: Angular 18 with MSAL for Azure integration
- **Infrastructure**: Bicep templates for Azure resource management
- **Containerization**: Docker support for API deployment

---

## 9. Development & Deployment

### 9.1 Local Development

#### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ and npm
- Angular CLI 18
- MySQL database (local or Azure-hosted)
- Azure AD B2C tenant configuration

#### API Development
```bash
cd CafeLokaal.Api
dotnet restore
dotnet run
```

#### Frontend Development
```bash
cd CafeLokaal.Web
npm install
ng serve
```

### 9.2 Docker Support

The API includes Docker support with multi-stage builds:

```dockerfile
# Build stage with .NET 9.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Runtime stage with ASP.NET Core 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0
```

### 9.3 Infrastructure Deployment

The project includes Bicep templates for Infrastructure as Code:

- `Infra/API/cafelokaal-api.bicep` - API App Service provisioning
- `Infra/Databases/mysql-database.bicep` - MySQL database setup
- `Infra/Webapp/cafelokaal-webapp.bicep` - Frontend hosting

### 9.4 Current Limitations & TODOs

- Rate limiting implementation is documented but not yet implemented
- POS data integration endpoints need to be developed
- GDPR compliance endpoints are planned but not implemented
- Comprehensive testing suite needs to be added
- API token management functionality is not yet implemented
