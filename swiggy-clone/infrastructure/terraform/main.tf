# =============================================================================
# Root Module — Composes all infrastructure modules
# =============================================================================

locals {
  name_prefix = "${var.project_name}-${var.environment}"
}

# --- Networking ---
module "vpc" {
  source = "./modules/vpc"

  name_prefix        = local.name_prefix
  vpc_cidr           = var.vpc_cidr
  availability_zones = var.availability_zones
  environment        = var.environment
}

# --- Container Orchestration ---
module "eks" {
  source = "./modules/eks"

  name_prefix         = local.name_prefix
  cluster_version     = var.eks_cluster_version
  vpc_id              = module.vpc.vpc_id
  private_subnet_ids  = module.vpc.private_subnet_ids
  node_instance_types = var.eks_node_instance_types
  node_desired_size   = var.eks_node_desired_size
  node_min_size       = var.eks_node_min_size
  node_max_size       = var.eks_node_max_size
  environment         = var.environment
}

# --- Database ---
module "rds" {
  source = "./modules/rds"

  name_prefix        = local.name_prefix
  vpc_id             = module.vpc.vpc_id
  database_subnet_ids = module.vpc.database_subnet_ids
  instance_class     = var.rds_instance_class
  allocated_storage  = var.rds_allocated_storage
  multi_az           = var.rds_multi_az
  db_name            = var.rds_db_name
  eks_security_group_id = module.eks.node_security_group_id
  environment        = var.environment
}

# --- Cache ---
module "elasticache" {
  source = "./modules/elasticache"

  name_prefix           = local.name_prefix
  vpc_id                = module.vpc.vpc_id
  private_subnet_ids    = module.vpc.private_subnet_ids
  node_type             = var.elasticache_node_type
  num_cache_nodes       = var.elasticache_num_cache_nodes
  eks_security_group_id = module.eks.node_security_group_id
  environment           = var.environment
}

# --- Event Streaming ---
module "msk" {
  source = "./modules/msk"

  name_prefix            = local.name_prefix
  vpc_id                 = module.vpc.vpc_id
  private_subnet_ids     = module.vpc.private_subnet_ids
  instance_type          = var.msk_instance_type
  number_of_broker_nodes = var.msk_number_of_broker_nodes
  ebs_volume_size        = var.msk_ebs_volume_size
  eks_security_group_id  = module.eks.node_security_group_id
  environment            = var.environment
}

# --- Search ---
module "opensearch" {
  source = "./modules/opensearch"

  name_prefix           = local.name_prefix
  vpc_id                = module.vpc.vpc_id
  private_subnet_ids    = module.vpc.private_subnet_ids
  instance_type         = var.opensearch_instance_type
  instance_count        = var.opensearch_instance_count
  ebs_volume_size       = var.opensearch_ebs_volume_size
  eks_security_group_id = module.eks.node_security_group_id
  environment           = var.environment
}

# --- Storage & CDN ---
module "s3" {
  source = "./modules/s3"

  name_prefix = local.name_prefix
  environment = var.environment
}

# --- Container Registry ---
module "ecr" {
  source = "./modules/ecr"

  name_prefix = local.name_prefix
  environment = var.environment
}

# --- Monitoring ---
module "monitoring" {
  source = "./modules/monitoring"

  name_prefix      = local.name_prefix
  eks_cluster_name = module.eks.cluster_name
  alarm_email      = var.alarm_email
  environment      = var.environment
}
