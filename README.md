# Formula Team Manager

This project is an example/prototype of event sourcing with a functional programming language.

It models a system that helps [Formula One](https://www.formula1.com/) teams manage their resources.

## Build and Run

Before building and running the program, be sure to have [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core) installed on your machine.

Then, open the */src* directory in terminal session and execute the command `dotnet run`.

## Configurations

### Launch Settings

Add a folder named *Properties* in the */src* directoy.

Create a file named *launchSettings.json* within the *Properties* directory with the following text: 

``` Json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "FormulaTeamManager": {
      "commandName": "Project",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

```

### App Settings

Add a file named *appsettings.json* in the */src* folder and add the following text:

``` Json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}

```
