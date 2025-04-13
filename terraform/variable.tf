variable "region" {
  type = string
  default = "eu-west-1"
}

variable "rds_db_name" {
  type = string
  default = "galaxyworld"
}

variable "rds_db_password" {
  type = string
  sensitive = true
}

variable "database_name" {
    type = string
    default = "gwdatabase"
}

variable "ecs_cluster_name" {
  type = string
  default = "gw_ecs_cluster"
}

variable "ecr_repo_name" {
  type = string
  default = "galaxy_ecr"
}