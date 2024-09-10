# WishListik.Models

The **WishListik.Models** project contains the model classes used for representing various entities in the **WishListik** application. These models map to database tables and are used throughout the application to handle data such as users, items, wishlists, friendships, and reservations.

## Classes

### 1. FriendRequest.cs
Represents a friend request between two users.
- `FriendRequestId`: Unique identifier for the friend request.
- `RequesterId`: ID of the user who sent the request.
- `RequesteeId`: ID of the user receiving the request.
- Navigation properties: `Requester`, `Requestee`.

### 2. Friendship.cs
Represents a friendship between two users.
- `UserId1`, `UserId2`: IDs of the users in the friendship.
- Navigation properties: `User1`, `User2`.

### 3. Item.cs
Represents an item that can be added to a wishlist.
- `ItemId`: Unique identifier for the item.
- `Name`, `Description`, `Price`: Details about the item.
- Navigation property: `ItemsInWishlist` (relationship with the `ItemInWishlist` class).

### 4. ItemInWishlist.cs
Represents an item within a specific wishlist.
- `ItemId`, `WishlistId`: IDs for the item and wishlist.
- Navigation properties: `Item`, `Wishlist`.
- Contains a collection of `ItemReservations` for group gifting.

### 5. ItemReservation.cs
Represents a reservation made by a user for an item in a wishlist.
- `ItemId`, `WishlistId`, `UserId`: IDs for the reserved item, wishlist, and user.
- Navigation properties: `ItemInWishlist`, `User`.

### 6. RefreshToken.cs
Stores refresh tokens for user authentication.
- `Id`: Unique identifier for the refresh token.
- `UserId`: ID of the user associated with the token.
- `Token`: The refresh token itself.
- `Expiration`: Expiration date of the token.

### 7. User.cs
Represents a user in the application.
- `UserId`: Unique identifier for the user.
- `UserName`, `Email`: Basic user details.
- `RegistrationDate`: Date the user registered.
- Navigation properties: `Wishlist`, `UserMapping`, `FriendshipsAsUser1`, `FriendshipsAsUser2`, `SentFriendRequests`, `ReceivedFriendRequests`, `ItemReservations`.

### 8. UserMapping.cs
Helper class to map external user information (such as Google User ID) to the internal user model.
- `GoogleUserId`: External ID for the user.
- `UserId`: Internal user ID.
- Navigation property: `User`.

### 9. Wishlist.cs
Represents a user's wishlist.
- `WishlistId`, `UserId`: IDs for the wishlist and its owner.
- `Name`, `Description`: Details about the wishlist.
- Navigation properties: `User`, `ItemsInWishlist`.

## Purpose
The models in this project are used to define the structure of the data stored in the **SQL** database, and they serve as the basis for interactions between the **WishListik** app and the **WishListikAPI**. Each model corresponds to a SQL table, and relationships are defined via navigation properties.
