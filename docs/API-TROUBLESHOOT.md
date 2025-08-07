# 🔧 UserBook Endpoints Troubleshooting Guide

This guide helps you fix common issues with the User Book endpoints that weren't working according to your testing document.

## 🎯 Quick Fix Checklist

### 1. Database Schema Issues

**Problem:** Enum type mismatch between C# and PostgreSQL
**Solution:** Run this SQL in Supabase:

```sql
-- Ensure enum is properly defined
DROP TYPE IF EXISTS reading_status CASCADE;
CREATE TYPE reading_status AS ENUM ('ToRead', 'InProgress', 'Completed', 'Dropped');

-- Update table to use enum
ALTER TABLE user_books 
ALTER COLUMN reading_status TYPE reading_status 
USING reading_status::reading_status;

-- Verify the change
SELECT column_name, data_type, udt_name 
FROM information_schema.columns 
WHERE table_name = 'user_books' AND column_name = 'reading_status';
```

### 2. Service Registration Issues

**Problem:** Services not properly registered in DI container
**Solution:** Verify in `Program.cs`:

```csharp
// Ensure UserBookDao is registered
builder.Services.AddScoped<UserBookDao>();

// Ensure UserBookService is registered
builder.Services.AddScoped<UserBookService>();

// Add logging for UserBookDao
builder.Services.AddScoped<UserBookDao>();
```

### 3. Authentication Issues

**Problem:** JWT token not being passed or validated correctly
**Solution:** Test authentication first:

```bash
# 1. Login and get token
curl -X POST http://localhost:5198/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "TestPassword123"}'

# 2. Save the accessToken and test profile endpoint
curl -X GET http://localhost:5198/api/user/profile \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 🔍 Debugging Steps

### Step 1: Test Basic Functionality

1. **Check if API is running:**
```bash
curl http://localhost:5198/health
```

2. **Test authentication:**
```bash
curl -X GET http://localhost:5198/api/user/profile \
  -H "Authorization: Bearer YOUR_TOKEN"
```

3. **Check if books exist:**
```bash
curl http://localhost:5198/api/book
```

### Step 2: Test UserBook Endpoints One by One

1. **Get empty library (should work):**
```bash
curl -X GET http://localhost:5198/api/userbook/my-books \
  -H "Authorization: Bearer YOUR_TOKEN"
```

2. **Add a book (most likely to fail):**
```bash
curl -X POST http://localhost:5198/api/userbook \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "bookId": "VALID_BOOK_ID_FROM_STEP_3",
    "readingStatus": 0,
    "progress": 0
  }'
```

### Step 3: Check Logs

Look for errors in:
- Console output
- `logs/taletrail-*.txt` files
- Supabase logs (if available)

## 🐛 Common Error Scenarios

### Error: "Failed to create user book"

**Possible Causes:**
1. Invalid book ID
2. Book already in user's list
3. Database connection issues
4. Enum conversion problems

**Debug Steps:**
```bash
# 1. Verify book exists
curl http://localhost:5198/api/book/YOUR_BOOK_ID

# 2. Check if already in list
curl -X GET http://localhost:5198/api/userbook/my-books \
  -H "Authorization: Bearer YOUR_TOKEN"

# 3. Test database connection
curl http://localhost:5198/api/diagnostic/connection
```

### Error: "Book not found in your list"

**Possible Causes:**
1. Using wrong book ID
2. Book was never added
3. User ID mismatch

**Debug Steps:**
```bash
# 1. List all books in library
curl -X GET http://localhost:5198/api/userbook/my-books \
  -H "Authorization: Bearer YOUR_TOKEN"

# 2. Verify user identity
curl -X GET http://localhost:5198/api/user/profile \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Error: "Invalid reading status"

**Possible Causes:**
1. Wrong enum values
2. Database enum mismatch

**Solutions:**
- Use numeric values: 0 (ToRead), 1 (InProgress), 2 (Completed), 3 (Dropped)
- Verify database enum matches C# enum

## 🔧 Manual Database Fixes

### Check Current Data
```sql
-- See all user books
SELECT ub.*, u.username, b.title 
FROM user_books ub 
JOIN users u ON ub.user_id = u.id 
JOIN books b ON ub.book_id = b.id;

-- Check enum values
SELECT DISTINCT reading_status FROM user_books;
```

### Clean Up Test Data
```sql
-- Remove test user books (optional)
DELETE FROM user_books WHERE user_id = 'YOUR_USER_ID';

-- Or remove all user books for fresh start
TRUNCATE user_books;
```

### Manually Insert Test Data
```sql
-- Get a valid user ID and book ID first
SELECT id, username FROM users LIMIT 1;
SELECT id, title FROM books LIMIT 1;

-- Insert test user book
INSERT INTO user_books (id, user_id, book_id, reading_status, progress, created_at, updated_at)
VALUES (
  gen_random_uuid(),
  'YOUR_USER_ID',
  'YOUR_BOOK_ID',
  'ToRead',
  0,
  NOW(),
  NOW()
);
```

## 🚀 Performance Optimization

### Add Database Indexes
```sql
-- Improve query performance
CREATE INDEX IF NOT EXISTS idx_user_books_user_id ON user_books(user_id);
CREATE INDEX IF NOT EXISTS idx_user_books_book_id ON user_books(book_id);
CREATE INDEX IF NOT EXISTS idx_user_books_status ON user_books(reading_status);
CREATE INDEX IF NOT EXISTS idx_user_books_user_book ON user_books(user_id, book_id);
```

## 🧪 Test Data Setup

### Create Test User and Books
```sql
-- Create test user (if not exists)
INSERT INTO users (id, email, username, full_name, created_at, updated_at)
VALUES (
  gen_random_uuid(),
  'testuser@example.com',
  'testuser',
  'Test User',
  NOW(),
  NOW()
) ON CONFLICT (email) DO NOTHING;

-- Add test books to user library
WITH test_user AS (
  SELECT id FROM users WHERE email = 'testuser@example.com'
),
sample_books AS (
  SELECT id FROM books LIMIT 3
)
INSERT INTO user_books (id, user_id, book_id, reading_status, progress, created_at, updated_at)
SELECT 
  gen_random_uuid(),
  test_user.id,
  sample_books.id,
  CASE 
    WHEN ROW_NUMBER() OVER () = 1 THEN 'ToRead'
    WHEN ROW_NUMBER() OVER () = 2 THEN 'InProgress'  
    ELSE 'Completed'
  END,
  CASE 
    WHEN ROW_NUMBER() OVER () = 1 THEN 0
    WHEN ROW_NUMBER() OVER () = 2 THEN 50
    ELSE 100
  END,
  NOW(),
  NOW()
FROM test_user, sample_books;
```

## 📊 Validation Script

Create this test script to validate all endpoints:

```bash
#!/bin/bash
# userbook_test.sh

BASE_URL="http://localhost:5198"
EMAIL="test@example.com"
PASSWORD="TestPassword123"

echo "🔑 Step 1: Login..."
LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}")

TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.data.accessToken')

if [ "$TOKEN" == "null" ]; then
  echo "❌ Login failed. Check credentials."
  exit 1
fi

echo "✅ Login successful"

echo "📚 Step 2: Get books..."
BOOKS_RESPONSE=$(curl -s -X GET "$BASE_URL/api/book")
BOOK_ID=$(echo $BOOKS_RESPONSE | jq -r '.data[0].id')

if [ "$BOOK_ID" == "null" ]; then
  echo "❌ No books found. Check data seeding."
  exit 1
fi

echo "✅ Found book: $BOOK_ID"

echo "📖 Step 3: Get my books..."
MY_BOOKS=$(curl -s -X GET "$BASE_URL/api/userbook/my-books" \
  -H "Authorization: Bearer $TOKEN")

echo $MY_BOOKS | jq '.success'

echo "➕ Step 4: Add book to library..."
ADD_BOOK=$(curl -s -X POST "$BASE_URL/api/userbook" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"bookId\":\"$BOOK_ID\",\"readingStatus\":0,\"progress\":0}")

echo $ADD_BOOK | jq '.success'

echo "🔄 Step 5: Update book status..."
UPDATE_BOOK=$(curl -s -X PUT "$BASE_URL/api/userbook/$BOOK_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"readingStatus\":1,\"progress\":25}")

echo $UPDATE_BOOK | jq '.success'

echo "📊 Step 6: Get reading stats..."
STATS=$(curl -s -X GET "$BASE_URL/api/userbook/stats" \
  -H "Authorization: Bearer $TOKEN")

echo $STATS | jq '.success'

echo "🗑️ Step 7: Remove book..."
DELETE_BOOK=$(curl -s -X DELETE "$BASE_URL/api/userbook/$BOOK_ID" \
  -H "Authorization: Bearer $TOKEN")

echo $DELETE_BOOK | jq '.success'

echo "✅ All tests completed!"
```

Make it executable and run:
```bash
chmod +x userbook_test.sh
./userbook_test.sh
```

## 🎯 Next Steps After Fixing

1. **Update your testing.md** to mark UserBook endpoints as working
2. **Add integration tests** to prevent regressions
3. **Consider adding** batch operations (add/remove multiple books)
4. **Implement** reading goals and progress tracking features
5. **Add** book recommendations based on reading history

## 🆘 Still Not Working?

If you're still having issues:

1. **Check the complete error message** in logs
2. **Verify Supabase connection** with diagnostic endpoints
3. **Test with a fresh database** (backup first!)
4. **Enable debug logging** for detailed information
5. **Share the specific error message** for targeted help

The improved code I provided should resolve the main issues with enum handling and error management. Let me know if you need help with any specific error messages!