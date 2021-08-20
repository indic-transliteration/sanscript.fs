namespace Indic.Sanscript.Schemes

open System.Collections.Generic
open Tomlet
open Tomlet.Models

module Toml =
  // Type for identifying this scheme
  type TomlType = class end

  // Given a TOML table for a specific character type (Eg: vowels, consonants, etc.),
  // converts this into a generic dictionary form that can be consumed in other modules.
  let private mapt (t: TomlTable) =
    t.Entries
    |> Seq.map (fun kv -> (kv.Key, kv.Value))
    |> Map.ofSeq


  // Function to parse TOML files
  let parse s =
    let p = TomlParser()
    p.Parse s

  // Converts a list of TomlValues to their string representation
  let private tomlvalToStringList (l: TomlValue list) = l |> List.map (fun v -> v.StringValue)

  // Given a TOML Document (i.e., parsed output of a TOML file), and a specific character
  // type (Eg: vowel, consonants, etc.), returns back a Map of the TomlTable that
  // the character table represents - when the values of the table represent a string.
  let maps (d: TomlDocument) (s: string) =
    try
      let t = d.GetSubTable(s)
      Some (mapt t |> Map.map (fun k v -> v.StringValue))
    with
      | _ -> None

  // Given a TOML Document (i.e., parsed output of a TOML file), and a specific character
  // type (Eg: vowel, consonants, etc.), returns back a Map of the TomlTable that
  // the character table represents - when the values of the table represent an array.
  let mapl (d: TomlDocument) (s: string) =
    try
      let t = d.GetSubTable(s)
      let m =
        mapt t
        |> Map.map (fun k v ->
                      (v :?> TomlArray).ArrayValues
                       |> List.ofSeq
                       |> tomlvalToStringList)
      Some m
    with
      | _ -> None