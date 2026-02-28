# =============================================================================
# MSK Module Variables
# =============================================================================

variable "name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID"
  type        = string
}

variable "private_subnet_ids" {
  description = "Private subnet IDs"
  type        = list(string)
}

variable "instance_type" {
  description = "MSK broker instance type"
  type        = string
}

variable "number_of_broker_nodes" {
  description = "Number of broker nodes"
  type        = number
}

variable "ebs_volume_size" {
  description = "EBS volume size in GB"
  type        = number
}

variable "eks_security_group_id" {
  description = "EKS node security group ID for ingress"
  type        = string
}

variable "environment" {
  description = "Deployment environment"
  type        = string
}
