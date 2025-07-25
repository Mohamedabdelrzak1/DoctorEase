# DoctorEase API Project

Welcome to DoctorEase, a scalable and modular web API built using ASP.NET Core Web API for clinic and medical appointment management. This project focuses on providing core functionalities for a clinic management system, including patient management, booking system, medical articles, testimonials, and medical cases with image uploads.

This API is designed with clean architecture principles, repository pattern, dependency injection, and JWT-based authentication to ensure scalability, maintainability, and security.

🚀 Key Features
Patient Management: Full CRUD operations for managing patients.

Booking Management: Create, update, and track medical appointments with status control.

Article Management: Add, update, and manage medical articles with image upload.

Testimonials: Manage and display patient testimonials and feedback.

Medical Cases Management: Create and manage medical cases with before-and-after images.

User Authentication: JWT-based login with secure password hashing and reset password support.

Role-Based Access: Admin and user roles to control access levels.

Email Service: Send reset password links using MailKit.

Image Upload: Upload and manage images linked to articles and medical cases.

Clean Architecture: Separation of concerns across layers with repository pattern and service layer.

Validation: Input validation using data annotations and structured DTOs.

Entity Framework Core: Seamless database integration with migrations and LINQ queries.

API Documentation: Fully documented using Swagger UI for interactive API testing.

📦 Technologies Used
ASP.NET Core Web API

Entity Framework Core

SQL Server

JWT Authentication

Swagger

AutoMapper

LINQ

MailKit

Clean Architecture

🛠️ Project Highlights

✅ Authentication: JWT-based login, secure password hashing, and reset password via email.

✅ Admin Dashboard: Manage articles, patients, bookings, testimonials, and medical cases from structured endpoints.

✅ File Upload: Efficient image handling and storage under wwwroot/images.

✅ Statistics Endpoint: Retrieve overall system metrics including total bookings, patients, and testimonials.

✅ Validation: Comprehensive input validation for all endpoints.

✅ Error Handling: Consistent error response models using a global error handling middleware.

✅ Modular Design: Easily extendable structure for adding more clinic management features.

Future Enhancements
Appointment reminders via email or SMS.

Payment integration for online booking.

Advanced admin dashboard for charts and analytics.

User notifications and announcements.

🤝 Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change or add.

🛡️ License
This project is licensed under the MIT License.



