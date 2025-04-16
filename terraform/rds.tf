resource "aws_db_subnet_group" "db-subnet" {
  name = "main-rds-subnet-groupv2"
  subnet_ids = [aws_subnet.subnet1.id, aws_subnet.subnet2.id]
}

resource "aws_db_instance" "galaxyworld_db_instance" {
  allocated_storage = 20
  identifier = var.db_name
  db_name = var.db_name
  instance_class = "db.t3.micro"
  engine = "postgres"
  username = var.db_username
  password = var.db_password
  skip_final_snapshot = true
  publicly_accessible = true
  multi_az= false
  db_subnet_group_name = aws_db_subnet_group.db-subnet.name
  vpc_security_group_ids = [aws_security_group.rds-sec-grp.id]
}
