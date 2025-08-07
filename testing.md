
# ✅ TaleTrail API - Working & Fixed Endpoints

## ✅ Working Endpoints

- `POST /api/auth/signup` - yes
- `POST /api/auth/login` - yes
- `GET /api/user/profile` - No
- `PUT /api/user/profile` - No
- `GET /api/profile/{username}` - yes

## 🔄 Now Fixed Endpoints

### 📚 Book Endpoints
- `GET /api/book` (with optional `?search=`)
- `GET /api/book/{id}`
- `GET /api/book/by-author/{authorId}`

### ✍️ Author Endpoints
- `GET /api/author`
- `GET /api/author/{id}`

### 🏢 Publisher Endpoints
- `GET /api/publisher`
- `GET /api/publisher/{id}`

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
