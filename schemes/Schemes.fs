namespace Indic.Sanscript.Schemes

open System
open System.IO
open System.Reflection
open System.Text
open System.Collections.Generic
open System.Runtime.CompilerServices
open Tomlet.Models

// Each of the language schemes have the following components
// Regardless of the format of the schemes themselves, we extract
// the components in this form for any language.
// The schemes for each language is read from a language-specific
// scheme file. The scheme files may be of any format: JSON, or TOML.
// In the current implementation, we have scheme files in TOML format.
type Scheme =
  { Vowels: Map<string, string>
    Virama: Map<string, string>
    Consonants: Map<string, string>
    Symbols: Map<string, string>

    // Optional items that may not be configured in
    // the scheme files, depending on the language
    VowelMarks: Map<string, string> option // Roman scripts do not have vowel_marks
    Yogavaahas: Map<string, string> option
    ExtraConsonants: Map<string, string> option
    Accents: Map<string, string> option
    AccentedVowelAlternates: Map<string, string list> option
    Zwj: Map<string, string> option
    Skip: Map<string, string> option
    mutable Alternates: Map<string, string list> option
    Candra: Map<string, string> option // Roman scripts do not have candra
    Other: Map<string, string> option

    // Finally, we have a calculated field: any scheme read from the
    // roman directory, the following field will be made true.
    IsRoman: bool }
  with
    // Get the values only from the maps - this is used for adding alternates for some of the characters
    member private x.Values (m: Map<string, string>) = m |> Map.toList |> List.map (fun (k,v) -> v)

    // Get only the values of some maps as a list
    member x.VowelsList = x.Values x.Vowels
    member x.ConsonantList = x.Values x.Consonants
    member x.ExtraConsonantList =
      if x.ExtraConsonants.IsSome then
        x.Values x.ExtraConsonants.Value
      else []

// We want all our tests to access internal members from here
module SchemesAssemblyInfo =
  [<assembly: InternalsVisibleTo("Indic.Sanscript.Test")>]
  do()

// The main Schemes module
module Schemes =

  // Placeholder for unimplemented functions
  let private undefined<'T> : 'T = failwith "Not implemented yet"

  // Private module that deals with loading language scheme
  // files and decoding them
  module private Internal =
    //------------- Begin: Language Scheme-format-specific functions ----
    // These are the only configurations required when a language scheme
    // is changed between other formats JSON, TOML, etc.

    // Parse function, that takes in language scheme data and emits out a
    // format-specific form of the data. Eg: Toml.parse emits out a
    // TomlDocument. It does not matter what is the type of the emitted format,
    // (i.e., this emitted format is opaque, and simply is provided as an
    // input to the map function below.)
    let parse = Toml.parse

    // A function that takes in the emitted parsed output (above), and returns
    // back a standard F# Map<string, string> for consumption in other modules.
    let maps d s= (Toml.maps d s).Value

    // A function that takes in the emitted parsed output (above), and returns
    // back a standard F# Map<string, string list> for consumption in other modules.
    let mapl d s= (Toml.mapl d s).Value

    // Same as maps and mapl, but returns an option: these return None if
    // the said character type is not given in the Toml files.
    let mapso = Toml.maps
    let maplo = Toml.mapl

    // Add and retrieve boolean values
    let addbool = Toml.addBoolValue
    let getBool = Toml.getBoolValue

    //------------- End: Language Scheme-format-specific functions ----

    // Decode scheme data into a decoded object
    // This decoded object depends on the format of
    // the scheme (JSON, TOML, etc.)
    // s: Scheme data from a language scheme file as a string
    // returns: Result<DecodedObject, string>:
    //          Ok -> Decoded object if successful in parsing
    //          Error -> an error message.
    let decodeScheme (s: string) =
      try
        let scheme = parse s
        Ok scheme
      with
      | ex -> Error ex.Message

    // Get data out of a scheme file
    // m: Manifest name of the scheme file (Eg: "sanscript.toml.brahmic.devanagari.toml")
    let data (a: Assembly) (m: string) =
      let s = a.GetManifestResourceStream(m)
      use r = new StreamReader(s, Encoding.UTF8)
      r.ReadToEnd()

    // Get the name of the language from its scheme file name
    // m: Scheme file name (manifest name)
    let lang (m: string) =
      // Language schemes have this name:
      // sanscript.toml.<scheme>.<lang>.toml
      // Where <scheme> is "brahmic" or "roman"
      // and <lang> is what we have to extract
      // Basically, the last 3 parts of the scheme name
      // minus the last "toml" is what we should get -
      // this will allow for any change in the folder
      // names of the schemes in the future (the last three)
      // parts will remain the same regardless of where the
      // schemes are kept in the folder-tree.
      let exti = m.LastIndexOf(".")
      let filei = if exti > 0 then m.LastIndexOf(".", exti - 1) else -1
      m.Substring(filei + 1,(exti - filei - 1))

    // List of manifests (language scheme files) that contain the language schemes
    let manifests (a: Assembly) =
      a.GetManifestResourceNames()
      |> Array.filter (fun m -> m.StartsWith("Indic.Sanscript.Schemes.Toml"))

    // Given a document-form of the scheme (format-specific, such
    //  as TOML or JSON) extracts the scheme form from that.
    let scheme doc =
      { Vowels = maps doc "vowels"
        Virama = maps doc "virama"
        Consonants = maps doc "consonants"
        Symbols = maps doc "symbols"

        // Optional items that may not be configured in
        // the scheme files, depending on the language
        VowelMarks = mapso doc "vowel_marks"
        Yogavaahas = mapso doc "yogavaahas"
        ExtraConsonants = mapso doc "extra_consonants"
        Accents = mapso doc "accents"
        AccentedVowelAlternates = maplo doc "accented_vowel_alternates"
        Zwj = mapso doc "zwj"
        Skip = mapso doc "skip"
        Alternates = maplo doc "alternates"
        Candra = mapso doc "candra"
        Other = mapso doc "other"

      // Finally, we have a calculated field: any scheme read from the
      // roman directory, the following field will be made true.
        IsRoman = getBool doc "IsRoman" }

  // Try decoding a scheme manifest file
  // m: Manifest file name
  // returns: Option<(string, DecodedObject)>
  //          Success: tuple (<language name>, <parsed scheme object>)
  //          Failure: None
  let tryDecodeScheme (a: Assembly) (m: string) = async {
    let scheme = Internal.data a m |> Internal.decodeScheme

    match scheme with
    | (Ok doc) ->
        Toml.addBoolValue doc "IsRoman" (m.Contains(".roman."))
        return Some (Internal.lang m, Internal.scheme doc)
    | (Error msg) ->
      Console.WriteLine($"Unable to parse {m}: {msg}")
      return None
  }

  /// <summary>
  ///   Returns a map of all schemes parsed from scheme files.
  /// <para>
  ///   Gets the complete scheme map from the parsed scheme files
  ///   and returns the result in a format-specific object. The
  ///   map is Map&lt;language, Scheme&gt;.
  /// </para>
  /// </summary>
  ///
  /// <param name="asm">Name of the assembly in which the
  ///        scheme files are present as manifests
  /// </param>
  ///
  /// <returns>Language Scheme</returns>
  ///
  // Map of (<name of the language>, <TOML Document>)
  let schememap (asm: Assembly) =
    Internal.manifests asm
    |> Array.map (tryDecodeScheme asm)
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.choose id
    |> Map.ofArray