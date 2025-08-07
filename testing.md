
# ✅ TaleTrail API - Working & Fixed Endpoints

## ✅ Working Endpoints

- `POST /api/auth/signup` - Yes
- `POST /api/auth/login` - Yes
- `GET /api/user/profile` - Yes
- `PUT /api/user/profile` - Yes
- `GET /api/profile/{username}` - Yes

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
- `GET /api/blog` (with optional `?userId=`) - yes
- `GET /api/blog/{id}` - yes
- `POST /api/blog` 🔒 - yes
- `PUT /api/blog/{id}` 🔒 - yes
- `DELETE /api/blog/{id}` 🔒 - yes
- `POST /api/blog-like/{blogId}` 🔒 - yes
- `DELETE /api/blog-like/{blogId}` 🔒 - yes
- `GET /api/blog-like/{blogId}/count` - yes

### 🌟 Review Endpoints
- `GET /api/review/book/{bookId}` - yes
- `POST /api/review` 🔒 - yes
- `PUT /api/review/{id}` 🔒 - yes
- `DELETE /api/review/{id}` 🔒 - yes

### 📚 User Book Endpoints - No
- `GET /api/userbook/my-books` 🔒
- `POST /api/userbook` 🔒
- `PUT /api/userbook/{bookId}` 🔒
- `DELETE /api/userbook/{bookId}` 🔒
