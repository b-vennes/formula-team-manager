namespace FormulaTeamManager

module Results =

    type Car = { Id: string; TeamId: string; Parts: List<string> }

    type Team = { Id: string; TeamName: string }

    type CarNotFoundError = { CarId: string }

    type TeamNotFoundError = { TeamId: string }

    type TeamInformation = { TeamId: string; TamName: string; Cars: list<Car> }
