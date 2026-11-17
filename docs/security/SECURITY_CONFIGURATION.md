# Sprint 13 - Security configuration

## JWT signing key

The JWT signing key is intentionally not stored in `appsettings.json` or
`appsettings.Development.json`.

For local development, initialize .NET User Secrets for the API project:

```powershell
dotnet user-secrets init --project src/AircraftMaintenanceOperations.API
```

Generate a new random 256-bit key and store it locally (do not commit it):

```powershell
$key = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
dotnet user-secrets set "Jwt:SecretKey" "$key" --project src/AircraftMaintenanceOperations.API
```

The API requires `Jwt:SecretKey` to contain at least 32 UTF-8 bytes. The issuer,
audience, and token expiration are non-secret settings in `appsettings.json`.
For deployment, supply the key through a secret manager or the environment
variable `Jwt__SecretKey`.

Changing the key invalidates tokens signed with the previous key. Rotate the
old key anywhere it may have been used.

## Development-only seeded accounts

Development account seeding is disabled by default. To opt in locally, set:

```powershell
dotnet user-secrets set "DevelopmentSeedUsers:Enabled" "true" --project src/AircraftMaintenanceOperations.API
```

If enabled, configure all six passwords in User Secrets. Each must satisfy the
configured Identity password policy (12+ characters, upper/lowercase, digit,
non-alphanumeric character, and at least four unique characters):

```powershell
dotnet user-secrets set "DevelopmentSeedUsers:AdminPassword" "<unique-local-password>" --project src/AircraftMaintenanceOperations.API
dotnet user-secrets set "DevelopmentSeedUsers:SupervisorPassword" "<unique-local-password>" --project src/AircraftMaintenanceOperations.API
dotnet user-secrets set "DevelopmentSeedUsers:TechnicianPassword" "<unique-local-password>" --project src/AircraftMaintenanceOperations.API
dotnet user-secrets set "DevelopmentSeedUsers:InventoryPassword" "<unique-local-password>" --project src/AircraftMaintenanceOperations.API
dotnet user-secrets set "DevelopmentSeedUsers:PilotPassword" "<unique-local-password>" --project src/AircraftMaintenanceOperations.API
dotnet user-secrets set "DevelopmentSeedUsers:OperationsPassword" "<unique-local-password>" --project src/AircraftMaintenanceOperations.API
```

The API startup only invokes account seeding in the Development environment.
Role creation remains idempotent and is performed at startup. Existing accounts
in a database are not deleted or automatically reset by disabling seeding;
review and rotate any previously seeded accounts separately.

## Previously committed credentials

The prior JWT signing key and development account passwords were present in
Git history. Treat them as exposed. Rotate the JWT key and any seeded account
passwords that may have been used against a reachable database. Removing values
from the current branch does not remove them from earlier commits. If the
repository is public, assess whether history rewriting is appropriate after
coordinating with collaborators; history rewriting does not replace credential
rotation.
