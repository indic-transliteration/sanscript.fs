open Indic.Sanscript

module Program = 
  
  [<EntryPoint>] 
  let main _ = 
    let schemes = Sanscript.t "" "" "" ""
    printfn "%A" schemes
    0
