namespace Indic.Sanscript.Schemes

open System
open System.IO
open System.Reflection
open System.Text
open System.Collections.Generic
open System.Runtime.CompilerServices

// Each of the language schemes have the following components
// Regardless of the format of the schemes themselves, we extract
// the components in this form for any language.
type Scheme =
  { Vowels: Map<string, string> option
    VowelMarks: Map<string, string> option
    Yogavaahas: Map<string, string> option
    Virama: Map<string, string> option
    Consonants: Map<string, string> option
    Symbols: Map<string, string> option
    Zwj: Map<string, string> option
    Skip: Map<string, string> option
    Accents: Map<string, string> option
    AccentedVowelAlternates: Map<string, string list> option
    Candra: Map<string, string> option
    Other: Map<string, string> option
    ExtraConsonants: Map<string, string> option
    Alternates: Map<string, string list> option }

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
    let maps = Toml.maps

    // A function that takes in the emitted parsed output (above), and returns
    // back a standard F# Map<string, string list> for consumption in other modules.
    let mapl = Toml.mapl
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

  // Try decoding a scheme manifest file
  // m: Manifest file name
  // returns: Option<(string, DecodedObject)>
  //          Success: tuple (<language name>, <parsed scheme object>)
  //          Failure: None
  let tryDecodeScheme (a: Assembly) (m: string) = async {
    let scheme = Internal.data a m |> Internal.decodeScheme
    match scheme with
    | (Ok doc) -> return Some (Internal.lang m, doc)
    | (Error msg) ->
      Console.WriteLine($"Unable to parse {m}: {msg}")
      return None
  }

  // Map of (<name of the language>, <TOML Document>)
  let private schememap (a: Assembly) =
    Internal.manifests a
    |> Array.map (tryDecodeScheme a)
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.choose id
    |> Map.ofArray

  /// <summary>
  ///   Language scheme for a specific language
  ///
  /// <para>
  ///   Given name of a language, we get back the map of various character
  ///   types from that language.
  /// </para>
  /// </summary>
  ///
  /// <param name="asm">Name of the assembly in which the scheme files are present</param>
  /// <param name="lang">Name of the language (Eg: "devanagari")</param>
  ///
  /// <returns>Language Scheme</returns>
  ///
  let scheme (asm: Assembly) (lang: string) =
    let d = (schememap asm).[lang]
    { Vowels = Internal.maps d "vowels"
      VowelMarks = Internal.maps d "vowel_marks"
      Yogavaahas = Internal.maps d "yogavaahas"
      Virama = Internal.maps d "virama"
      Consonants = Internal.maps d "consonants"
      Symbols = Internal.maps d "symbols"
      Zwj = Internal.maps d "zwj"
      Skip = Internal.maps d "skip"
      Accents = Internal.maps d "accents"
      AccentedVowelAlternates = Internal.mapl d "accented_vowel_alternates"
      Candra = Internal.maps d "candra"
      Other = Internal.maps d "other"
      ExtraConsonants = Internal.maps d "extra_consonants"
      Alternates = Internal.mapl d "alternates"}