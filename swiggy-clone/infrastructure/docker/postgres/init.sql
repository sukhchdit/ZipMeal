-- ═══════════════════════════════════════════════════════════════════
-- Swiggy Clone — PostgreSQL Initialization Script
-- ═══════════════════════════════════════════════════════════════════
-- This script runs once when the postgres container is first created.
-- It installs the extensions required by the application.
-- ═══════════════════════════════════════════════════════════════════

-- uuid-ossp: Generates RFC 4122 UUIDs used as primary keys throughout
--            the domain entities (restaurants, orders, users, etc.).
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- cube + earthdistance (CASCADE installs cube automatically):
--   Provides earth_distance() and earth_box() functions used for
--   geo-proximity queries — e.g., "find restaurants within 5 km".
CREATE EXTENSION IF NOT EXISTS "earthdistance" CASCADE;

-- pg_trgm: Trigram-based fuzzy text search used for restaurant and
--          menu-item name matching (LIKE / ILIKE / similarity()).
CREATE EXTENSION IF NOT EXISTS "pg_trgm";
