# =============================================================================
# Root Outputs
# =============================================================================

# --- VPC ---
output "vpc_id" {
  description = "VPC ID"
  value       = module.vpc.vpc_id
}

# --- EKS ---
output "eks_cluster_name" {
  description = "EKS cluster name"
  value       = module.eks.cluster_name
}

output "eks_cluster_endpoint" {
  description = "EKS cluster API endpoint"
  value       = module.eks.cluster_endpoint
}

output "eks_oidc_provider_arn" {
  description = "EKS OIDC provider ARN for IRSA"
  value       = module.eks.oidc_provider_arn
}

# --- RDS ---
output "rds_endpoint" {
  description = "RDS PostgreSQL endpoint"
  value       = module.rds.endpoint
}

# --- ElastiCache ---
output "elasticache_endpoint" {
  description = "ElastiCache Redis endpoint"
  value       = module.elasticache.endpoint
}

# --- MSK ---
output "msk_bootstrap_brokers_tls" {
  description = "MSK TLS bootstrap brokers"
  value       = module.msk.bootstrap_brokers_tls
}

# --- OpenSearch ---
output "opensearch_endpoint" {
  description = "OpenSearch domain endpoint"
  value       = module.opensearch.endpoint
}

# --- S3 ---
output "media_bucket_name" {
  description = "S3 media bucket name"
  value       = module.s3.bucket_name
}

output "cdn_domain_name" {
  description = "CloudFront distribution domain name"
  value       = module.s3.cdn_domain_name
}

# --- ECR ---
output "ecr_repository_url" {
  description = "ECR repository URL"
  value       = module.ecr.repository_url
}

# --- Monitoring ---
output "prometheus_endpoint" {
  description = "Amazon Managed Prometheus workspace endpoint"
  value       = module.monitoring.prometheus_endpoint
}

output "sns_alarm_topic_arn" {
  description = "SNS topic ARN for alarms"
  value       = module.monitoring.sns_topic_arn
}
