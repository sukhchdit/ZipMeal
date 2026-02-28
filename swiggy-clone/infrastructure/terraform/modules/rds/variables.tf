# =============================================================================
# RDS Module Variables
# =============================================================================

variable "name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID"
  type        = string
}

variable "database_subnet_ids" {
  description = "Database subnet IDs"
  type        = list(string)
}

variable "instance_class" {
  description = "RDS instance class"
  type        = string
}

variable "allocated_storage" {
  description = "Allocated storage in GB"
  type        = number
}

variable "multi_az" {
  description = "Enable Multi-AZ"
  type        = bool
}

variable "db_name" {
  description = "Database name"
  type        = string
}

variable "eks_security_group_id" {
  description = "EKS node security group ID for ingress"
  type        = string
}

variable "environment" {
  description = "Deployment environment"
  type        = string
}
