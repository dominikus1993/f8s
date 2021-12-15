namespace FSharpNetes
open System.Linq

module internal Utils =
    let toList(s: _ list) = s.ToList()

module internal Enumerable = 
    let toList(s: _ seq) = s.ToList()

