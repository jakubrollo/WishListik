# WishListik API

Welcome to the WishListik API documentation. This document provides an overview of the API endpoints available for managing user accounts, friend requests, friendships, item reservations, items, and wishlists.

## Overview

The WishListik API is built using ASP.NET Core and follows RESTful principles. It provides a set of HTTP endpoints that enable interaction with the application's data. ASP.NET Core is a powerful framework for building modern web applications and APIs, offering robust support for RESTful services, authentication, and data management.

## Base URL

All endpoints are relative to the base URL. For local development, I used a tool called ngrok to expose my local API. After running ngrok, the base URL will be the forwarding address provided by ngrok (e.g., `https://abc123.ngrok.io`). Update this URL in the Google's OAuth 2.0 authorized URLs as needed.

## Authentication

The API supports authentication using JWT Bearer tokens. For Google authentication, you can use the `/Account/signin-google` endpoint. Tokens are required for accessing most endpoints. The API returns both access and refresh tokens:

- **Access Token:** Used for authorizing requests to the API. It has a limited lifespan and must be included in the `Authorization` header of requests.
- **Refresh Token:** Used to obtain a new access token when the current one expires. It is exchanged at the `/Account/refresh` endpoint for a new access token.

## Endpoints

### Account

- **GET** `/Account/login`
  - Logs in a user. (Currently just redirects to Google sign-in)
- **GET** `/Account/signin-google`
  - Initiates Google authentication for user sign-in.
- **POST** `/Account/refresh`
  - Refreshes the tokens using a valid refresh token.

### FriendRequests

- **POST** `/api/FriendRequests`
  - Sends a friend request to a specified user.
- **POST** `/api/FriendRequests/accept`
  - Accepts a pending friend request.
- **POST** `/api/FriendRequests/reject`
  - Rejects a pending friend request.
- **GET** `/api/FriendRequests/pending`
  - Retrieves a list of pending friend requests of the authenticated user.

### Friendships

- **GET** `/api/Friendships/me`
  - Retrieves the list of friends for the authenticated user.
- **GET** `/api/Friendships/{friendId}`
  - Retrieves the wishlist of a specified friend.
- **DELETE** `/api/Friendships/{friendId}`
  - Deletes a friendship with the specified friend.

### ItemReservations

- **GET** `/api/ItemReservations/{itemId}/{wishlistId}`
  - Retrieves a list of users who have reserved a specific item in a given wishlist.
- **DELETE** `/api/ItemReservations/{itemId}/{wishlistId}`
  - Cancels the reservation for a specific item in a given wishlist.
- **POST** `/api/ItemReservations`
  - Reserves an item in a user's wishlist.

### Items

- **GET** `/api/Items`
  - Retrieves a list of all items.
- **POST** `/api/Items`
  - Adds a new item.
- **GET** `/api/Items/{id}`
  - Retrieves details of a specific item.

### Users

- **GET** `/api/Users/me`
  - Retrieves details of the currently authenticated user.
- **GET** `/api/Users/me/id`
  - Retrieves the ID of the currently authenticated user.

### Wishlists

- **GET** `/api/Wishlists/me`
  - Retrieves the wishlist of the currently authenticated user.
- **POST** `/api/Wishlists/me/items`
  - Adds an item to the authenticated user's wishlist.
- **DELETE** `/api/Wishlists/me/items/{itemId}`
  - Removes an item from the authenticated user's wishlist.

## Development and Testing

To run and test the API locally:

1. Clone the repository.
2. Set up your database connection and update "JwtSettings" in `appsettings.json`.
3. Run the application.
4. Optionally, use ngrok to expose your local API for external testing. Start ngrok with a command like `ngrok http https://localhost:7191/`, and use the provided ngrok URL as your base URL for API requests (useful for testing with emulators).

## Database Setup

The `ApplicationDbContext` class in the API is configured to work with SQL Server. It will automatically generate the necessary database tables based on the entity models defined in your application. To manage the SQL Server database, I used  Microsoft SQL Server and Microsoft SQL Server Management Studio (SSMS).

