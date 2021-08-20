namespace Indic.Sanscript.Tests

open System.Reflection
open Tomlet
open Indic.Sanscript
open Indic.Sanscript.Schemes

module Logic =

  let isGoodScheme a m =
    let decode = async {
      let! res = Schemes.tryDecodeScheme a m
      return
        match res with
        | Some _ -> true
        | None -> false
      }
    decode |> Async.RunSynchronously