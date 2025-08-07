# 🚀 TaleTrail API Deployment Guide

This comprehensive guide covers deploying TaleTrail API to various cloud platforms using Docker containers.

## 🎯 Supported Deployment Platforms

- [**Render** (Recommended)](#render-recommended) - Easy Docker deployment with auto-scaling
- [**Railway**](#railway) - Modern platform with simple Git deployments
- [**Heroku**](#heroku) - Traditional PaaS with container support
- [**DigitalOcean App Platform**](#digitalocean-app-platform) - Managed container platform
- [**AWS ECS/Fargate**](#aws-ecsfargate) - Enterprise container orchestration
- [**Google Cloud Run**](#google-cloud-run) - Serverless container platform
- [**Fly.io**](#flyio) - Global edge deployment
- [**Vercel**](#vercel) - Serverless functions (alternative approach)

---

## 🏆 Render (Recommended)

Render offers excellent Docker support with Kubernetes orchestration benefits without complexity, including private networks, load balancing, scaling, and build caches. It also supports "Blueprints" (Infrastructure as Code) to define deployments in YAML files.

### Why Choose Render?

- ✅ **Free tier** with reasonable limits
- ✅ **Automatic HTTPS** and custom domains
- ✅ **Zero-config Docker** deployment
- ✅ **Git-based deployments** with auto-deploy
- ✅ **Built-in monitoring** and health checks
- ✅ **Environment variable** management
- ✅ **Persistent storage** options

### Prerequisites

- GitHub repository with your TaleTrail API code
- Supabase database setup and credentials
- Render account (free at [render.com](https://render.com))

### Step-by-Step Deployment

#### 1. **Prepare Your Repository**

```bash
# Ensure your code is pushed to GitHub
git add .
git commit -m "feat: prepare for Render deployment"
git push origin main
```

#### 2. **Create Web Service**

1. Go to [Render Dashboard](https://dashboard.render.com)
2. Click **"New"** → **"Web Service"**
3. Connect your **GitHub repository**
4. Select your **TaleTrail API repository**
5. Configure the service:

| Setting             | Value                        |
| ------------------- | ---------------------------- |
| **Name**            | `taletrail-api`              |
| **Environment**     | `Docker`                     |
| **Region**          | Choose closest to your users |
| **Branch**          | `main`                       |
| **Dockerfile Path** | `TaleTrail.API/Dockerfile`   |

#### 3. **Configure Environment Variables**

You can set environment variables in the Render Dashboard, and Render will automatically deploy your service to incorporate the new values.

Go to **Environment** tab and add:

```bash
# Required Supabase Configuration
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your_supabase_anon_key
SUPABASE_JWT_SECRET=your_supabase_jwt_secret

# Application Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:10000

# CORS Configuration (add your frontend domains)
ALLOWED_ORIGINS=https://your-frontend-domain.com,https://your-other-domain.com

# Optional: Logging
LOGGING__LOGLEVEL__DEFAULT=Information
```

#### 4. **Advanced Settings**

- **Auto-Deploy**: Enable for automatic deployments on git push
- **Health Check Path**: `/health`
- **Start Command**: Leave empty (uses Dockerfile ENTRYPOINT)

#### 5. **Deploy**

Click **"Create Web Service"** and Render will:

1. Clone your repository
2. Build the Docker image
3. Deploy the container
4. Run health checks
5. Provide you with a public URL

#### 6. **Verify Deployment**

```bash
# Test health endpoint
curl https://your-app-name.onrender.com/health

# Test API
curl https://your-app-name.onrender.com/api/book

# Access Swagger UI
open https://your-app-name.onrender.com/swagger
```

### Render Blueprint (Optional)

Create `render.yaml` in your repository root for Infrastructure as Code:

```yaml
services:
  - type: web
    name: taletrail-api
    env: docker
    dockerfilePath: ./TaleTrail.API/Dockerfile
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://+:10000
      - key: SUPABASE_URL
        sync: false # Set in dashboard for security
      - key: SUPABASE_KEY
        sync: false # Set in dashboard for security
      - key: SUPABASE_JWT_SECRET
        sync: false # Set in dashboard for security
    healthCheckPath: /health
    numInstances: 1
    plan: free # or starter, standard, pro
    region: oregon # or frankfurt, singapore, ohio
    buildCommand: ""
    startCommand: ""
```

Deploy with Blueprint:

```bash
# Add render.yaml to your repo
git add render.yaml
git commit -m "feat: add Render blueprint"
git push origin main

# Deploy via Render Dashboard using Blueprint
```

---

## 🚆 Railway

Railway offers excellent Docker support that works out of the box for Rails applications. However, it doesn't support docker-compose-like multi-service deployments, and every push to master triggers a new deployment.

### Why Choose Railway?

- ✅ **Simple Git integration**
- ✅ **Automatic Docker detection**
- ✅ **Built-in databases**
- ✅ **Great developer experience**
- ✅ **Reasonable pricing**

### Deployment Steps

#### 1. **Setup Railway**

```bash
# Install Railway CLI
npm install -g @railway/cli

# Login to Railway
railway login

# Initialize project
railway init
```

#### 2. **Deploy from GitHub**

1. Go to [Railway Dashboard](https://railway.app)
2. Click **"New Project"** → **"Deploy from GitHub repo"**
3. Select your **TaleTrail API repository**
4. Railway auto-detects Dockerfile

#### 3. **Configure Environment Variables**

In Railway dashboard, go to **Variables** and add:

```bash
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your_supabase_anon_key
SUPABASE_JWT_SECRET=your_supabase_jwt_secret
ASPNETCORE_ENVIRONMENT=Production
PORT=8080
ALLOWED_ORIGINS=https://your-frontend.vercel.app
```

#### 4. **Custom Domain (Optional)**

1. Go to **Settings** → **Domains**
2. Add your custom domain
3. Configure DNS records as shown

#### 5. **Deploy**

```bash
# Deploy via CLI
railway up

# Or push to GitHub (auto-deploy enabled by default)
git push origin main
```

---

## 🌊 Heroku

Despite changes to Heroku's pricing model, it remains a popular choice for many developers.

### Prerequisites

- Heroku account
- Heroku CLI installed
- Docker Desktop running

### Deployment Steps

#### 1. **Setup Heroku**

```bash
# Install Heroku CLI (macOS)
brew tap heroku/brew && brew install heroku

# Login
heroku login

# Create app
heroku create your-taletrail-api
```

#### 2. **Configure Container Stack**

```bash
# Set stack to container
heroku stack:set container -a your-taletrail-api

# Add environment variables
heroku config:set SUPABASE_URL=https://your-project.supabase.co -a your-taletrail-api
heroku config:set SUPABASE_KEY=your_supabase_anon_key -a your-taletrail-api
heroku config:set SUPABASE_JWT_SECRET=your_supabase_jwt_secret -a your-taletrail-api
heroku config:set ASPNETCORE_ENVIRONMENT=Production -a your-taletrail-api
heroku config:set ALLOWED_ORIGINS=https://your-frontend-domain.com -a your-taletrail-api
```

#### 3. **Create heroku.yml**

Create `heroku.yml` in your repository root:

```yaml
build:
  docker:
    web: TaleTrail.API/Dockerfile
run:
  web: dotnet TaleTrail.API.dll
```

#### 4. **Deploy**

```bash
# Add heroku remote
heroku git:remote -a your-taletrail-api

# Deploy
git push heroku main

# View logs
heroku logs --tail -a your-taletrail-api
```

#### 5. **Scale and Monitor**

```bash
# Scale up (if needed)
heroku ps:scale web=1 -a your-taletrail-api

# Open app
heroku open -a your-taletrail-api

# Monitor dyno usage
heroku ps -a your-taletrail-api
```

---

## 🌊 DigitalOcean App Platform

DigitalOcean offers robust features for application hosting and deployment as a Heroku alternative.

### Why Choose DigitalOcean?

- ✅ **Competitive pricing**
- ✅ **Global CDN included**
- ✅ **Integrated with DO ecosystem**
- ✅ **Good performance**

### Deployment Steps

#### 1. **Create App**

1. Go to [DigitalOcean Apps](https://cloud.digitalocean.com/apps)
2. Click **"Create App"**
3. Connect your **GitHub repository**
4. Select **TaleTrail API** repository

#### 2. **Configure App**

- **Source**: Your GitHub repository
- **Branch**: `main`
- **Autodeploy**: Enable
- **Build Command**: (Leave empty - uses Dockerfile)
- **Run Command**: (Leave empty - uses Dockerfile CMD)

#### 3. **Environment Variables**

Add in **App-Level Environment Variables**:

```bash
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your_supabase_anon_key
SUPABASE_JWT_SECRET=your_supabase_jwt_secret
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:8080
```

#### 4. **App Specs (Optional)**

Create `.do/app.yaml` for Infrastructure as Code:

```yaml
name: taletrail-api
services:
  - name: api
    source_dir: /
    github:
      repo: your-username/taletrail-api
      branch: main
      deploy_on_push: true
    dockerfile_path: TaleTrail.API/Dockerfile
    http_port: 8080
    instance_count: 1
    instance_size_slug: basic-xxs
    env:
      - key: ASPNETCORE_ENVIRONMENT
        value: "Production"
      - key: ASPNETCORE_URLS
        value: "http://0.0.0.0:8080"
    health_check:
      http_path: /health
    routes:
      - path: /
```

---

## ☁️ AWS ECS/Fargate

For enterprise deployments requiring AWS integration.

### Prerequisites

- AWS Account with appropriate permissions
- AWS CLI configured
- Docker image in Amazon ECR

### Deployment Overview

#### 1. **Push Image to ECR**

```bash
# Create ECR repository
aws ecr create-repository --repository-name taletrail-api --region us-east-1

# Get login token
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789012.dkr.ecr.us-east-1.amazonaws.com

# Build and tag image
docker build -t taletrail-api -f TaleTrail.API/Dockerfile .
docker tag taletrail-api:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/taletrail-api:latest

# Push image
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/taletrail-api:latest
```

#### 2. **Create Task Definition**

Create `ecs-task-definition.json`:

```json
{
  "family": "taletrail-api",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "executionRoleArn": "arn:aws:iam::123456789012:role/ecsTaskExecutionRole",
  "containerDefinitions": [
    {
      "name": "taletrail-api",
      "image": "123456789012.dkr.ecr.us-east-1.amazonaws.com/taletrail-api:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        },
        {
          "name": "ASPNETCORE_URLS",
          "value": "http://+:8080"
        }
      ],
      "secrets": [
        {
          "name": "SUPABASE_URL",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:taletrail/supabase-url"
        },
        {
          "name": "SUPABASE_KEY",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:taletrail/supabase-key"
        },
        {
          "name": "SUPABASE_JWT_SECRET",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:taletrail/jwt-secret"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/taletrail-api",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      },
      "healthCheck": {
        "command": [
          "CMD-SHELL",
          "curl -f http://localhost:8080/health || exit 1"
        ],
        "interval": 30,
        "timeout": 5,
        "retries": 3,
        "startPeriod": 60
      }
    }
  ]
}
```

#### 3. **Deploy with CLI**

```bash
# Register task definition
aws ecs register-task-definition --cli-input-json file://ecs-task-definition.json

# Create or update service
aws ecs create-service \
  --cluster your-cluster \
  --service-name taletrail-api \
  --task-definition taletrail-api:1 \
  --desired-count 1 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-12345],securityGroups=[sg-12345],assignPublicIp=ENABLED}"
```

---

## 🌐 Google Cloud Run

Serverless container platform with automatic scaling.

### Deployment Steps

#### 1. **Setup Google Cloud**

```bash
# Install gcloud CLI
curl https://sdk.cloud.google.com | bash

# Initialize and authenticate
gcloud init
gcloud auth login

# Enable required APIs
gcloud services enable run.googleapis.com containerregistry.googleapis.com
```

#### 2. **Build and Push to Container Registry**

```bash
# Configure Docker for GCR
gcloud auth configure-docker

# Build and tag image
docker build -t gcr.io/your-project-id/taletrail-api -f TaleTrail.API/Dockerfile .

# Push to GCR
docker push gcr.io/your-project-id/taletrail-api
```

#### 3. **Deploy to Cloud Run**

```bash
gcloud run deploy taletrail-api \
  --image gcr.io/your-project-id/taletrail-api \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --port 8080 \
  --set-env-vars "ASPNETCORE_ENVIRONMENT=Production,ASPNETCORE_URLS=http://+:8080" \
  --set-secrets "SUPABASE_URL=supabase-url:latest,SUPABASE_KEY=supabase-key:latest,SUPABASE_JWT_SECRET=jwt-secret:latest"
```

#### 4. **Configure Custom Domain**

```bash
# Map custom domain
gcloud run domain-mappings create \
  --service taletrail-api \
  --domain api.yourdomain.com \
  --region us-central1
```

---

## 🚁 Fly.io

Global edge deployment platform.

### Why Choose Fly.io?

- ✅ **Global edge locations**
- ✅ **Excellent performance**
- ✅ **Simple configuration**
- ✅ **WebSocket support**

### Deployment Steps

#### 1. **Install flyctl**

```bash
# macOS
brew install flyctl

# Login
fly auth login
```

#### 2. **Initialize Fly App**

```bash
# Initialize (creates fly.toml)
fly launch

# Follow prompts:
# - App name: taletrail-api
# - Region: choose closest to users
# - PostgreSQL: No (using Supabase)
# - Deploy now: No
```

#### 3. **Configure fly.toml**

```toml
app = "taletrail-api"
primary_region = "iad"

[build]
dockerfile = "TaleTrail.API/Dockerfile"

[env]
ASPNETCORE_ENVIRONMENT = "Production"
ASPNETCORE_URLS = "http://0.0.0.0:8080"

[[services]]
internal_port = 8080
protocol = "tcp"
auto_stop_machines = true
auto_start_machines = true
min_machines_running = 0

[[services.ports]]
handlers = ["http"]
port = 80
force_https = true

[[services.ports]]
handlers = ["tls", "http"]
port = 443

[services.concurrency]
type = "connections"
hard_limit = 25
soft_limit = 20

[[services.tcp_checks]]
interval = "15s"
timeout = "2s"
grace_period = "1s"
restart_limit = 0

[[services.http_checks]]
interval = "10s"
timeout = "2s"
grace_period = "1s"
restart_limit = 0
method = "get"
path = "/health"
```

#### 4. **Set Secrets**

```bash
fly secrets set SUPABASE_URL=https://your-project.supabase.co
fly secrets set SUPABASE_KEY=your_supabase_anon_key
fly secrets set SUPABASE_JWT_SECRET=your_supabase_jwt_secret
fly secrets set ALLOWED_ORIGINS=https://your-frontend-domain.com
```

#### 5. **Deploy**

```bash
# Deploy application
fly deploy

# Check status
fly status

# View logs
fly logs
```

---

## ⚡ Vercel (Serverless Alternative)

While Vercel is primarily for frontend apps, you can deploy .NET APIs using serverless functions.

### Prerequisites

- Vercel account
- Vercel CLI

### Setup Steps

#### 1. **Install Vercel CLI**

```bash
npm install -g vercel
vercel login
```

#### 2. **Create vercel.json**

```json
{
  "version": 2,
  "builds": [
    {
      "src": "TaleTrail.API/Program.cs",
      "use": "@vercel/dotnet"
    }
  ],
  "routes": [
    {
      "src": "/(.*)",
      "dest": "/TaleTrail.API"
    }
  ],
  "env": {
    "ASPNETCORE_ENVIRONMENT": "Production"
  }
}
```

#### 3. **Deploy**

```bash
vercel --prod
```

**Note**: Vercel has limitations for .NET APIs compared to container-based deployments.

---

## 📊 Platform Comparison

| Platform             | Free Tier    | Pricing      | Ease of Use | Performance | Features   |
| -------------------- | ------------ | ------------ | ----------- | ----------- | ---------- |
| **Render**           | ✅ Yes       | $7/month+    | ⭐⭐⭐⭐⭐  | ⭐⭐⭐⭐    | ⭐⭐⭐⭐⭐ |
| **Railway**          | ✅ $5 credit | $5/month+    | ⭐⭐⭐⭐⭐  | ⭐⭐⭐⭐    | ⭐⭐⭐⭐   |
| **Heroku**           | ❌ No        | $5/month+    | ⭐⭐⭐⭐    | ⭐⭐⭐      | ⭐⭐⭐⭐   |
| **DigitalOcean**     | ❌ No        | $5/month+    | ⭐⭐⭐⭐    | ⭐⭐⭐⭐    | ⭐⭐⭐⭐   |
| **AWS ECS**          | ✅ Limited   | Variable     | ⭐⭐        | ⭐⭐⭐⭐⭐  | ⭐⭐⭐⭐⭐ |
| **Google Cloud Run** | ✅ Yes       | Pay per use  | ⭐⭐⭐      | ⭐⭐⭐⭐⭐  | ⭐⭐⭐⭐   |
| **Fly.io**           | ✅ Limited   | $1.94/month+ | ⭐⭐⭐⭐    | ⭐⭐⭐⭐⭐  | ⭐⭐⭐⭐   |

## 🔧 Pre-Deployment Checklist

### ✅ Code Preparation

- [ ] All environment variables documented in `.env.example`
- [ ] Dockerfile optimized and tested locally
- [ ] Health check endpoint (`/health`) working
- [ ] CORS configured for your frontend domain(s)
- [ ] Database connection tested
- [ ] All secrets removed from code
- [ ] Logging configured appropriately

### ✅ Environment Setup

- [ ] Supabase database created and configured
- [ ] Environment variables ready
- [ ] Domain name purchased (if using custom domain)
- [ ] SSL certificate (automatically handled by most platforms)

### ✅ Testing

- [ ] API works locally with Docker
- [ ] All endpoints tested
- [ ] Authentication flow verified
- [ ] Database seeding completed
- [ ] Performance acceptable under load

## 🚨 Troubleshooting Common Issues

### Issue: "Application failed to start"

**Solutions:**

1. Check environment variables are set correctly
2. Verify Dockerfile builds locally
3. Check application logs for specific errors
4. Ensure port configuration matches platform requirements

### Issue: "Database connection failed"

**Solutions:**

1. Verify Supabase credentials
2. Check network connectivity from deployment platform
3. Ensure connection strings are correctly formatted
4. Test with diagnostic endpoints

### Issue: "JWT authentication not working"

**Solutions:**

1. Verify `SUPABASE_JWT_SECRET` is set correctly
2. Check token expiration settings
3. Ensure CORS is configured for your frontend
4. Test auth flow with API client

### Issue: "Slow cold starts"

**Solutions:**

1. Use platforms with better cold start performance (Railway, Fly.io)
2. Implement health check endpoints
3. Consider using always-on instances for production
4. Optimize Docker image size

### Issue: "Environment variables not loading"

**Solutions:**

1. Restart the application after setting variables
2. Check variable names match exactly (case-sensitive)
3. Verify quotes and special characters are handled correctly
4. Test with diagnostic endpoints

## 🌟 Production Best Practices

### 🔐 Security

- **Use secrets management** for sensitive data
- **Enable HTTPS** (automatic on most platforms)
- **Configure CORS** restrictively for production
- **Set up monitoring** and alerting
- **Regular security updates** for dependencies

### 📊 Performance

- **Use CDN** for static assets (if any)
- **Enable compression** in production
- **Monitor response times** and errors
- **Set up health checks** and auto-restart
- **Configure appropriate instance sizing**

### 🔍 Monitoring

- **Set up logging** aggregation (Render logs, CloudWatch, etc.)
- **Monitor application metrics** (response time, error rate)
- **Set up uptime monitoring** (UptimeRobot, Pingdom)
- **Create alerts** for critical issues
- **Regular backup verification** (Supabase handles this)

### 🚀 Scaling

- **Start small** and scale based on actual usage
- **Monitor resource usage** (CPU, memory, bandwidth)
- **Use auto-scaling** features when available
- **Consider load balancing** for high-traffic applications
- **Plan for database scaling** (Supabase handles this well)

---

## 🎉 Congratulations!

You now have comprehensive deployment options for your TaleTrail API! Choose the platform that best fits your needs:

- **New to deployment?** → Start with **Render** (easiest)
- **Want modern features?** → Try **Railway** or **Fly.io**
- **Need enterprise features?** → Consider **AWS ECS** or **Google Cloud Run**
- **On a budget?** → **Render** or **Railway** free tiers

Your TaleTrail API is now ready for the world! 🌍📚✨
