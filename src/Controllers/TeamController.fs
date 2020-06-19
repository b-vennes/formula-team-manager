namespace FormulaTeamManager.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FormulaTeamManager

/// <summary>Handles command and query requests for teams.</summary>
/// <param name="logger">The standard logging utility.</param>
/// <param name="eventLogger">The event logging utility.</param>
[<ApiController>]
[<Route("[controller]")>]
type TeamController (logger : ILogger<TeamController>, eventLogger: EventLogger) =
    inherit ControllerBase()

    /// <summary>Performs a team name change.</summary>
    /// <param name="parameters">The name change parameters.</param>
    /// <returns>An error message if an error has occurred.</returns>
    [<HttpPost("ChangeTeamName")>]
    member __.ChangeTeamName([<FromBody>] parameters: ChangeTeamNameParameters) : Option<string> =
        {
            Event = 
                TeamEvent(
                    ChangedTeamName(
                        ChangedTeamNameEvent(parameters.PreviousTeamName, parameters.NewTeamName)));
            Timestamp = DateTime.Now;
        }
        |> eventLogger.Log

        None