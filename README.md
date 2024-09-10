# WishListik Final Project

This repository consists of three main projects that together form the **WishListik** application, a mobile and API-based wishlist management system.

## Projects

### 1. [WishListik](./WishListik/)
This directory contains the **WishListik** mobile application, developed using .NET MAUI and currently targeting Android. The application allows users to manage wishlists, add friends, and reserve items for each other. It utilizes the **WishListikAPI** for backend functionality.

- Features:
  - User authentication (Google Sign-In)
  - Wishlist management
  - Social features (friend requests, item reservations)
  - Built for Android using .NET MAUI
  
For more details, see the [WishListik README](./WishListik/README.md).

### 2. [WishListikAPI](./WishListikAPI/)
This directory contains the **WishListikAPI**, a RESTful API developed in ASP.NET that serves as the backend for the **WishListik** mobile application. It provides endpoints for account management, friend requests, wishlists, and item reservations.

- Features:
  - RESTful API with JWT-based authentication
  - Endpoints for account management, wishlists, and friend interactions
  - SQL Server integration with token-based authentication (access and refresh tokens)

For more details, see the [WishListikAPI README](./WishListikAPI/README.md).

### 3. [WishListik-Models](./WishListik-Models/)
This directory contains the **WishListik.Models** project, which includes the model classes representing the entities used in the **WishListikAPI** and the **WishListik** app. These models are mapped to the SQL database tables and manage data for users, wishlists, items, and relationships between them.

For more details, see the [WishListik-Models README](./WishListik-Models/README.md).

---

Each of the projects is interdependent, and together they form the full **WishListik** application, from backend to frontend.
