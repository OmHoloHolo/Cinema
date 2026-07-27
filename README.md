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

# API routes description

The recommended starting point, after completing the authorization, is retrieving the list of the screenings from `GET /screenings` on the Show service.
With the obtained screeing ids you can switch to the Booking service, there you can perform some operations

## Booking routes

| Method  |                  Route                       |               Description                                                                                                                            |
|-------- |----------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------|
| GET     | /screenings/{screeningId}/available-seats    | Check the available seats of a screening                                                                                                             |
| POST    | /screenings/{screeningId}/reservations       | Create a reservation specifing a seat. <br> You can omit the field seatId or set it null to let the system get a random seat from the available ones |
| DELETE  | /screenings/{screeningId}/reservations       | Delete an existing reservation                                                                                                                       |
| POST    | /multiple-reservations                       | Make a request for a multiple creation reservation to reserve seat from the same or different screening                                              |
| GET     | /screenings/{screeningId}/reservations       | Check all the reservations of a screening                                                                                                            |

## Show routes
| Method  |       Route                       |               Description                                                                                                                            |
|-------- |-----------------------------------|----------------------------------------------------------------------------------------------------------------|
| GET     | /screenings                       | Check the all the screenings in the system                                                                     |
| GET     | screenings/{screeningId}/seats    | Check all the existing seats of a screening (used by Booing service to build the data for the available seats) |


# CI Pipeline

The pipeline will build and test the entire solution.
When it's run by a user, allows you to choose which artifacts publish between the two projects.
