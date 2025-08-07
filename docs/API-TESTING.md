# 🧪 TaleTrail API Testing Guide

This guide provides step-by-step instructions for testing all API endpoints using Postman, curl, or any REST client.

## 🔑 Authentication Flow

### 1. Sign Up
```bash
curl -X POST http://localhost:5198/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "TestPassword123",
    "fullName": "Test User",
    "username": "testuser"
  }'
```

### 2. Login (Get JWT Token)
```bash
curl -X POST http://localhost:5198/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "TestPassword123"
  }'
```

**Save the `accessToken` from the response for authenticated requests!**

## 📚 Book Endpoints (Public)

### Get All Books
```bash
curl -X GET http://localhost:5198/api/book
```

### Search Books
```bash
curl -X GET "http://localhost:5198/api/book?search=1984"
```

### Get Book by ID
```bash
curl -X GET http://localhost:5198/api/book/{book-id}
```

### Get Books by Author
```bash
curl -X GET http://localhost:5198/api/book/by-author/{author-id}
```

## 👥 Author & Publisher Endpoints

### Get All Authors
```bash
curl -X GET http://localhost:5198/api/author
```

### Get All Publishers
```bash
curl -X GET http://localhost:5198/api/publisher
```

## 🔒 User Book Endpoints (Authenticated)

**Note:** Replace `{JWT_TOKEN}` with your actual access token from login response.

### Get My Books
```bash
curl -X GET http://localhost:5198/api/userbook/my-books \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

### Add Book to My List
```bash
curl -X POST http://localhost:5198/api/userbook \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "bookId": "{book-id}",
    "readingStatus": 0,
    "progress": 0
  }'
```

**Reading Status Values:**
- `0` = ToRead
- `1` = InProgress  
- `2` = Completed
- `3` = Dropped

### Update Book Status
```bash
curl -X PUT http://localhost:5198/api/userbook/{book-id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "readingStatus": 1,
    "progress": 45
  }'
```

### Remove Book from List
```bash
curl -X DELETE http://localhost:5198/api/userbook/{book-id} \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

### Get Reading Statistics
```bash
curl -X GET http://localhost:5198/api/userbook/stats \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

### Get Books by Status
```bash
curl -X GET http://localhost:5198/api/userbook/by-status/inprogress \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

## ⭐ Review Endpoints

### Get Reviews for a Book
```bash
curl -X GET http://localhost:5198/api/review/book/{book-id}
```

### Create Review (Authenticated)
```bash
curl -X POST http://localhost:5198/api/review \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "bookId": "{book-id}",
    "rating": 5,
    "content": "Amazing book! Highly recommended."
  }'
```

### Update Review (Authenticated)
```bash
curl -X PUT http://localhost:5198/api/review/{review-id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "rating": 4,
    "content": "Updated review content"
  }'
```

### Delete Review (Authenticated)
```bash
curl -X DELETE http://localhost:5198/api/review/{review-id} \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

## 📝 Blog Endpoints

### Get All Blogs
```bash
curl -X GET http://localhost:5198/api/blog
```

### Get Blogs by User
```bash
curl -X GET "http://localhost:5198/api/blog?userId={user-id}"
```

### Get Blog by ID
```bash
curl -X GET http://localhost:5198/api/blog/{blog-id}
```

### Create Blog Post (Authenticated)
```bash
curl -X POST http://localhost:5198/api/blog \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "title": "My Reading Journey",
    "content": "This year I have read some amazing books..."
  }'
```

### Update Blog Post (Authenticated)
```bash
curl -X PUT http://localhost:5198/api/blog/{blog-id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "title": "Updated Title",
    "content": "Updated content"
  }'
```

### Delete Blog Post (Authenticated)
```bash
curl -X DELETE http://localhost:5198/api/blog/{blog-id} \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

## 👍 Blog Like Endpoints (Authenticated)

### Like a Blog
```bash
curl -X POST http://localhost:5198/api/blog-like/{blog-id} \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

### Unlike a Blog
```bash
curl -X DELETE http://localhost:5198/api/blog-like/{blog-id} \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

### Get Like Count
```bash
curl -X GET http://localhost:5198/api/blog-like/{blog-id}/count
```

## 👤 User Profile Endpoints

### Get My Profile (Authenticated)
```bash
curl -X GET http://localhost:5198/api/user/profile \
  -H "Authorization: Bearer {JWT_TOKEN}"
```

### Update My Profile (Authenticated)
```bash
curl -X PUT http://localhost:5198/api/user/profile \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "fullName": "Updated Name",
    "username": "updatedusername",
    "bio": "Book lover and avid reader",
    "avatarUrl": "https://example.com/avatar.jpg"
  }'
```

### Get Public Profile by Username
```bash
curl -X GET http://localhost:5198/api/profile/{username}
```

## 🏥 Health & Diagnostic Endpoints

### Health Check
```bash
curl -X GET http://localhost:5198/health
```

### Test Database Connection
```bash
curl -X GET http://localhost:5198/api/diagnostic/connection
```

### Test Database Tables
```bash
curl -X GET http://localhost:5198/api/diagnostic/tables
```

### Check Environment Variables
```bash
curl -X GET http://localhost:5198/api/diagnostic/environment
```

## 📝 Postman Collection

### Environment Variables
Create a Postman environment with these variables:
```json
{
  "base_url": "http://localhost:5198",
  "jwt_token": "your_jwt_token_here",
  "user_id": "your_user_id_here",
  "book_id": "a_valid_book_id",
  "author_id": "a_valid_author_id",
  "review_id": "a_valid_review_id",
  "blog_id": "a_valid_blog_id"
}
```

### Pre-request Script for Authentication
Add this to requests that need authentication:
```javascript
// Automatically add JWT token to requests
if (pm.environment.get("jwt_token")) {
    pm.request.headers.add({
        key: 'Authorization',
        value: 'Bearer ' + pm.environment.get("jwt_token")
    });
}
```

## 🧪 Testing Workflow

### 1. Initial Setup
1. Start the API (`dotnet run`)
2. Sign up a new user
3. Login to get JWT token
4. Save the token for authenticated requests

### 2. Test Book Management
1. Get all books (should show seeded data)
2. Pick a book ID for testing
3. Add book to your list
4. Update book status to "InProgress"
5. Update progress to 50%
6. Mark book as "Completed"
7. Get your reading stats

### 3. Test Reviews
1. Create a review for a book
2. Update the review
3. Get all reviews for that book
4. Delete the review

### 4. Test Blog Posts
1. Create a blog post
2. Get all blogs
3. Like the blog post
4. Update the blog post
5. Delete the blog post

### 5. Test User Profile
1. Get your profile
2. Update your profile information
3. Get your public profile by username

## 📊 Expected Response Format

All API responses follow this format:

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {
    // Response data here
  }
}
```

### Error Response
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

## 🚨 Common Issues & Solutions

### Issue: 401 Unauthorized
**Solution:** Ensure you're including the JWT token in the Authorization header

### Issue: 404 Not Found (for books/authors)
**Solution:** Use the actual IDs from the seeded data. Check `/api/book` and `/api/author` first.

### Issue: 400 Bad Request (UserBook endpoints)
**Solution:** 
1. Ensure the book exists
2. Check that readingStatus values are 0-3
3. Verify progress is 0-100

### Issue: Database connection errors
**Solution:** Check your `.env` file has correct Supabase credentials

### Issue: "Book already in your list"
**Solution:** Check your current books first with `/api/userbook/my-books`

## 📈 Performance Testing

### Load Testing with curl
```bash
# Test concurrent requests
for i in {1..10}; do
  curl -X GET http://localhost:5198/api/book &
done
wait
```

### Response Time Testing
```bash
# Measure response time
time curl -X GET http://localhost:5198/api/book
```

## 🔍 Debugging Tips

1. **Enable detailed logging** in `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "TaleTrail.API": "Debug"
    }
  }
}
```

2. **Check application logs** in the `logs/` directory

3. **Use Swagger UI** at `http://localhost:5198/swagger` for interactive testing

4. **Monitor database** through Supabase dashboard

5. **Check authentication** by decoding JWT tokens at [jwt.io](https://jwt.io)

Happy testing! 🚀