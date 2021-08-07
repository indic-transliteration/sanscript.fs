module FScript

#r "nuget: Samboy063.Tomlet"
#load "Sanscript.Core.fs"

open System.Reflection
open FSharp.Indic.Sanscript
open Tomlet

let assembly = Assembly.LoadFile("/home/dev/work/sanscript.fs/src/bin/Debug/net5.0/sanscript.dll")

// Inject Toml decoding function
let tryTomlDecode =
    let tomlDecode =
        let p = TomlParser()
        p.Parse
    SansCore.tryDecodeScheme tomlDecode assembly 

// Test: Just to check if we are able to decode a specific toml file
let l,t = tryTomlDecode "sanscript.toml.brahmic.devanagari.toml" 
          |> Async.RunSynchronously |> Option.get   // TODO