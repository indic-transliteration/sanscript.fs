namespace FSharp.Indic.Sanscript.Tests

open FSharp.Indic.Sanscript

module Logic =
  let isGoodScheme s = 
    let res = Sanscript.decodeScheme s
    match res with
    | Ok _ -> true
    | Error _ -> false