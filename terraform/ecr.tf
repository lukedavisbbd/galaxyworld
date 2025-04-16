resource "aws_ecr_repository" "gw_ecr" {
  name = var.ecr_repo_name
  image_tag_mutability = "MUTABLE"
}

resource "aws_ecs_cluster" "ecs_cluster" {
  name = var.ecs_cluster_name
}

resource "aws_cloudwatch_log_group" "ecs_logs" {
  name              = "/ecs/ecs-logs"
  retention_in_days = 14
}


output "repository_url" {
  value = aws_ecr_repository.gw_ecr.repository_url
  sensitive = true
}

output "db_endpoint" {
  description = "RDS instance endpoint"
  value       = aws_db_instance.galaxyworld_db_instance.endpoint
}
