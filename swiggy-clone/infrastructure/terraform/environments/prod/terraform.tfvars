# =============================================================================
# Production Environment — Full sizing with HA
# =============================================================================

environment = "prod"
aws_region  = "ap-south-1"

# VPC
vpc_cidr           = "10.2.0.0/16"
availability_zones = ["ap-south-1a", "ap-south-1b", "ap-south-1c"]

# EKS
eks_cluster_version     = "1.29"
eks_node_instance_types = ["m5.large"]
eks_node_desired_size   = 3
eks_node_min_size       = 3
eks_node_max_size       = 10

# RDS
rds_instance_class    = "db.r6g.large"
rds_allocated_storage = 100
rds_multi_az          = true

# ElastiCache
elasticache_node_type       = "cache.r6g.large"
elasticache_num_cache_nodes = 3

# MSK
msk_instance_type          = "kafka.m5.large"
msk_number_of_broker_nodes = 3
msk_ebs_volume_size        = 200

# OpenSearch
opensearch_instance_type   = "r6g.large.search"
opensearch_instance_count  = 3
opensearch_ebs_volume_size = 100

# Monitoring
alarm_email = ""
