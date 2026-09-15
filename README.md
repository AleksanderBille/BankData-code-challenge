# API
The API was written in ASP.NET mainly because it is a framework i have a lot of experience with. I am using EFCore because it is easy to define the structure of the database programatically and it generates migrations by itself.

## The solution
I used Supabase for the DB because it lets me use its own auth plane instead of having to create my own. I have defined account and transaction. Account is bound to a given user so that i can check for each endpoint whether that user has access to that account. It alse lets be retieve all accounts for one user. I created a transaction data class for audits on a user. A transaction is saves when the transfor business logic completes. 

I created a service that i integrated into the controller to sepperate the more complex business logic from the traffic layer. The service only allows you to transfor money out of your own account, you can however transfor to any account. I created checks for whether the amount was positive and whether the from account had a high enough balance for the transfor to take place. 

I created a small testing suite for the API. Since the transfor logic is contained in a service i could test this alone by injecting a mock context. For the rest of the API i created a custom web app facotory that hooks into the program and replaces the database specific service middleware with an in memory database so the testing does not affect or depend on the Supabase DB. I used a testauthhandler the replaces the auth handler provided by supabase. I created a very simple react frontend that lets you access the API.

The database is hosted in Supabase and the API is hosted through Google Cloud Run so the only thing that needs deploying is the frontend.

## Running the application
API and Database is running in the cloud.

- cd into Fronend folder
- create .env file (I will have sent the contents of the .env file in the email)
- npm i
- npm run dev

## Using the application
- create a user in the frontend e.g bankuser@mail.dk / password123
- log into the frontned
- create two accounts with some balance
- use transfer to move money between the two accounts or to another valid account id (I created an account with id 5)
- Try transfering more money than is available in the account balance
- Try moving from an account id you dont own
- Try moving a negative amount.
