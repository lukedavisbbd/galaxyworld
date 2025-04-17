variable "db_name" {
  description = "The name of the RDS instance and the database"
  type        = string
  default     = "galaxyworld"
}

variable "db_username" {
  description = "The username for the RDS instance"
  type        = string
}

variable "db_password" {
  description = "The password for the RDS instance"
  type        = string
  sensitive   = true
}

variable "key_name"{
  description = "Key name"
  type = string
}
