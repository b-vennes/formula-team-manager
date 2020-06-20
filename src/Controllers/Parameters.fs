namespace FormulaTeamManager.Controllers

module Parameters =

    /// <summary>Parameters for adding a part to a car.</summary>
    type AddPartParameters() =
        let mutable teamName = ""
        let mutable partName = ""

        /// <summary>The name of the car's team.</summary>
        member __.TeamName
            with get () = teamName
            and set (value) = teamName <- value

        /// <summary>The name of the added part.</summary>
        member __.PartName
            with get () = partName
            and set (value) = partName <- value

    /// <summary>Parameters for changing a team's name.</summary>
    type ChangeTeamNameParameters() =
        let mutable previousTeamName = ""
        let mutable newTeamName = ""

        /// <summary>The team's previous name.</summary>
        member __.PreviousTeamName
            with get () = previousTeamName
            and set (value) = previousTeamName <- value

        /// <summary>The team's new name.</summary>
        member __.NewTeamName
            with get () = newTeamName
            and set (value) = newTeamName <- value