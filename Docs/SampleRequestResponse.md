# 📋 TaleTrail API - Sample Requests & Responses

## Authentication

### 1. User Signup

**Endpoint:** `POST /api/auth/signup`

**Request:**

```json
{
  "email": "john.doe@example.com",
  "password": "securePassword123",
  "fullName": "John Doe"
}
```

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "email": "john.doe@example.com",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "v1.MRjVnEtJQAFhvI9...",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "fullName": "John Doe"
  }
}
```

**Response (Error - 400):**

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "Email": ["Invalid email format"],
    "Password": ["Password must be at least 6 characters"]
  }
}
```

### 2. User Login

**Endpoint:** `POST /api/auth/login`

**Request:**

```json
{
  "email": "john.doe@example.com",
  "password": "securePassword123"
}
```

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "email": "john.doe@example.com",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "v1.MRjVnEtJQAFhvI9...",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "fullName": "John Doe"
  }
}
```

## Book Management

### 3. Get All Books

**Endpoint:** `GET /api/book`

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "The Great Gatsby",
      "description": "A classic American novel set in the Jazz Age",
      "coverUrl": "https://example.com/covers/gatsby.jpg",
      "language": "English",
      "publicationYear": 1925,
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "createdAt": "2024-01-15T10:30:00Z"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "title": "To Kill a Mockingbird",
      "description": "A gripping tale of racial injustice and childhood innocence",
      "coverUrl": "https://example.com/covers/mockingbird.jpg",
      "language": "English",
      "publicationYear": 1960,
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "createdAt": "2024-01-16T14:20:00Z"
    }
  ]
}
```

### 4. Create Book

**Endpoint:** `POST /api/book`

**Request:**

```json
{
  "title": "1984",
  "description": "A dystopian social science fiction novel and cautionary tale",
  "coverUrl": "https://example.com/covers/1984.jpg",
  "language": "English",
  "publicationYear": 1949
}
```

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Book created successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "title": "1984",
    "description": "A dystopian social science fiction novel and cautionary tale",
    "coverUrl": "https://example.com/covers/1984.jpg",
    "language": "English",
    "publicationYear": 1949,
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "createdAt": "2024-01-17T09:15:00Z"
  }
}
```

### 5. Get Book by ID

**Endpoint:** `GET /api/book/{id}`

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Success",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "The Great Gatsby",
    "description": "A classic American novel set in the Jazz Age",
    "coverUrl": "https://example.com/covers/gatsby.jpg",
    "language": "English",
    "publicationYear": 1925,
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

**Response (Error - 404):**

```json
{
  "success": false,
  "message": "Book with ID 550e8400-e29b-41d4-a716-446655440099 not found"
}
```

## Blog Management

### 6. Get All Blogs

**Endpoint:** `GET /api/blog`

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "title": "My Reading Journey with Classic Literature",
      "content": "I've been exploring classic literature lately and wanted to share my thoughts...",
      "createdAt": "2024-01-18T16:45:00Z"
    },
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "userId": "456e7890-e89b-12d3-a456-426614174001",
      "title": "Top 10 Science Fiction Books of 2024",
      "content": "Science fiction continues to evolve and surprise us. Here are my top picks...",
      "createdAt": "2024-01-19T11:20:00Z"
    }
  ]
}
```

### 7. Create Blog

**Endpoint:** `POST /api/blog`

**Request:**

```json
{
  "title": "Why Fantasy Books Matter in Modern Times",
  "content": "Fantasy literature offers us an escape from reality while simultaneously helping us understand it better. In this post, I'll explore how fantasy books have shaped my worldview and why I believe they're more relevant than ever in our current digital age..."
}
```

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Blog created successfully",
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440002",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "title": "Why Fantasy Books Matter in Modern Times",
    "content": "Fantasy literature offers us an escape from reality while simultaneously helping us understand it better...",
    "createdAt": "2024-01-20T14:30:00Z"
  }
}
```

## Review Management

### 8. Get Book Reviews

**Endpoint:** `GET /api/review/book/{bookId}`

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "id": "770e8400-e29b-41d4-a716-446655440000",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "bookId": "550e8400-e29b-41d4-a716-446655440000",
      "rating": 5,
      "comment": "An absolute masterpiece! Fitzgerald's prose is unmatched.",
      "createdAt": "2024-01-16T12:00:00Z"
    },
    {
      "id": "770e8400-e29b-41d4-a716-446655440001",
      "userId": "456e7890-e89b-12d3-a456-426614174001",
      "bookId": "550e8400-e29b-41d4-a716-446655440000",
      "rating": 4,
      "comment": "Great book, though the ending felt a bit rushed.",
      "createdAt": "2024-01-17T08:45:00Z"
    }
  ]
}
```

### 9. Create Review

**Endpoint:** `POST /api/review`

**Request:**

```json
{
  "bookId": "550e8400-e29b-41d4-a716-446655440002",
  "rating": 4,
  "comment": "Orwell's vision is haunting and more relevant today than ever. A must-read for understanding surveillance culture."
}
```

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Review created successfully",
  "data": {
    "id": "770e8400-e29b-41d4-a716-446655440002",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "bookId": "550e8400-e29b-41d4-a716-446655440002",
    "rating": 4,
    "comment": "Orwell's vision is haunting and more relevant today than ever. A must-read for understanding surveillance culture.",
    "createdAt": "2024-01-20T15:20:00Z"
  }
}
```

## Watchlist Management

### 10. Get User Watchlist

**Endpoint:** `GET /api/watchlist/user/{userId}`

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "id": "880e8400-e29b-41d4-a716-446655440000",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "bookId": "550e8400-e29b-41d4-a716-446655440000",
      "status": "completed",
      "addedAt": "2024-01-10T10:00:00Z"
    },
    {
      "id": "880e8400-e29b-41d4-a716-446655440001",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "bookId": "550e8400-e29b-41d4-a716-446655440001",
      "status": "reading",
      "addedAt": "2024-01-15T14:30:00Z"
    },
    {
      "id": "880e8400-e29b-41d4-a716-446655440002",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "bookId": "550e8400-e29b-41d4-a716-446655440002",
      "status": "to_read",
      "addedAt": "2024-01-18T09:20:00Z"
    }
  ]
}
```

### 11. Add to Watchlist

**Endpoint:** `POST /api/watchlist`

**Request:**

```json
{
  "bookId": "550e8400-e29b-41d4-a716-446655440003",
  "status": "to_read"
}
```

**Response (Success - 200):**

```json
{
  "success": true,
  "message": "Book added to watchlist successfully",
  "data": {
    "id": "880e8400-e29b-41d4-a716-446655440003",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "bookId": "550e8400-e29b-41d4-a716-446655440003",
    "status": "to_read",
    "addedAt": "2024-01-20T16:00:00Z"
  }
}
```

## Error Responses

### 400 Bad Request (Validation Error)

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "Title": ["Title is required"],
    "Rating": ["Rating must be between 1 and 5"]
  }
}
```

### 401 Unauthorized

```json
{
  "success": false,
  "message": "Invalid or expired token"
}
```

### 404 Not Found

```json
{
  "success": false,
  "message": "Resource not found"
}
```

### 500 Internal Server Error

```json
{
  "success": false,
  "message": "An internal server error occurred"
}
```

## Headers

### Authentication

For protected endpoints, include the JWT token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Content-Type

For POST/PUT requests with JSON body:

```
Content-Type: application/json
```

## Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `400 Bad Request` - Validation error or malformed request
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Access denied
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Notes

1. All timestamps are in ISO 8601 format (UTC)
2. GUIDs are used for all ID fields
3. Pagination will be added in future versions
4. Rate limiting may apply to certain endpoints
5. Some endpoints require authentication (JWT token)
6. File uploads for book covers will be supported in future versions
