# MiniFab Project - Namespace Structure

## Standardized Namespaces

This document outlines the standardized namespace structure for the MiniFab project:

### API Project Namespaces

- **Root**: `MiniFab.Api`
- **Controllers**: `MiniFab.Api.Controllers`
- **Models**: `MiniFab.Api.Models`
- **Data**: `MiniFab.Api.Data`
- **Services**: 
  - `MiniFab.Api.Services.Device`
  - `MiniFab.Api.Services.SensorData`
  - `MiniFab.Api.Services.MessageBroker`
- **Hubs**: `MiniFab.Api.Hubs`
- **Migrations**: `MiniFab.Api.Migrations`

### Producer Project Namespaces

- **Root**: `MiniFab.Producer`

### Client Dashboard

- The Vue.js client-dashboard doesn't use C# namespaces

## Changes Made

1. **Migration Files**: Updated from `api.Migrations` to `MiniFab.Api.Migrations`
2. **Producer Project**: Added proper namespace `MiniFab.Producer`
3. **Consistency**: All projects now follow consistent PascalCase naming convention
4. **Structure**: Follows .NET naming conventions with project prefix

## Benefits

- **Consistency**: All namespaces follow the same pattern
- **Clarity**: Clear project and feature separation
- **Best Practices**: Follows .NET naming conventions
- **Maintainability**: Easier to navigate and understand the codebase
