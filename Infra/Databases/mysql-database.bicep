param serverName string
param location string = resourceGroup().location
param adminUsername string = 'mysqladmin'
@secure()
param adminPassword string
param databaseName string

resource mysql 'Microsoft.DBforMySQL/flexibleServers@2024-12-01-preview' = {
  name: serverName
  location: location
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    administratorLogin: adminUsername
    administratorLoginPassword: adminPassword
    version: '5.7'
    storage: {
      storageSizeGB: 10
    }
    network: {
      publicNetworkAccess: 'Enabled'
    }
  }
}

resource db 'Microsoft.DBforMySQL/flexibleServers/databases@2024-12-01-preview' = {
  parent: mysql
  name: databaseName
}

output connectionString string = 'Server=${mysql.name}.mysql.database.azure.com;Database=${databaseName};User Id=${adminUsername}@${mysql.name};Password=${adminPassword};Ssl Mode=Required;'
