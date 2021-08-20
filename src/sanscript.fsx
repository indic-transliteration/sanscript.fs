module Indic.Sanscript.Fsi

#r "nuget: Samboy063.Tomlet"
#load "../schemes/toml/Toml.fs"
#load "../schemes/Schemes.fs"
#load "Sanscript.fs"

open System.IO
open System.Reflection
open Indic.Sanscript.Schemes
open Tomlet.Models

let cwd = Directory.GetCurrentDirectory()
let asm = Assembly.LoadFile($"{cwd}/bin/Debug/net5.0/Indic.Sanscript.Schemes.Toml.dll")
let s = Schemes.scheme asm "tamil"

// let tamil = (asm.GetManifestResourceNames() |> Array.filter(fun m -> m.EndsWith("tamil.toml"))).[0]

// // Test: Just to check if we are able to decode a specific toml file
// let l,s = (Schemes.tryDecodeScheme asm tamil |> Async.RunSynchronously).Value
