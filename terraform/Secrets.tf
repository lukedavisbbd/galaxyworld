# Generate a random password
resource "random_password" "password" {
  length           = 16
  special          = true
  override_special = "!#$&*()-=+[]{}<>:?"  # all tf special characters without '/', '@', '"', ' ' (rds requirement)
}

# Create a Secrets Manager secret
resource "aws_secretsmanager_secret" "galaxyworld-secret" {
  name = "galaxyworld-rds-db-secretss"
}

# Create a new version of the secret with the generated password
resource "aws_secretsmanager_secret_version" "s-version" {
  secret_id     = aws_secretsmanager_secret.galaxyworld-secret.id
  secret_string = <<EOF
    {
      "username": "myuser",
      "password": "${random_password.password.result}"
    }
  EOF
}
