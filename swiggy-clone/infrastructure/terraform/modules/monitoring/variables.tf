# =============================================================================
# Monitoring Module Variables
# =============================================================================

variable "name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "eks_cluster_name" {
  description = "EKS cluster name for CloudWatch dimensions"
  type        = string
}

variable "alarm_email" {
  description = "Email for alarm notifications"
  type        = string
  default     = ""
}

variable "environment" {
  description = "Deployment environment"
  type        = string
}
