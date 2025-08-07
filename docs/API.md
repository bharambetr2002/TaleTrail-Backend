# 📚 TaleTrail API Documentation

Complete API reference for TaleTrail book tracking application.

## 🌐 Base URL

- **Development**: `http://localhost:5198`
- **Production**: `https://your-app.onrender.com`

## 🔐 Authentication

TaleTrail uses JWT (JSON Web Token) based authentication with Supabase Auth.

### Authentication Headers

For protected endpoints, include the JWT token in the Authorization header:

```http
Authorization: Bearer <your_jwt_token>
```

### Token Expiry

Tokens expire after 1 hour. You'll need to handle token refresh in your client application.

---

## 📋 API Endpoints Overview

| Category           | Endpoints   | Authentication |
| ------------------ | ----------- | -------------- |
| **Authentication** | 2 endpoints | ❌ Public      |
| **Books**          | 3 endpoints | ❌ Public      |
| **Authors**        | 2 endpoints | ❌ Public      |
| **Publishers**     | 2 endpoints | ❌ Public      |
| **User Books**     | 4 endpoints | ✅ Protected   |
| **Reviews**        | 4 endpoints | Mixed          |
| **Blogs**          | 5 endpoints | Mixed          |
| **Blog Likes**     | 3 endpoints | ✅ Protected   |
| **User Profile**   | 3 endpoints | Mixed          |

---

## 🔑 Authentication Endpoints

### Register User

**POST** `/api/auth/signup`

Register a new user account.

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "securePassword123",
  "fullName": "John Doe",
  "username": "johndoe"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Account created successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": "uuid",
      "email": "user@example.com",
      "fullName": "John Doe",
      "username": "johndoe",
      "bio": null,
      "avatarUrl": null,
      "createdAt": "2024-01-07T10:00:00Z",
      "updatedAt": "2024-01-07T10:00:00Z"
    }
  }
}
```

### Login User

**POST** `/api/auth/login`

Authenticate an existing user.

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": "uuid",
      "email": "user@example.com",
      "fullName": "John Doe",
      "username": "johndoe",
      "bio": "Book lover and avid reader",
      "avatarUrl": "https://example.com/avatar.jpg",
      "createdAt": "2024-01-07T10:00:00Z",
      "updatedAt": "2024-01-07T10:00:00Z"
    }
  }
}
```

---

## 📖 Book Endpoints

### Get All Books

**GET** `/api/book`

Retrieve all books with optional search functionality.

**Query Parameters:**

- `search` (optional): Search term for book titles

**Example Requests:**

```http
GET /api/book
GET /api/book?search=harry potter
```

**Response:**

```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "id": "uuid",
      "title": "Harry Potter and the Philosopher's Stone",
      "description": "The first novel in the Harry Potter series...",
      "language": "English",
      "coverImageUrl": "https://example.com/cover.jpg",
      "publicationYear": 1997,
      "publisherId": "uuid",
      "publisherName": "Bloomsbury Publishing",
      "authors": [
        {
          "id": "uuid",
          "name": "J.K. Rowling",
          "bio": "British author...",
          "birthDate": "1965-07-31",
          "deathDate": null,
          "bookCount": 0
        }
      ]
    }
  ]
}
```

### Get Book by ID

**GET** `/api/book/{id}`

Retrieve a specific book by its ID.

**Path Parameters:**

- `id`: Book UUID

**Response:**

```json
{
  "success": true,
  "message": "Success",
  "data": {
    "id": "uuid",
    "title": "1984",
    "description": "A dystopian social science fiction novel...",
    "language": "English",
    "coverImageUrl": "https://example.com/1984.jpg",
    "publicationYear": 1949,
    "publisherId": "uuid",
    "publisherName": "Penguin Books",
    "authors": [
      {
        "id": "uuid",
        "name": "George Orwell",
        "bio": "English novelist and journalist...",
        "birthDate": "1903-06-25",
        "deathDate": "1950-01-21",
        "bookCount": 0
      }
    ]
  }
}
```

### Get Books by Author

**GET** `/api/book/by-author/{authorId}`

Retrieve all books by a specific author.

**Path Parameters:**

- `authorId`: Author UUID

**Response:**

```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "id": "uuid",
      "title": "1984",
      "description": "A dystopian social science fiction novel...",
      "language": "English",
      "coverImageUrl": "https://example.com/1984.jpg",
      "publicationYear": 1949,
      "publisherId": "uuid",
      "publisherName": "Penguin Books",
      "authors": [...]
    }
  ]
}
```

---

## 👨‍💼 Author Endpoints

### Get All Authors

**GET** `/api/author`

Retrieve all authors.

**Response:**

```json
{
  "success": true,
  "message": "Retrieved 5 authors",
  "data": [
    {
      "id": "uuid",
      "name": "George Orwell",
      "bio": "English novelist and journalist...",
      "birthDate": "1903-06-25",
      "deathDate": "1950-01-21",
      "bookCount": 0
    }
  ]
}
```

### Get Author by ID

**GET** `/api/author/{id}`

Retrieve a specific author by ID.

**Path Parameters:**

- `id`: Author UUID

**Response:**

```json
{
  "success": true,
  "message": "Author retrieved successfully",
  "data": {
    "id": "uuid",
    "name": "J.K. Rowling",
    "bio": "British author, best known for the Harry Potter fantasy series.",
    "birthDate": "1965-07-31",
    "deathDate": null,
    "bookCount": 0
  }
}
```

---

## 🏢 Publisher Endpoints

### Get All Publishers

**GET** `/api/publisher`

Retrieve all publishers.

**Response:**

```json
{
  "success": true,
  "message": "Retrieved 4 publishers",
  "data": [
    {
      "id": "uuid",
      "name": "Penguin Books",
      "description": "British publishing house founded in 1935.",
      "address": "London, UK",
      "foundedYear": 1935,
      "bookCount": 0
    }
  ]
}
```

### Get Publisher by ID

**GET** `/api/publisher/{id}`

Retrieve a specific publisher by ID.

**Path Parameters:**

- `id`: Publisher UUID

---

## 📚 User Book Endpoints (Protected)

All user book endpoints require authentication.

### Get My Books

**GET** `/api/userbook/my-books` 🔒

Retrieve the current user's book list.

**Response:**

```json
{
  "success": true,
  "message": "Retrieved 3 books from your library",
  "data": [
    {
      "id": "uuid",
      "bookId": "uuid",
      "bookTitle": "1984",
      "bookCoverUrl": "https://example.com/1984.jpg",
      "readingStatus": 2,
      "progress": 100,
      "startedAt": "2024-01-01T00:00:00Z",
      "completedAt": "2024-01-15T00:00:00Z",
      "addedAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

### Add Book to My List

**POST** `/api/userbook` 🔒

Add a book to the current user's reading list.

**Request Body:**

```json
{
  "bookId": "uuid",
  "readingStatus": 0,
  "progress": 0
}
```

**Reading Status Values:**

- `0`: ToRead
- `1`: InProgress
- `2`: Completed
- `3`: Dropped

**Response:**

```json
{
  "success": true,
  "message": "Book added to your library",
  "data": {
    "id": "uuid",
    "bookId": "uuid",
    "bookTitle": "Kafka on the Shore",
    "bookCoverUrl": "https://example.com/kafka.jpg",
    "readingStatus": 0,
    "progress": 0,
    "startedAt": null,
    "completedAt": null,
    "addedAt": "2024-01-07T10:00:00Z"
  }
}
```

### Update Book Status

**PUT** `/api/userbook/{bookId}` 🔒

Update reading status and progress for a book in the user's list.

**Path Parameters:**

- `bookId`: Book UUID

**Request Body:**

```json
{
  "readingStatus": 1,
  "progress": 45
}
```

### Remove Book from List

**DELETE** `/api/userbook/{bookId}` 🔒

Remove a book from the current user's reading list.

**Path Parameters:**

- `bookId`: Book UUID

**Response:**

```json
{
  "success": true,
  "message": "Book removed from your library",
  "data": null
}
```

---

## ⭐ Review Endpoints

### Get Reviews for Book

**GET** `/api/review/book/{bookId}`

Get all reviews for a specific book.

**Path Parameters:**

- `bookId`: Book UUID

**Response:**

```json
{
  "success": true,
  "message": "Retrieved 2 reviews for book",
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "username": "johndoe",
      "bookId": "uuid",
      "bookTitle": "1984",
      "rating": 5,
      "content": "Absolutely brilliant dystopian novel...",
      "createdAt": "2024-01-07T10:00:00Z"
    }
  ]
}
```

### Create Review

**POST** `/api/review` 🔒

Create a new book review.

**Request Body:**

```json
{
  "bookId": "uuid",
  "rating": 5,
  "content": "This book was absolutely amazing! The character development..."
}
```

**Response:**

```json
{
  "success": true,
  "message": "Review created successfully",
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "username": "johndoe",
    "bookId": "uuid",
    "bookTitle": "1984",
    "rating": 5,
    "content": "This book was absolutely amazing!...",
    "createdAt": "2024-01-07T10:00:00Z"
  }
}
```

### Update Review

**PUT** `/api/review/{id}` 🔒

Update an existing review (only your own reviews).

**Path Parameters:**

- `id`: Review UUID

**Request Body:**

```json
{
  "rating": 4,
  "content": "Updated review content..."
}
```

### Delete Review

**DELETE** `/api/review/{id}` 🔒

Delete a review (only your own reviews).

**Path Parameters:**

- `id`: Review UUID

---

## 📝 Blog Endpoints

### Get All Blogs

**GET** `/api/blog`

Get all blog posts or blogs by a specific user.

**Query Parameters:**

- `userId` (optional): Filter blogs by user ID

**Response:**

```json
{
  "success": true,
  "message": "Retrieved 5 blogs",
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "username": "johndoe",
      "title": "My Top 10 Books of 2024",
      "content": "Here are my favorite books I read this year...",
      "likeCount": 15,
      "isLikedByCurrentUser": false,
      "createdAt": "2024-01-07T10:00:00Z",
      "updatedAt": "2024-01-07T10:00:00Z"
    }
  ]
}
```

### Get Blog by ID

**GET** `/api/blog/{id}`

Get a specific blog post.

**Path Parameters:**

- `id`: Blog UUID

### Create Blog Post

**POST** `/api/blog` 🔒

Create a new blog post.

**Request Body:**

```json
{
  "title": "My Reading Journey in 2024",
  "content": "This year has been incredible for my reading goals..."
}
```

### Update Blog Post

**PUT** `/api/blog/{id}` 🔒

Update a blog post (only your own posts).

**Path Parameters:**

- `id`: Blog UUID

**Request Body:**

```json
{
  "title": "Updated Title",
  "content": "Updated content..."
}
```

### Delete Blog Post

**DELETE** `/api/blog/{id}` 🔒

Delete a blog post (only your own posts).

---

## 👍 Blog Like Endpoints (Protected)

### Like Blog

**POST** `/api/blog-like/{blogId}` 🔒

Like a blog post.

**Path Parameters:**

- `blogId`: Blog UUID

### Unlike Blog

**DELETE** `/api/blog-like/{blogId}` 🔒

Remove like from a blog post.

### Get Like Count

**GET** `/api/blog-like/{blogId}/count`

Get the number of likes for a blog post.

**Response:**

```json
{
  "success": true,
  "message": "Like count retrieved successfully",
  "data": 15
}
```

---

## 👤 User Profile Endpoints

### Get My Profile

**GET** `/api/user/profile` 🔒

Get the current user's profile.

**Response:**

```json
{
  "success": true,
  "message": "Profile retrieved successfully",
  "data": {
    "id": "uuid",
    "email": "user@example.com",
    "fullName": "John Doe",
    "username": "johndoe",
    "bio": "Avid reader and book blogger",
    "avatarUrl": "https://example.com/avatar.jpg",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-07T10:00:00Z"
  }
}
```

### Update Profile

**PUT** `/api/user/profile` 🔒

Update the current user's profile.

**Request Body:**

```json
{
  "fullName": "John Updated Doe",
  "username": "johnupdated",
  "bio": "Updated bio...",
  "avatarUrl": "https://example.com/new-avatar.jpg"
}
```

### Get Public Profile

**GET** `/api/profile/{username}`

Get a user's public profile by username.

**Path Parameters:**

- `username`: User's username

---

## 🏥 Health Check Endpoints

### Health Check

**GET** `/health`

Check if the API is healthy and running.

**Response:**

```
Healthy
```

---

## 📊 Error Responses

All endpoints return errors in a consistent format:

### Error Response Format

```json
{
  "success": false,
  "message": "Error description",
  "error": {
    "type": "ErrorType",
    "details": "Additional error details"
  }
}
```

### Common HTTP Status Codes

| Status Code | Meaning               | Description             |
| ----------- | --------------------- | ----------------------- |
| `200`       | OK                    | Request successful      |
| `400`       | Bad Request           | Invalid request data    |
| `401`       | Unauthorized          | Authentication required |
| `403`       | Forbidden             | Access denied           |
| `404`       | Not Found             | Resource not found      |
| `500`       | Internal Server Error | Server error            |

### Common Error Types

- `ValidationError`: Invalid request data
- `UnauthorizedAccess`: Authentication required
- `NotFound`: Resource not found
- `InvalidOperation`: Operation not allowed
- `Timeout`: Request timeout
- `InternalError`: Server error

---

## 🔍 Rate Limiting

Currently, there are no rate limits implemented, but consider implementing them for production use.

## 📝 Notes

- All timestamps are in UTC format
- All UUIDs are standard UUID v4 format
- File uploads are not currently supported
- The API supports JSON only (no XML or other formats)
- CORS is configured for common frontend development ports

## 🚀 SDK and Client Libraries

While official SDKs are not available yet, you can easily generate clients using:

- **OpenAPI/Swagger Codegen**: Use the `/swagger/v1/swagger.json` endpoint
- **Postman**: Import the API collection from Swagger
- **Insomnia**: Import the API specification

## 📚 Examples

Check out the `/docs/examples/` folder for complete request/response examples and sample client code in different languages.
