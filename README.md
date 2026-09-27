# Travel Guide API

A school project to learn ASP.NET, Entity Framework, Design Architecture, and RESTful API development.

> **Note:** This project is built for SQL Server only. For other database support, you'll need to build your own implementation.

## Setup Instructions

1. Paste the `secrets.json` into the user secrets file
2. Start the Docker container
3. Open the `_scripts` folder in an integrated terminal and run:
  ```bash
  ./database-rebuild-all.sh travelguide sqlserver docker root ../AppWebApi
  ```
  for Windows:
  ```bash
  .\database-rebuild-all.sh travelguide sqlserver docker root ../AppWebApi
  ```
4. Build the SQL views and stored procedures in `/DbContext/SqlScript`

Happy coding! 🚀