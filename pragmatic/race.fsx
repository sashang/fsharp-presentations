open System

type Driver =
    {
        Name: string
        Age: int
        Wins: int
    }

type Race =
    {
        Name: string
        Date: DateTime
    }

type Team =
    {
        Name: string
        Drivers: Driver list
    }