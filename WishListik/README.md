# WishListik Mobile Application

## Overview

The **WishListik** mobile application is a wishlist management app developed using **.NET MAUI** for the Android platform. The app connects to the **WishListikAPI**, enabling users to authenticate, create wishlists, add friends, and reserve items in wishlist within the app.

## Features

### 1. User Authentication
- The app provides user authentication using **Google Sign-In**.
- It ensures secure access to users' accounts and their wishlists.
- The app uses JWT tokens for maintaining user sessions.

### 2. Wishlist Management
- Users can create and manage a list of desired items.
- Each item in the wishlist can have a name, description, and price.
- Users can choose items from an existing database or create custom items.
- Items can be marked as **reserved** to indicate that the gift has been purchased or claimed by someone else.
- Multiple people can reserve one item, allowing for group gifting.

### 3. Social Features
- Users can add friends and interact with their wishlists.
- Friends can view each other’s wishlists and mark items they intend to buy.
- The app ensures that users cannot see who has reserved items on their wishlist, preserving the element of surprise.

### 4. Database Management
- The app uses **Microsoft SQL Server** for storing user data, wishlists, items, and friends.
- The **ApplicationDbContext** class in the API ensures that necessary database tables are created and updated seamlessly.

### 5. Cross-Platform Development with .NET MAUI
- The app is built using **.NET MAUI**, leveraging its cross-platform capabilities.
- Currently, the app is deployed for **Android**, with a user-friendly interface built using native controls.
  
### 6. User Interface (UI) Design
- The UI is designed to be simple and intuitive, utilizing .NET MAUI’s native components for a consistent user experience.
  
### Pages in the App

- **App.xaml**: Core application settings and resources.
- **AppShell.xaml**: Defines the application's navigation structure.
- **AddedFriendsPage.xaml**: Displays a list of friends that the user has added.
- **FriendRequestsPage.xaml**: Displays friend requests the user has received and an input bar to send friend requests.
- **FriendsPage.xaml**: Displays the user's friends.
- **FriendWishlistPage.xaml**: Shows a friend's wishlist, allowing the user to mark items as reserved.
- **LoadingPage.xaml**: Simple loading screen (currently used only when refreshing tokens)
- **LoginPage.xaml**: Allows users to log in using Google Sign-In.
- **MainPage.xaml**: The main page where users can view items and add them to their wishlist.
- **UserPage.xaml**: Displays the authenticated user’s profile information and their wishlist. Here the user can remove items from their wishlist.

### Services in the App

- **AccountService.cs**: Manages user authentication, including token management and Google Sign-In.
- **AuthenticatedServiceBase.cs**: Base service class for authenticated requests.
- **FriendRequestsService.cs**: Handles sending, accepting, and rejecting friend requests.
- **FriendshipsService.cs**: Manages friendships, including viewing and deleting friends.
- **ItemReservationsService.cs**: Handles item reservation functionality within wishlists.
- **ItemsService.cs**: Manages item retrieval and creation.
- **UsersService.cs**: Manages user-related operations like fetching user info.
- **WishlistsService.cs**: Handles operations related to managing wishlists.


## Development Setup

To run this application locally:

1. **Clone the Repository**: Make sure to clone the mobile app's repository.
2. **Set Up the WishListikAPI**: Follow the instructions in the **WishListikAPI README** to set up and run the API.
3. **Connect to the API**: Ensure that the API’s base URL is correctly configured in the app's settings.

## Technologies Used

- **.NET MAUI**: For building the cross-platform mobile application.
- **WishListikAPI**: RESTful ASP.NET API for backend operations.
- **Google Sign-In**: Used for user authentication.


#### Used for the API:
- **Microsoft SQL Server**: Database management system.
- **SQL Server Management Studio**: Used for database interaction and management.

## Images used

- I used icons for the navigation tab from this website. [ionic](https://ionic.io/ionicons)
- The logo was created by me (Jakub Rollo).

(The loading gif was also created by much younger (16/17yo) me, when I played around with GANs (Ganbreeder). I followed this [tutorial](https://www.youtube.com/watch?v=LR5f3-5vB_U&ab_channel=BenMarriott)

