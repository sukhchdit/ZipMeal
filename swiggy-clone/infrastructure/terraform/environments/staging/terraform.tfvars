# =============================================================================
# Staging Environment — Medium sizing for realistic testing
# =============================================================================

environment = "staging"
aws_region  = "ap-south-1"

# VPC
vpc_cidr           = "10.1.0.0/16"
availability_zones = ["ap-south-1a", "ap-south-1b", "ap-south-1c"]

# EKS
eks_cluster_version     = "1.29"
eks_node_instance_types = ["t3.medium"]
eks_node_desired_size   = 2
eks_node_min_size       = 2
eks_node_max_size       = 5

# RDS
rds_instance_class    = "db.t3.small"
rds_allocated_storage = 50
rds_multi_az          = false

# ElastiCache
elasticache_node_type       = "cache.t3.small"
elasticache_num_cache_nodes = 2

# MSK
msk_instance_type          = "kafka.t3.small"
msk_number_of_broker_nodes = 3
msk_ebs_volume_size        = 50

# OpenSearch
opensearch_instance_type   = "t3.medium.search"
opensearch_instance_count  = 2
opensearch_ebs_volume_size = 30
