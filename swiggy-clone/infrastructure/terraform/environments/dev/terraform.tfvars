# =============================================================================
# Dev Environment — Small sizing for cost efficiency
# =============================================================================

environment = "dev"
aws_region  = "ap-south-1"

# VPC
vpc_cidr           = "10.0.0.0/16"
availability_zones = ["ap-south-1a", "ap-south-1b", "ap-south-1c"]

# EKS
eks_cluster_version     = "1.29"
eks_node_instance_types = ["t3.small"]
eks_node_desired_size   = 1
eks_node_min_size       = 1
eks_node_max_size       = 3

# RDS
rds_instance_class    = "db.t3.micro"
rds_allocated_storage = 20
rds_multi_az          = false

# ElastiCache
elasticache_node_type       = "cache.t3.micro"
elasticache_num_cache_nodes = 1

# MSK
msk_instance_type          = "kafka.t3.small"
msk_number_of_broker_nodes = 3
msk_ebs_volume_size        = 20

# OpenSearch
opensearch_instance_type   = "t3.small.search"
opensearch_instance_count  = 1
opensearch_ebs_volume_size = 10
