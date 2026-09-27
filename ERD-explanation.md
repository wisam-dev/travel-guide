# Database Design Explanation — TravelGuide

## Overview

The database models a travel/attractions app: countries contain cities, cities contain
addresses, and attractions (sights) sit at an address, belong to a category, and can receive
comments from users. Seven entities in total: **Country**, **City**, **Category**, **Address**,
**Attraction**, **User**, **Comment**.

## Entities

| Entity | Key fields | Purpose |
|---|---|---|
| **Country** | `CountryId` (PK), `Name` | One row per supported country (Sweden, Norway, Denmark, Finland in the seed data). |
| **City** | `CityId` (PK), `Name`, `CountryId` (FK) | A city belongs to exactly one country. |
| **Category** | `CategoryId` (PK), `Name` | Attraction categories (Restaurant, Café, Museum, etc.) as a proper lookup table rather than a hardcoded string/enum, so they can be queried and extended without code changes. |
| **Address** | `AddressId` (PK), `Street`, `ZipCode`, `CityId` (FK), `CountryId` (FK) | The physical location of one attraction. Kept as its own entity (rather than a flat string on Attraction) so street/zip/city/country are all properly normalized and independently queryable. |
| **Attraction** | `AttractionId` (PK), `Title`, `Description`, `CategoryId` (FK), `AddressId` (FK) | The central entity — a sight/place recommended to users. |
| **User** | `UserId` (PK), `Name`, `Email` | An app user who can leave comments. |
| **Comment** | `CommentId` (PK), `Text`, `CreatedAt`, `UserId` (FK), `AttractionId` (FK) | A user's comment on a specific attraction. |

Every table also has a `Seeded` (bool) column, marking rows created by the test-data generator so
they can be distinguished from — and bulk-deleted separately from — real data created through the
API's create endpoints.

## Relationships

| Relationship | Cardinality | Delete behavior |
|---|---|---|
| Country → City | one-to-many | Restrict (can't delete a country that still has cities) |
| Country → Address | one-to-many | Restrict |
| City → Address | one-to-many | Restrict |
| Category → Attraction | one-to-many | Restrict |
| Address → Attraction | one-to-many* | Restrict |
| User → Comment | one-to-many | **Cascade** (deleting a user deletes their comments) |
| Attraction → Comment | one-to-many | **Cascade** (deleting an attraction deletes its comments) |

\* Modeled as one-to-many at the database level, but by design each `Address` row is created
exclusively for one `Attraction` (never shared) — so it behaves as one-to-one in practice. The
API's delete-attraction endpoint removes the linked address along with it for that reason.

## Design decisions

- **`Guid` primary keys everywhere**, generated in code (`Guid.NewGuid()`) rather than
  database-identity integers, so new rows never depend on a round-trip to the database to get an
  ID, and IDs stay unique across environments (useful once seeding, testing, and real inserts mix
  in the same tables).
- **Normalization over convenience.** `Category`, `Country`, and `City` are all separate tables
  rather than strings on `Attraction`, and `Address` is separate from `Attraction` rather than a
  raw string field. This is what makes the required filter/count queries (by category, by
  country, by city; counting distinct cities/attractions) simple, indexed, joins instead of
  string matching.
- **`Address` deliberately duplicates `CountryId`** even though it's derivable through
  `City.CountryId`. This is a denormalization trade-off for query convenience (filtering an
  address by country doesn't need to join through City) — the application layer is responsible
  for keeping the two in sync (done automatically: `Address.CountryId` is always set from the
  chosen `City.CountryId`, never entered independently).
- **Cascade deletes are scoped narrowly** — only `Comment` cascades (from its `User` and its
  `Attraction`), matching the assignment's explicit requirement that deleting a user or an
  attraction also removes their comments. Every other relationship uses `Restrict`, so you can't
  accidentally delete a `Country`/`City`/`Category`/`Address` that's still referenced elsewhere.
- **Indexes** beyond the ones EF Core creates automatically for foreign keys: a unique index on
  `User.Email`, unique indexes on `Country.Name` and `Category.Name` (since each should only
  exist once), a searchable index on `City.Name` and `Attraction.Title`, and an index on the
  `Seeded` column of the two largest tables (`Attraction`, `Comment`) to keep the
  delete-test-data stored procedure fast.
- **Varchar length overrides.** The default convention maps every `string` column to
  `varchar(200)`; `Attraction.Description` and `Comment.Text` are widened (`varchar(2000)` and
  `varchar(1000)` respectively) since generated test descriptions/comments are full sentences or
  paragraphs that would otherwise get truncated.

See `erd.png` for the visual diagram.
