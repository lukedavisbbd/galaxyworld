resource "aws_ecr_repository" "default" {
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
  value = aws_ecr_repository.default.repository_url
  sensitive = true
}
