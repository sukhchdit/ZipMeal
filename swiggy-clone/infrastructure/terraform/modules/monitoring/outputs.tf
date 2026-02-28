# =============================================================================
# Monitoring Module Outputs
# =============================================================================

output "prometheus_endpoint" {
  description = "Amazon Managed Prometheus workspace endpoint"
  value       = aws_prometheus_workspace.main.prometheus_endpoint
}

output "prometheus_workspace_id" {
  description = "Prometheus workspace ID"
  value       = aws_prometheus_workspace.main.id
}

output "sns_topic_arn" {
  description = "SNS alarm topic ARN"
  value       = aws_sns_topic.alarms.arn
}
