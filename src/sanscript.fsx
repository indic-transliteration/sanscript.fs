module Sanscript.Fsi

#r "nuget: Samboy063.Tomlet"
#load "../schemes/toml/Toml.fs"
#load "../schemes/Schemes.fs"
#load "Sanscript.fs"

open System.IO
open System.Reflection
open Indic.Sanscript.Schemes
open Tomlet.Models

let cwd = Directory.GetCurrentDirectory()
let assembly = Assembly.LoadFile($"{cwd}/bin/Debug/net5.0/Sanscript.dll")
let tryTomlDecode =
  Schemes.tryDecodeScheme Toml.parse assembly

// Test: Just to check if we are able to decode a specific toml file
let l,d = tryTomlDecode "Sanscript.toml.brahmic.tamil.toml"
          |> Async.RunSynchronously |> Option.get   // TODO

