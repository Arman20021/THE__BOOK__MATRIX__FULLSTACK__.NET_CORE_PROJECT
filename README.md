# The Book Matrix
http://thebookmatrix.tryasp.net/ 

A professional ASP.NET Core MVC book shop management system built with a layered architecture, SQL Server Database First approach, session-based authentication, email verification, admin book management, shopping cart, SSLCommerz sandbox payment, and sales analytics dashboard.

## Project Overview

**The Book Matrix** is an online book shop management application where users can register, verify their email, browse books, search and filter products, add books to a cart, and complete payment through SSLCommerz sandbox. Admin users can manage books, categories, users, orders, payments, and sales statistics.

The project follows a clean beginner-friendly flow:

```text
View → Controller → Service → Repository → DbContext → Database
```

The solution is organized into three main layers:

```text
MVC → BLL → DAL
```

| Layer | Responsibility |
|---|---|
| MVC | Controllers, Razor views, user interface, routing, session handling |
| BLL | DTOs, services, business rules, data preparation for views |
| DAL | Entity Framework models, DbContext, repositories, database operations |

## Main Features

### Authentication and Authorization

- User registration and login
- Session-based login and logout
- Email verification using verification tokens
- Role-based access control
- Separate access for Admin and User roles
- Inactive users cannot log in when `IsActive` is checked in login logic

### User Features

- Browse active books after login
- Search books by title, author, description, or category
- Filter books by category
- Filter books by minimum and maximum price
- Sort books alphabetically or by price
- Select book quantity
- Add books to cart
- View cart with line totals and grand total
- Update cart quantity
- Remove cart items
- Pay through SSLCommerz sandbox

### Admin Features

- Admin dashboard
- Manage books
- Add, edit, and soft-delete books
- Upload book images
- Manage users
- Add users manually
- Edit user information
- Activate or deactivate users
- View sales statistics
- View paid, pending, failed, and cancelled orders
- View most sold books
- View recent orders

### Payment Features

- SSLCommerz sandbox payment integration
- Pending order creation before payment
- Permanent order item history
- Payment validation using `val_id`
- Duplicate payment prevention
- Paid cart status update
- Success, fail, cancel, and IPN payment flows

## Technology Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core MVC |
| Language | C# |
| Database | SQL Server |
| ORM | Entity Framework Core, Database First |
| UI | Razor Views, Bootstrap |
| Authentication | Session-based authentication |
| Email | SMTP email service |
| Payment Gateway | SSLCommerz Sandbox |
| Architecture | Repository-Service-Controller pattern |

## Project Structure

```text
The_Book_Matrix
│
├── The_Book_Matrix
│   ├── Controllers
│   │   ├── AuthController.cs
│   │   ├── HomeController.cs
│   │   ├── AdminDashboardController.cs
│   │   ├── AdminBookController.cs
│   │   ├── AdminUserController.cs
│   │   ├── CartController.cs
│   │   └── PaymentController.cs
│   │
│   ├── Views
│   │   ├── Auth
│   │   ├── Home
│   │   ├── AdminDashboard
│   │   ├── AdminBook
│   │   ├── AdminUser
│   │   ├── Cart
│   │   └── Payment
│   │
│   ├── wwwroot
│   │   └── images
│   │       └── books
│   │
│   ├── appsettings.json
│   └── Program.cs
│
├── BLL
│   ├── DTOs
│   │   ├── RegisterDto.cs
│   │   ├── LoginDto.cs
│   │   ├── AuthResultDto.cs
│   │   ├── BookDto.cs
│   │   ├── CategoryDto.cs
│   │   ├── CartDto.cs
│   │   ├── CartItemDto.cs
│   │   ├── ResultDto.cs
│   │   ├── SalesSummaryDto.cs
│   │   └── AdminUserDto.cs
│   │
│   ├── Services
│   │   ├── AuthService.cs
│   │   ├── EmailService.cs
│   │   ├── BookService.cs
│   │   ├── CategoryService.cs
│   │   ├── CartService.cs
│   │   ├── AdminDashboardService.cs
│   │   └── AdminUserService.cs
│   │
│   └── AutoMapperProfiles
│       └── MappingProfile.cs
│
└── DAL
    ├── EF
    │   ├── BookShopDBContext.cs
    │   └── Tables
    │       ├── User.cs
    │       ├── Role.cs
    │       ├── EmailVerificationToken.cs
    │       ├── Category.cs
    │       ├── Book.cs
    │       ├── Cart.cs
    │       ├── CartItem.cs
    │       ├── CustomerOrder.cs
    │       ├── CustomerOrderItem.cs
    │       └── Payment.cs
    │
    └── Repositories
        ├── UserRepository.cs
        ├── EmailVerificationRepository.cs
        ├── CategoryRepository.cs
        ├── BookRepository.cs
        ├── CartRepository.cs
        ├── PaymentRepository.cs
        └── AdminUserRepository.cs
```

## Database Tables

The system uses the following main database tables:

| Table | Purpose |
|---|---|
| Roles | Stores user roles such as Admin and User |
| Users | Stores user account information |
| EmailVerificationTokens | Stores email verification tokens |
| Categories | Stores book categories |
| Books | Stores book details, stock, price, and image path |
| Carts | Stores active or paid user carts |
| CartItems | Stores books added to carts |
| CustomerOrders | Stores order and payment attempt information |
| CustomerOrderItems | Stores purchased book details permanently |
| Payments | Stores SSLCommerz payment response data |

## Important Business Rules

- New registered users are assigned the normal User role.
- Users must verify email before full login access.
- Admin users are redirected to the admin dashboard.
- Normal users are redirected to the book home page.
- Users can only see active books.
- Book deletion uses soft delete by setting `IsActive = false`.
- If an inactive or deleted book exists in a cart, it is removed from cart items.
- Cart data is temporary, but order item data is stored permanently.
- Only paid orders are counted in sales statistics.
- Payment success is accepted only after SSLCommerz validation.
- IPN testing on localhost needs a public URL, such as ngrok or live hosting.

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/The_Book_Matrix.git
cd The_Book_Matrix
```

### 2. Configure the Database

Create a SQL Server database named:

```text
BookShopDB
```

Then create the required tables:

```text
Roles
Users
EmailVerificationTokens
Categories
Books
Carts
CartItems
CustomerOrders
CustomerOrderItems
Payments
```

### 3. Update `appsettings.json`

Replace the placeholder values with your own local settings.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BookShopDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },

  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "Email": "your-email@gmail.com",
    "Password": "your-app-password",
    "DisplayName": "Book Shop Management"
  },

  "AppSettings": {
    "BaseUrl": "https://localhost:7283"
  },

  "SslCommerz": {
    "StoreId": "testbox",
    "StorePassword": "qwerty",
    "SessionApiUrl": "https://sandbox.sslcommerz.com/gwprocess/v4/api.php",
    "ValidationApiUrl": "https://sandbox.sslcommerz.com/validator/api/validationserverAPI.php"
  }
}
```


## Application Workflows

### Registration and Email Verification

```text
User opens Register page
↓
User submits registration form
↓
AuthController receives RegisterDto
↓
AuthService checks duplicate email
↓
User is saved with RoleId = User
↓
Verification token is created
↓
EmailService sends verification email
↓
User clicks verification link
↓
Token is validated
↓
User email is marked as verified
```

### Login Flow

```text
User submits login form
↓
AuthController sends LoginDto to AuthService
↓
AuthService checks email, password, verification, and active status
↓
Session stores UserId, FullName, Email, and RoleName
↓
Admin goes to Admin Dashboard
↓
Normal User goes to Home page
```

### Book Management Flow

```text
Admin login
↓
Admin Dashboard
↓
Manage Books
↓
Add, edit, upload image, or soft-delete book
↓
BookService applies business rules
↓
BookRepository saves changes
↓
Database updates book records
```

### Cart Flow

```text
User opens Home page
↓
User searches or filters books
↓
User selects quantity
↓
User adds book to cart
↓
CartService checks stock and active status
↓
CartRepository creates or updates cart item
↓
User views cart and grand total
```

### Payment Flow

```text
User clicks Pay Now
↓
PaymentController creates pending order
↓
Cart items are copied into CustomerOrderItems
↓
System sends payment request to SSLCommerz sandbox
↓
User completes payment
↓
SSLCommerz redirects to Success, Fail, or Cancel action
↓
System validates payment using val_id
↓
If valid, order becomes Paid and cart becomes Paid
↓
Payment record is saved
```

### Admin Sales Dashboard Flow

```text
Admin opens dashboard
↓
AdminDashboardController checks Admin role
↓
AdminDashboardService loads order and payment data
↓
PaymentRepository returns paid and pending order data
↓
Dashboard shows sales statistics, most sold books, and recent orders
```

## Search and Filter Options

Users can search and filter books from the Home page.

| Option | Description |
|---|---|
| Search text | Searches title, author, description, and category |
| Category | Shows books from selected category |
| Minimum price | Shows books equal to or above selected price |
| Maximum price | Shows books equal to or below selected price |
| Name A-Z | Sorts books alphabetically |
| Name Z-A | Sorts books reverse alphabetically |
| Price low to high | Sorts books by ascending price |
| Price high to low | Sorts books by descending price |


## Future Improvements

- Add ASP.NET Core Identity
- Hash passwords securely
- Add order history for users
- Add invoice generation
- Add product reviews and ratings
- Add stock deduction after successful payment
- Add pagination for book listing
- Add API endpoints for mobile or frontend apps
- Add unit tests for services
- Add admin charts for sales analytics

## Author

Developed by **MD.ARMAN ISALM**.

## License

This project is for academic and learning purposes. Add a license file before publishing it as an open-source project.
