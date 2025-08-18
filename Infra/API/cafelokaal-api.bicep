@description('Name of the API App Service')
param apiName string = 'cafelokaal-api'

@description('Location for all resources')
param location string = resourceGroup().location

// ────────────── App Service Plan ──────────────
resource plan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${apiName}-plan'
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    capacity: 1
  }
  properties: {
    reserved: false // Windows plan
  }
}

// ────────────── API App Service ──────────────
resource api 'Microsoft.Web/sites@2022-03-01' = {
  name: apiName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: plan.id
    siteConfig: {
      netFrameworkVersion: 'v7.0'
    }
  }
}
