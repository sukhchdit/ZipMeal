# =============================================================================
# S3 Module Outputs
# =============================================================================

output "bucket_name" {
  description = "S3 media bucket name"
  value       = aws_s3_bucket.media.bucket
}

output "bucket_arn" {
  description = "S3 media bucket ARN"
  value       = aws_s3_bucket.media.arn
}

output "cdn_domain_name" {
  description = "CloudFront distribution domain name"
  value       = aws_cloudfront_distribution.media.domain_name
}

output "cdn_distribution_id" {
  description = "CloudFront distribution ID"
  value       = aws_cloudfront_distribution.media.id
}
