namespace FSharp.Indic.Sanscript.Tests

open System.IO
open FSharp.Indic.Sanscript

module Logic =
  let stream s =
    new StringReader(s)
    
  let isGoodScheme s = 
    let s = stream s
    let res = Sanscript.decodeScheme s
    match res with
    | Ok _ -> true
    | Error _ -> false