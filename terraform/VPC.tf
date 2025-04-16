# VPC
resource "aws_vpc" "main" {
  cidr_block           = "10.0.0.0/16"
  enable_dns_support   = true
  enable_dns_hostnames = true
}

# Public Subnet
resource "aws_subnet" "subnet1" {
  vpc_id                = aws_vpc.main.id
  cidr_block            = "10.0.1.0/24"
  availability_zone     = "eu-west-1a"
  map_public_ip_on_launch = true

}

resource "aws_subnet" "subnet2" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.2.0/24"
  availability_zone = "eu-west-1b"
  map_public_ip_on_launch = true
}

# Internet Gateway
resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.main.id
}

# Route Tables
resource "aws_route_table" "public-route-table" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.igw.id
  }
}

# Route Table Associations
resource "aws_route_table_association" "public-assoc" {
 subnet_id      = aws_subnet.subnet1.id
 route_table_id = aws_route_table.public-route-table.id
 
}

resource "aws_route_table_association" "public-assoc1" {
  subnet_id      = aws_subnet.subnet2.id
  route_table_id = aws_route_table.public-route-table.id
  
}
