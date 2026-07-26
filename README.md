# Cinema

A little project that manages the basic reservation operations of a cinema, made in .NET 10.
The solution is composed of two C# projects: Show and Booking.

### Show
Represents and manages the domain of the shows details, it knows the screenings in program, the rooms and the seats in the cinema and has the movies catalog.

### Booking
Has the control of the reservations, knows how to combine the data retrieved from Show service to validate and create or cancel the reservation of a screening.

## Technical details

As mentioned above, the solution is built in .NET 10, both services are web applications, with their own API routes and their own databases.
### API
The routes can be invoked through the Swagger UI and require JWT authentication (The token can be obtained via `GET /auth/token`).
### Database
For simplicity, the database is created using SQLite. A migration that populates the basic information about the shows, like screenings, movies, seats and rooms, 
will run at the startup of the Show service, for the Booking service the migration will just create the database structure without pre-load any data.

## Local setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Build

From the repository root:

```bash
dotnet build Cinema.slnx
```

### Run

Start each service in a separate terminal:

```bash
dotnet run --project src/Show/Show.csproj
```

```bash
dotnet run --project src/Booking/Booking.csproj
```

| Service | URL                              | Swagger                               |
|---------|----------------------------------|---------------------------------------|
| Show    | http://localhost:5000            | http://localhost:5000/swagger         |
| Booking | http://localhost:5001            | http://localhost:5001/swagger         |

> **Note:** Since Booking depends on Show data, Show has to be up to resolve available seats information.

### Run tests

```bash
dotnet test Cinema.slnx
```
