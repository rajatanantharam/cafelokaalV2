# CafeLokaal - Functional Requirements

## 1. Overview

CafeLokaal is a SaaS platform for cafes to integrate their POS systems and visualize order processing blockages. The platform includes a secure .NET backend hosted on Azure and a frontend built with Angular. The system is designed to be scalable, GDPR-compliant, and support any number of tenant cafes.

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

- **Response:** A JSON array of Cafe Order Models for the authenticated cafe.

```json
[
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
```

- Backend uses the authenticated user's context to filter orders for their associated CafeId (multi-tenancy enforced).

- **Error Responses:**

  - `401 Unauthorized`: Access token missing or invalid.
  - `403 Forbidden`: User does not have permission to access the data.
  - `500 Internal Server Error`: Unexpected backend failure.

### 3.4 POS Data Integration

- **Rate Limiting:** Maximum 60 requests per minute per cafe. Exceeding this limit will result in a `429 Too Many Requests` response.

#### Integration Instructions for Cafes

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

### 4.1 Frontend (Angular)

- Angular 17+
- Hosted on Azure Static Web Apps or CDN
- MSAL integration for secure token handling

### 4.2 Backend (.NET)

- .NET (Latest Version)
- Hosted on Azure App Service
- Uses Microsoft.Identity.Web for token validation
- Role-based authorization
- Entity Framework with multi-tenant filtering (per CafeId)

### 4.3 Azure Services

- Azure SQL: Data storage (tenant-aware)
- Azure Key Vault: Secure key and secret storage
- Azure AD B2C: Authentication and identity management
- Azure Blob Storage: Archive POS data if needed
- Azure Functions: Process POS data async
- Azure Monitor + App Insights: Logging and telemetry
- Azure Service Bus (optional): Decoupled ingestion and processing

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

## 7. Appendix

- Cafe = Tenant
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

### 7.2 Cafe Order Model

Each order submitted by a cafe includes the following data:

- **OrderId** (GUID): Unique identifier for the order
- **Order States** (object with 3 stages):
  - **Order Received**:
    - `StartTimestamp` (datetime)
    - `EndTimestamp` (datetime)
  - **Order Prepared**:
    - `StartTimestamp` (datetime)
    - `EndTimestamp` (datetime)
  - **Order Served**:
    - `StartTimestamp` (datetime)
    - `EndTimestamp` (datetime)

The frontend application will use these timestamps to visualize where delays or bottlenecks occur in the ordering process.
