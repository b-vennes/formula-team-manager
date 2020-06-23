namespace FormulaTeamManager

open FormulaTeamManager.Eventing
open System

module Actions =

    type InitializeCarParameters() =
        let mutable teamId = ""
        let mutable partNames = ""

        member __.TeamId
            with get () = teamId
            and set (value) = teamId <- value

        member __.PartNames
            with get() = partNames
            and set(value) = partNames <- value

    type AddPartParameters() =
        let mutable carId =""
        let mutable partName = ""

        member __.CarId
            with get () = carId
            and set (value) = carId <- value

        member __.PartName
            with get () = partName
            and set (value) = partName <- value

    type InitializeTeamParameters() =
        let mutable teamName = ""

        member __.TeamName
            with get () = teamName
            and set (value) = teamName <- value

    type ChangeTeamNameParameters() =
        let mutable teamId = ""
        let mutable teamName = ""

        member __.TeamId
            with get () = teamId
            and set (value) = teamId <- value

        member __.TeamName
            with get () = teamName
            and set (value) = teamName <- value

    type ActionParameters =
        | InitializeCar of InitializeCarParameters
        | AddPart of AddPartParameters
        | InitializeTeam of InitializeTeamParameters
        | ChangeTeamName of ChangeTeamNameParameters

    let handleAction redis actionParameters =
        let handleInitializeCarAction (parameters: InitializeCarParameters) =
            { TeamId = parameters.TeamId; CarId = Guid.NewGuid().ToString() ; PartNames = parameters.PartNames.Split(",") |> Array.toList }
            |> InitializedCar
            |> EventLogging.logEvent redis
            |> Ok

        let handleAddPartAction (parameters: AddPartParameters) =
            { CarId = parameters.CarId; PartName = parameters.PartName }
            |> AddedPart
            |> EventLogging.logEvent redis
            |> Ok

        let handleInitializeTeamAction (parameters: InitializeTeamParameters) =
            { InitializedTeamEvent.TeamId = Guid.NewGuid().ToString(); TeamName = parameters.TeamName}
            |> InitializedTeam
            |> EventLogging.logEvent redis
            |> Ok

        let handleChangeTeamAction (parameters: ChangeTeamNameParameters) =
            { TeamId = parameters.TeamId ; TeamName = parameters.TeamName}
            |> ChangedTeamName
            |> EventLogging.logEvent redis
            |> Ok

        match actionParameters with
        | InitializeCar parameters -> handleInitializeCarAction parameters
        | AddPart parameters -> handleAddPartAction parameters
        | InitializeTeam parameters -> handleInitializeTeamAction parameters
        | ChangeTeamName parameters -> handleChangeTeamAction parameters
