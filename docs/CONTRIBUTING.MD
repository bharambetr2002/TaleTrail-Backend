# 🤝 Contributing to TaleTrail API

Thank you for your interest in contributing to TaleTrail API! This document provides guidelines for contributing to the project.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Making Changes](#making-changes)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Testing](#testing)
- [Documentation](#documentation)

## 📜 Code of Conduct

By participating in this project, you agree to abide by our code of conduct:

- **Be respectful**: Treat everyone with respect and kindness
- **Be inclusive**: Welcome newcomers and help them learn
- **Be collaborative**: Work together towards common goals
- **Be constructive**: Provide helpful feedback and suggestions

## 🚀 Getting Started

### Prerequisites

Before you start contributing, make sure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- [Docker](https://www.docker.com/) (optional)
- A [Supabase](https://supabase.com) account for testing

### Areas for Contribution

We welcome contributions in these areas:

- 🐛 **Bug fixes**
- ✨ **New features**
- 📚 **Documentation improvements**
- 🧪 **Tests**
- 🎨 **Code quality improvements**
- 🔧 **DevOps and tooling**

## 💻 Development Setup

1. **Fork the repository**

   ```bash
   # Fork the repo on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/taletrail-api.git
   cd taletrail-api
   ```

2. **Add the original repository as upstream**

   ```bash
   git remote add upstream https://github.com/ORIGINAL_OWNER/taletrail-api.git
   ```

3. **Set up your development environment**

   ```bash
   # Copy environment file
   cp .env.example .env
   # Edit .env with your Supabase credentials

   # Restore dependencies
   cd TaleTrail.API
   dotnet restore

   # Run the application
   dotnet run
   ```

4. **Verify your setup**
   ```bash
   # Test health endpoint
   curl http://localhost:5198/health
   ```

## 🔄 Making Changes

### Branch Naming Convention

Create a new branch for your changes:

```bash
git checkout -b feature/your-feature-name
git checkout -b bugfix/issue-number-description
git checkout -b docs/documentation-update
```

Examples:

- `feature/user-following-system`
- `bugfix/123-fix-user-login-issue`
- `docs/update-api-documentation`

### Commit Message Format

Use clear and descriptive commit messages:

```
type(scope): short description

Longer description if necessary

Fixes #issue_number
```

**Types:**

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**

```
feat(auth): add password reset functionality

fix(userbook): resolve enum conversion issue
Fixes #123

docs(api): update authentication endpoints documentation

refactor(services): simplify user service methods
```

### Making Your Changes

1. **Keep changes focused**: One feature or fix per pull request
2. **Follow existing patterns**: Maintain consistency with existing code
3. **Add tests**: Include tests for new functionality
4. **Update documentation**: Update relevant documentation
5. **Test thoroughly**: Ensure all tests pass

## 🔀 Pull Request Process

1. **Update your fork**

   ```bash
   git fetch upstream
   git checkout main
   git merge upstream/main
   ```

2. **Create a feature branch**

   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes and commit**

   ```bash
   git add .
   git commit -m "feat: add your feature description"
   ```

4. **Push to your fork**

   ```bash
   git push origin feature/your-feature-name
   ```

5. **Create a Pull Request**
   - Go to the original repository on GitHub
   - Click "New Pull Request"
   - Select your branch
   - Fill out the PR template

### Pull Request Template

When creating a PR, please include:

```markdown
## Description

Brief description of the changes

## Type of Change

- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Refactoring
- [ ] Performance improvement

## Testing

- [ ] Tests added/updated
- [ ] All tests passing
- [ ] Manual testing completed

## Screenshots (if applicable)

## Related Issues

Fixes #issue_number

## Checklist

- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
```

## 📏 Coding Standards

### C# Coding Style

Follow the established patterns in the codebase:

```csharp
// Use PascalCase for public members
public class BookService
{
    private readonly BookDao _bookDao; // private fields with underscore prefix

    // Use async/await patterns
    public async Task<BookResponseDTO> GetBookAsync(Guid id)
    {
        // Use descriptive variable names
        var book = await _bookDao.GetByIdAsync(id);

        // Handle null cases
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        return MapToDto(book);
    }
}
```

### File Organization

```
TaleTrail.API/
├── Controllers/         # API controllers
├── Services/           # Business logic
├── DAO/               # Data access objects
├── Model/             # Entities and DTOs
│   ├── DTOs/          # Data transfer objects
│   ├── Entities/      # Database entities
│   └── Validations/   # Validation models
├── Middleware/        # Custom middleware
├── Extensions/        # Extension methods
└── Helpers/          # Utility classes
```

### Database Conventions

- Use PascalCase for entity properties
- Use snake_case for database column names
- Always use GUIDs for primary keys
- Include `CreatedAt` and `UpdatedAt` timestamps

### Error Handling

```csharp
// Use specific exceptions
throw new KeyNotFoundException("Resource not found");
throw new UnauthorizedAccessException("Access denied");
throw new InvalidOperationException("Operation not allowed");

// Use try-catch in controllers
try
{
    var result = await _service.DoSomethingAsync();
    return Ok(ApiResponse<T>.SuccessResponse("Success", result));
}
catch (Exception ex)
{
    return BadRequest(ApiResponse<T>.ErrorResponse(ex.Message));
}
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test TaleTrail.Tests/
```

### Writing Tests

Create tests for:

- New services and business logic
- API controllers
- Data access objects
- Utility functions

Example test structure:

```csharp
[Fact]
public async Task GetBookAsync_WithValidId_ReturnsBook()
{
    // Arrange
    var bookId = Guid.NewGuid();
    var expectedBook = new Book { Id = bookId, Title = "Test Book" };

    // Act
    var result = await _bookService.GetBookAsync(bookId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedBook.Title, result.Title);
}
```

## 📚 Documentation

### Types of Documentation

1. **Code Comments**: For complex logic

   ```csharp
   /// <summary>
   /// Converts reading status enum to database string representation
   /// </summary>
   /// <param name="status">The reading status enum</param>
   /// <returns>String representation for database storage</returns>
   private static string ConvertEnumToString(ReadingStatus status)
   ```

2. **API Documentation**: Update `docs/API.md` for new endpoints

3. **README Updates**: Update main README for significant changes

4. **Setup Documentation**: Update `docs/SETUP.md` for environment changes

### Documentation Guidelines

- Keep documentation up-to-date with code changes
- Use clear, concise language
- Include examples where helpful
- Update API documentation for new endpoints
- Add inline comments for complex business logic

## 🚀 Release Process

### Versioning

We follow [Semantic Versioning](https://semver.org/):

- `MAJOR.MINOR.PATCH`
- Breaking changes increment MAJOR
- New features increment MINOR
- Bug fixes increment PATCH

### Release Checklist

- [ ] All tests passing
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] Version numbers updated
- [ ] Docker image builds successfully
- [ ] Deployment tested

## ❓ Getting Help

### Where to Ask Questions

- **GitHub Issues**: For bugs and feature requests
- **GitHub Discussions**: For general questions and ideas
- **Discord/Slack**: For real-time chat (if available)

### Reporting Bugs

When reporting bugs, please include:

1. **Environment details** (OS, .NET version, etc.)
2. **Steps to reproduce** the issue
3. **Expected behavior**
4. **Actual behavior**
5. **Error messages** (if any)
6. **Screenshots** (if applicable)

### Suggesting Features

When suggesting new features:

1. **Check existing issues** to avoid duplicates
2. **Describe the problem** you're trying to solve
3. **Propose a solution** (if you have one)
4. **Consider the impact** on existing functionality
5. **Be open to discussion** and feedback

## 🎯 Good First Issues

New contributors should look for issues labeled:

- `good-first-issue`
- `beginner-friendly`
- `documentation`
- `help-wanted`

These issues are typically:

- Well-defined and scoped
- Have clear acceptance criteria
- Don't require deep knowledge of the codebase
- Have maintainer guidance available

## 🏆 Recognition

Contributors will be recognized in:

- Project README
- Release notes
- Hall of Fame (if established)

Thank you for contributing to TaleTrail API! 🚀
