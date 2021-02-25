provider "azurerm" {
  features {}
}

terraform {
  backend "azurerm" {
    resource_group_name  = "terraform-state"
    storage_account_name = "devopsgithubterraform1"
    container_name       = "terraform-state"
    key                  = "ghxax.terraform.tfstate"
  }
}

/*
* Resource Group
*/
resource "azurerm_resource_group" "this" {
  name     = var.resource_name
  location = var.location
}

/*
* App Service Plan
*/
resource "azurerm_app_service_plan" "this" {
  name                = var.resource_name
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  kind                = "Windows"
  sku {
    tier = "Basic"
    size = "B1"
  }
}

/*
* Backend App Service
*/
resource "azurerm_app_service" "backend" {
  name                = "${var.resource_name}-api"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  app_service_plan_id = azurerm_app_service_plan.this.id
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"      = azurerm_application_insights.this.instrumentation_key
    "APPINSIGHTS_PORTALINFO"              = "ASP.NET"
    "APPINSIGHTS_PROFILERFEATURE_VERSION" = "1.0.0"
    "WEBSITE_HTTPLOGGING_RETENTION_DAYS"  = "35"
  }
  connection_string {
    name  = "TaskContext"
    type  = "SQLServer"
    value = "Server=tcp:${var.resource_name}.database.windows.net,1433;Initial Catalog=${var.resource_name};Persist Security Info=False;User ID=${var.sql_user};Password=${var.sql_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}

/*
* Frontend App Service
*/
resource "azurerm_app_service" "frontend" {
  name                = "${var.resource_name}-app"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  app_service_plan_id = azurerm_app_service_plan.this.id
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"      = azurerm_application_insights.this.instrumentation_key
    "APPINSIGHTS_PORTALINFO"              = "ASP.NET"
    "APPINSIGHTS_PROFILERFEATURE_VERSION" = "1.0.0"
    "WEBSITE_HTTPLOGGING_RETENTION_DAYS"  = "35"
  }
  connection_string {
    name  = "Backend"
    type  = "Custom"
    value = "https://${var.resource_name}-api.azurewebsites.net"
  }
}

/*
* Application Insights
*/
resource "azurerm_application_insights" "this" {
  name                = var.resource_name
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  application_type    = "web"
}

/*
* SQL Server
*/
resource "azurerm_sql_server" "this" {
  name                         = var.resource_name
  resource_group_name          = azurerm_resource_group.this.name
  location                     = azurerm_resource_group.this.location
  version                      = "12.0"
  administrator_login          = var.sql_user
  administrator_login_password = var.sql_password
}

/*
* SQL Database
*/
resource "azurerm_sql_database" "this" {
  name                = var.resource_name
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  server_name         = azurerm_sql_server.this.name
  create_mode         = "Default"
  edition             = "Basic"
}

/*
* SQL Database Firewall allow Azure resources
*/
resource "azurerm_sql_firewall_rule" "this" {
  name                = "Azure Resources"
  resource_group_name = azurerm_resource_group.this.name
  server_name         = azurerm_sql_server.this.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}