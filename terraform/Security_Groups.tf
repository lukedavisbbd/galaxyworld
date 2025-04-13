# Security Group for RDS
resource "aws_security_group" "rds-sec-grp" {
  name        = "rds-sec-group"
  description = "Allow traffic"
  vpc_id      = aws_vpc.main.id

  ingress {
    description      = "VPC"
    from_port        = 5432
    to_port          = 5432
    protocol         = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

