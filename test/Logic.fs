namespace FSharp.Indic.Sanscript.Tests

open System.Reflection
open Tomlet
open FSharp.Indic.Sanscript

module Logic =

  let tryTestTomlDecode a =
      let parse =
          let p = TomlParser()
          p.Parse
      SansCore.tryDecodeScheme parse a   

  let isGoodScheme a m = 
    let decode = async { 
      let! res = tryTestTomlDecode a m
      return 
        match res with
        | Some _ -> true
        | None -> false
      } 
    decode |> Async.RunSynchronously