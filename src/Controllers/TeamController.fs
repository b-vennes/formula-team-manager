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
    member __.QueryById(id: string) =
        queryTeam redis id

    [<HttpGet("QueryAll")>]
    member __.QueryAll() =
        logger.LogInformation "test" |> ignore
        queryTeams redis

    [<HttpPost("Initialize")>]
    member __.Initialize(parameters: InitializeTeamParameters) =
        handleAction redis (InitializeTeam(parameters))

    [<HttpPost("ChangeTeamName")>]
    member __.ChangeTeamName(parameters: ChangeTeamNameParameters) =
        handleAction redis (ChangeTeamName(parameters))
