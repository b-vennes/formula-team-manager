namespace FormulaTeamManager.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FormulaTeamManager.Actions
open FormulaTeamManager.Queries
open StackExchange.Redis

[<ApiController>]
[<Route("[controller]")>]
type CarController (logger: ILogger<CarController>, redis: ConnectionMultiplexer) =
    inherit ControllerBase()

    [<HttpPost("Initialize")>]
    member __.Initialize([<FromBody>] parameters: InitializeCarParameters) =
        logger.LogInformation "Initializing car"
        handleAction redis (InitializeCar(parameters))

    [<HttpPost("AddPart")>]
    member __.AddPart([<FromBody>] parameters: AddPartParameters) =
        logger.LogInformation "Adding part"
        handleAction redis (AddPart(parameters))

    [<HttpGet("QueryById/{carId}")>]
    member __.QueryById(carId: string) =
        queryCar redis carId

    [<HttpGet("QueryByTeamId/{teamId}")>]
    member __.QueryByTeamId(teamId: string) =
        queryCars redis teamId
