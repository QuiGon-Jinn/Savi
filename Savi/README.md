# Savi
Senior Software Developer Technical Vetting Assignment

Change the SqlDatabase config in appsettings.json to your local database connection string.

Create database by running the following command in the terminal:
dotnet ef database update --project Data --startup-project SaviWebApi

Insert demo data by running the PopulateData.sql scripts in sql server.

All users have the same password: "pass".

Run to open the swagger page where the APIs can be tested in your browser: