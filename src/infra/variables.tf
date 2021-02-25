variable "resource_name" {
  description = "The name of the resources"
  type        = string
}

variable "location" {
  description = "The Azure region in which all resources should be created"
  type        = string
}

variable "sql_user" {
  description = "The administrative user name for the SQL Server"
  type        = string
}

variable "sql_password" {
  description = "The administrative user password for the SQL Server"
  type        = string
}