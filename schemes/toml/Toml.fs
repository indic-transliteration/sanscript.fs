namespace Indic.Sanscript.Schemes

open Tomlet
open Tomlet.Models

module Toml =
  // Type for identifying this scheme
  type TomlType = class end

  // Given a TOML table for a specific character type (Eg: vowels, consonants, etc.),
  // converts this into a generic dictionary form that can be consumed in other modules.
  let private mapt (t: TomlTable) =
    t.Entries
    |> Seq.map (fun kv -> (kv.Key, kv.Value.StringValue))
    |> dict

  // Function to parse TOML files
  let parse s =
    let p = TomlParser()
    p.Parse s

  // Given a TOML Document (i.e., parsed output of a TOML file), and a specific character
  // type (Eg: vowel, consonants, etc.), returns back a Dictionary form of the TomlTable that
  // the character table represents.
  let map (d: TomlDocument) (s: string) =
    let t = d.GetSubTable(s)
    mapt t