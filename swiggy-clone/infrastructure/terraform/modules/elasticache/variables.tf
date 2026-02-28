# =============================================================================
# ElastiCache Module Variables
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

variable "node_type" {
  description = "ElastiCache node type"
  type        = string
}

variable "num_cache_nodes" {
  description = "Number of cache nodes"
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
