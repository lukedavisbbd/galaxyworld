# Data source to get the current AWS account ID
data "aws_caller_identity" "current" {}









# ECS Task Definition
resource "aws_ecs_task_definition" "ecs_task_definition" {
  family                   = "fargate-task"
  execution_role_arn       = aws_iam_role.ecs_task_exec_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  task_role_arn            = aws_iam_role.ecs_task_role.arn

  container_definitions = jsonencode([
    {
      name  = "galaxyworld-api"
      image = "${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.region}.amazonaws.com/${var.ecr_repo_name}:latest"
      portMappings = [{
        containerPort = 8080
        hostPort      = 8080
        protocol      = "tcp"
    }]
    environment = [
      {
        name  = "DB_URL"
        value = "jdbc:postgresql://${aws_db_instance.galaxyworld_db_instance.endpoint}/${aws_db_instance.galaxyworld_db_instance.db_name}"
      },
      {
        name  = "DB_USERNAME"
        value = var.rds_db_name
      },
      {
        name  = "DB_PASSWORD"
        value = var.rds_db_password
      },

    ]
    secrets: [
    ]
    logConfiguration = {
      logDriver = "awslogs"
      options = {
        awslogs-group         = aws_cloudwatch_log_group.ecs_logs.name
        awslogs-create-group  = "true"
        awslogs-region        = var.region
        awslogs-stream-prefix = "ecs"
      }
    }
  }])
}

# ECS Service
resource "aws_ecs_service" "ecs_service" {
  name            = "galaxyworld-api-service"
  cluster         = aws_ecs_cluster.ecs_cluster.id
  task_definition = aws_ecs_task_definition.ecs_task_definition.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets         = [aws_subnet.subnet1.id]
    security_groups = [aws_security_group.fargate_sg.id]
    assign_public_ip = true
  }

  force_new_deployment = true
}
