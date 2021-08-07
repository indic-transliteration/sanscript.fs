module FScript

#r "nuget: Samboy063.Tomlet"
#load "Sanscript.Core.fs"

open System.IO
open System.Reflection
open FSharp.Indic.Sanscript
open Tomlet

let cwd = Directory.GetCurrentDirectory()
let assembly = Assembly.LoadFile($"{cwd}/bin/Debug/net5.0/Sanscript.dll")

// Inject Toml decoding function
let tryTomlDecode =
    let tomlDecode =
        let p = TomlParser()
        p.Parse
    SansCore.tryDecodeScheme tomlDecode assembly 

// Test: Just to check if we are able to decode a specific toml file
let l,t = tryTomlDecode "Sanscript.toml.brahmic.devanagari.toml" 
          |> Async.RunSynchronously |> Option.get   // TODO