# Railway Deployment Guide

This guide will help you deploy the OpenAPI Guard application to Railway with PostgreSQL database.

## Prerequisites

1. Railway account (sign up at https://railway.app)
2. GitHub repository connected to Railway
3. Git CLI installed

## Deployment Steps

### 1. Create a New Railway Project

1. Go to [Railway Dashboard](https://railway.app/dashboard)
2. Click "New Project"
3. Select "Deploy from GitHub repo"
4. Connect your GitHub account and select the `openapi-guard` repository
5. Click "Deploy"

### 2. Add PostgreSQL Service

Railroad will detect the Dockerfile and create a service. Now you need to add a PostgreSQL database:

1. In your Railway project, click the **+ New** button
2. Select **PostgreSQL**
3. Railway will automatically provision a PostgreSQL instance
4. It will create a `DATABASE_URL` environment variable automatically

### 3. Configure Environment Variables

The application automatically converts Railway's `DATABASE_URL` to the proper PostgreSQL connection string.

**Optional environment variables you can set:**

- `ASPNETCORE_ENVIRONMENT`: Set to `Production` (default: `Production`)
- `ASPNETCORE_URLS`: Already set to `http://+:8080` in Dockerfile

### 4. Deploy

1. Push your changes to GitHub:
   ```bash
   git add .
   git commit -m "Add Railway deployment configuration"
   git push origin main
   ```

2. Railway will automatically detect the push and start the deployment
3. Monitor the deployment in the Railway dashboard
4. Once deployment completes, click on the OpenAPI Guard service to view the deployment URL

### 5. Verify Deployment

1. Click the deployment URL to access your application
2. The application will automatically run database migrations on startup
3. You should see the OpenAPI Guard application running

## Service Configuration

### Port Configuration
- The application runs on port **8080** inside the container
- Railway automatically exposes this on a public domain
- Environment variable: `ASPNETCORE_URLS=http://+:8080`

### Database Configuration
- PostgreSQL runs on port 5432 (internal)
- Database URL is provided via `DATABASE_URL` environment variable
- Connection string is automatically converted in `Program.cs`
- Migrations run automatically on application startup

## Troubleshooting

### Check Logs
- View service logs in Railway dashboard → your service → Logs tab
- Look for migration errors or connection issues

### Database Connection Issues
- Verify `DATABASE_URL` is set in environment variables
- Check that PostgreSQL service status is "Running"
- Ensure both services are connected in Railway

### Application Won't Start
- Check `ASPNETCORE_ENVIRONMENT` is set correctly
- Verify migrations are not failing in logs
- Ensure `ASPNETCORE_URLS=http://+:8080` is set

## Scaling & Additional Configuration

### Scale Replicas
- In service settings, adjust "Replicas" for horizontal scaling
- Recommended: 1 for development, 2+ for production

### Custom Domain
1. In Service Settings → Domain
2. Add a custom domain
3. Follow Railway's DNS setup instructions

### HTTPS
- Railway provides automatic HTTPS certificates
- Custom domains get free SSL automatically

## File Structure

The following files were added/modified for Railway deployment:

- `Dockerfile` - Multi-stage Docker build for ASP.NET application
- `.dockerignore` - Docker build optimization
- `railway.json` - Railway-specific configuration
- `src/OpenApiGuard.App/Program.cs` - Updated to read `DATABASE_URL` environment variable
- `src/OpenApiGuard.App/appsettings.json` - Default connection string configuration

## Notes

- Database migrations run automatically on application startup
- The application uses ASP.NET Identity for authentication
- PostgreSQL Alpine image is used for smaller footprint
- Application serves both API and Blazor UI components
