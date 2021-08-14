﻿namespace Indic.Sanscript.Schemes

open Tomlet

module Toml =
  // Type for identifying this scheme
  type TomlType = class end

  // Function to parse TOML files
  let parse =
    let p = TomlParser()
    p.Parse