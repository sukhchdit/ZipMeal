# =============================================================================
# MSK Module Outputs
# =============================================================================

output "bootstrap_brokers_tls" {
  description = "MSK TLS bootstrap brokers connection string"
  value       = aws_msk_cluster.main.bootstrap_brokers_tls
}

output "zookeeper_connect_string" {
  description = "ZooKeeper connection string"
  value       = aws_msk_cluster.main.zookeeper_connect_string
}

output "cluster_arn" {
  description = "MSK cluster ARN"
  value       = aws_msk_cluster.main.arn
}

output "security_group_id" {
  description = "MSK security group ID"
  value       = aws_security_group.msk.id
}
