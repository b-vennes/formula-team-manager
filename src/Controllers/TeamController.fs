namespace FormulaTeamManager.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FormulaTeamManager.Actions
open FormulaTeamManager.Queries
open StackExchange.Redis

[<ApiController>]
[<Route("[controller]")>]
type TeamController (logger: ILogger<TeamController>, redis: ConnectionMultiplexer) =
    inherit ControllerBase()

    [<HttpGet("QueryById/{id}")>]
    member this.QueryById(id: string) =
        let handleResult result =
            match result with
            | Ok team -> team |> this.Ok :> IActionResult
            | Error -> "Team not found." |> this.NotFound :> IActionResult

        queryTeam redis id |> handleResult

    [<HttpGet("QueryAll")>]
    member this.QueryAll() =
        let handleResult result =
            match result with
            | Ok teams -> teams |> this.Ok :> IActionResult
            | Error -> this.StatusCode(500) :> IActionResult

        queryTeams redis |> handleResult

    [<HttpPost("Initialize")>]
    member this.Initialize(parameters: InitializeTeamParameters) =
        let handleResult result =
            match result with
            | Ok -> this.Ok() :> IActionResult
            | Error -> this.StatusCode(500) :> IActionResult

        handleAction redis (InitializeTeam(parameters)) |> handleResult

    [<HttpPost("ChangeTeamName")>]
    member this.ChangeTeamName(parameters: ChangeTeamNameParameters) =
        let handleResult result =
            match result with
            | Ok -> this.Ok() :> IActionResult
            | Error -> this.StatusCode(500) :> IActionResult

        handleAction redis (ChangeTeamName(parameters)) |> handleResult
