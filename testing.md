
# ✅ TaleTrail API - Working & Fixed Endpoints

## ✅ Working Endpoints

- `POST /api/auth/signup` - 
- `POST /api/auth/login` - 
- `GET /api/user/profile` - No
- `PUT /api/user/profile` - No
- `GET /api/profile/{username}` - 

## 🔄 Now Fixed Endpoints

### 📚 Book Endpoints
- `GET /api/book` (with optional `?search=`) - Yes
- `GET /api/book/{id}` - Yes
- `GET /api/book/by-author/{authorId}` - yes (update(book_author table))

### ✍️ Author Endpoints
- `GET /api/author` - yes
- `GET /api/author/{id}` - yes

### 🏢 Publisher Endpoints
- `GET /api/publisher` - yes
- `GET /api/publisher/{id}` - yes

### 📝 Blog Endpoints
- `GET /api/blog` (with optional `?userId=`)
- `GET /api/blog/{id}`
- `POST /api/blog` 🔒
- `PUT /api/blog/{id}` 🔒
- `DELETE /api/blog/{id}` 🔒
- `POST /api/blog-like/{blogId}` 🔒
- `DELETE /api/blog-like/{blogId}` 🔒
- `GET /api/blog-like/{blogId}/count`

### 🌟 Review Endpoints
- `GET /api/review/book/{bookId}`
- `POST /api/review` 🔒
- `PUT /api/review/{id}` 🔒
- `DELETE /api/review/{id}` 🔒

### 📚 User Book Endpoints
- `GET /api/userbook/my-books` 🔒
- `POST /api/userbook` 🔒
- `PUT /api/userbook/{bookId}` 🔒
- `DELETE /api/userbook/{bookId}` 🔒
