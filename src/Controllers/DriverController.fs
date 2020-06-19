namespace FormulaTeamManager.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FormulaTeamManager

/// <summary>Handles driver command and query requests.</summary>
[<ApiController>]
[<Route("[controller]")>]
type DriverController (logger : ILogger<DriverController>) =
    inherit ControllerBase()

    