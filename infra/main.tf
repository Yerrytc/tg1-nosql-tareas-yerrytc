terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "rg-tg1-yerrytc"
    storage_account_name = "sttg1yerrytc01"
    container_name       = "tfstate"
    key                  = "tg1.tfstate"
  }
}

provider "azurerm" {
  features {}
  resource_provider_registrations = "none"
}

data "azurerm_resource_group" "rg" {
  name = "rg-tg1-yerrytc"
}

resource "azurerm_cosmosdb_account" "db" {
  name                = "cosmos-tg1-yerrytc"
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"
  free_tier_enabled   = true

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = data.azurerm_resource_group.rg.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "tareas" {
  name                = "tareasdb"
  resource_group_name = data.azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.db.name
  throughput          = 400
}

resource "azurerm_cosmosdb_sql_container" "tareas" {
  name                = "tareas"
  resource_group_name = data.azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.db.name
  database_name       = azurerm_cosmosdb_sql_database.tareas.name
  partition_key_paths = ["/id"]
}

resource "azurerm_service_plan" "plan" {
  name                = "plan-tg1-yerrytc"
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_linux_web_app" "app" {
  name                = "app-tg1-nosql-yerrytc"
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.plan.id
  https_only          = true

  site_config {
    always_on = false

    application_stack {
      dotnet_version = "10.0"
    }
  }

  app_settings = {
    Cosmos__Endpoint  = azurerm_cosmosdb_account.db.endpoint
    Cosmos__Key       = azurerm_cosmosdb_account.db.primary_key
    Cosmos__Database  = azurerm_cosmosdb_sql_database.tareas.name
    Cosmos__Container = azurerm_cosmosdb_sql_container.tareas.name
  }
}

output "app_url" {
  value = "https://${azurerm_linux_web_app.app.default_hostname}"
}

output "cosmos_endpoint" {
  value = azurerm_cosmosdb_account.db.endpoint
}
