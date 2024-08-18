# Wishlist Application Specification

## General description
A mobile application developed using .NET MAUI that allows users to create a wishlist of items they desire. Users can add friends, who can then mark the items they want to buy for them.

## Features
- **User Authentication:** Users will authenticate (probably by using Google Sign-In), ensuring access to their accounts and wishlist.
- **Wishlist Management:**
  - Users can create and maintain a list of items they wish to have.
  - Each item can include details such as name, description  (and maybe price).
  - (Users will probably be able to choose from an existing items in the databse and also create their own items).
  - Users can mark items as purchased (or remove them) to indicate that the item has already been gifted.
- **Social Features:**
  - Users can add friends within the app.
  - Friends can view each other's wishlists
  - User A can mark wishlisted item of friend B as taken so that someone else doesn't buy it. (User B doesn't see which items are marked so that it won't ruin the surprise.)
- **Database Management:**
  - Utilizes Microsoft SQL Server for storing user accounts, wishlists, and item details.
- **Mobile Platform:**
  - Developed using .NET MAUI for (most probably) iOS
- **User Interface Design**
    - Simple UI design which utilizes .NET MAUI's native controls and UI components

#
I have a lot of other ideas on how to expand this app (such as: creating events (f.e. weddings), grouping up on one gift, creating a calendar for all upcoming events, write down your own suggestions for a gift to a given user etc.) but I think this simpler version described above will be enough work already.

