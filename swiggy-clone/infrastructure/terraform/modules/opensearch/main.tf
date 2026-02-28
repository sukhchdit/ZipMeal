# =============================================================================
# OpenSearch Module — Search engine (ES 8.x compatible)
# =============================================================================

data "aws_caller_identity" "current" {}

resource "aws_security_group" "opensearch" {
  name_prefix = "${var.name_prefix}-opensearch-"
  vpc_id      = var.vpc_id
  description = "OpenSearch security group"

  ingress {
    description     = "HTTPS from EKS nodes"
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = [var.eks_security_group_id]
  }

  tags = {
    Name = "${var.name_prefix}-opensearch-sg"
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_cloudwatch_log_group" "opensearch" {
  name              = "/opensearch/${var.name_prefix}"
  retention_in_days = 14
}

resource "aws_opensearch_domain" "main" {
  domain_name    = "${var.name_prefix}-search"
  engine_version = "OpenSearch_2.11"

  cluster_config {
    instance_type          = var.instance_type
    instance_count         = var.instance_count
    zone_awareness_enabled = var.instance_count > 1

    dynamic "zone_awareness_config" {
      for_each = var.instance_count > 1 ? [1] : []
      content {
        availability_zone_count = min(var.instance_count, 3)
      }
    }
  }

  ebs_options {
    ebs_enabled = true
    volume_size = var.ebs_volume_size
    volume_type = "gp3"
  }

  vpc_options {
    subnet_ids         = slice(var.private_subnet_ids, 0, var.instance_count > 1 ? min(var.instance_count, 3) : 1)
    security_group_ids = [aws_security_group.opensearch.id]
  }

  encrypt_at_rest {
    enabled = true
  }

  node_to_node_encryption {
    enabled = true
  }

  domain_endpoint_options {
    enforce_https       = true
    tls_security_policy = "Policy-Min-TLS-1-2-PFS-2023-10"
  }

  advanced_security_options {
    enabled                        = true
    internal_user_database_enabled = true

    master_user_options {
      master_user_name     = "admin"
      master_user_password = "ChangeMe123!" # Managed via AWS Secrets Manager in practice
    }
  }

  log_publishing_options {
    cloudwatch_log_group_arn = aws_cloudwatch_log_group.opensearch.arn
    log_type                 = "INDEX_SLOW_LOGS"
  }

  access_policies = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect    = "Allow"
      Principal = { AWS = "*" }
      Action    = "es:*"
      Resource  = "arn:aws:es:*:${data.aws_caller_identity.current.account_id}:domain/${var.name_prefix}-search/*"
    }]
  })

  tags = {
    Name = "${var.name_prefix}-opensearch"
  }
}
