namespace FSharp.Indic

open System
open System.IO
open System.Reflection
open System.Text
open System.Collections.Generic
open Tommy
module Sanscript =
    let t data fromlang tolang options =
        let assembly = Assembly.GetExecutingAssembly()

        let tryDecodeScheme n = async {
            let s = assembly.GetManifestResourceStream(n)
            use r = new StreamReader(s, Encoding.UTF8)

            try
                let toml = TOML.Parse(r)
                return (Some toml)
            with
            | exn as ex -> 
                Console.WriteLine($"Unable to decode {n}: {ex.Message}")
                return None
        }

        let schemes = 
            assembly.GetManifestResourceNames()
            |> Array.filter (fun n -> n.StartsWith("stothramala"))
            |> Array.map tryDecodeScheme
            |> Async.Parallel
            |> Async.RunSynchronously 
            |> Array.choose id

        printfn "%A" schemes
