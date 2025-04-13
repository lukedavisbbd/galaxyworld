terraform {

  backend "s3" {
    bucket = "galaxyworldbucket"
    encrypt = true
    region = "eu-west-1"
    key = "gw_ecr/main.tfstate"
  }

  required_providers {
    aws = {
      source = "hashicorp/aws"
      version = "5.86.1"
    }
    }
}

provider "aws" {
  region = "eu-west-1"
}