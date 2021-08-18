namespace Indic.Sanscript.Schemes

open System
open System.IO
open System.Reflection
open System.Text
open System.Collections.Generic

// Each of the language schemes have the following components
// Regardless of the format of the schemes themselves, we extract
// the components in this form for any language.
type Scheme =
  { Vowels: IDictionary<string, string>
    VowelMarks: IDictionary<string, string>
    Yogavaahas: IDictionary<string, string>
    Virama: IDictionary<string, string>
    Consonants: IDictionary<string, string>
    Symbols: IDictionary<string, string>
    Zwj: IDictionary<string, string>
    Skip: IDictionary<string, string>
    Accents: IDictionary<string, string>
    AccentedVowelAlternates: IDictionary<string, string array>
    Candra: IDictionary<string, string>
    Other: IDictionary<string, string>
    ExtraConsonants: IDictionary<string, string>
    Alternates: IDictionary<string, string array> }
module Schemes =

  // Placeholder for unimplemented functions
  let private undefined<'T> : 'T = failwith "Not implemented yet"

  // In which assembly are the scheme files embedded?
  // The purpose of this is to allow external modules (such as
  // tests) to set up an assembly from which the scheme manifests
  // can be loaded.
  let mutable srcasm: Assembly = null

  // Private module that deals with loading language scheme
  // files and decoding them
  module private Internal =
    //------------- Begin: Language Scheme-format-specific functions ----
    // These are the only configurations required when a language scheme
    // is changed between other formats JSON, TOML, etc.

    // Assembly in which language scheme files are embedded
    let assembly =
      if (isNull srcasm) then
        Assembly.GetAssembly(typeof<Toml.TomlType>.DeclaringType)
      else
        srcasm

    // Parse function, that takes in language scheme data and emits out a
    // format-specific form of the data. Eg: Toml.parse emits out a
    // TomlDocument. It does not matter what is the type of the emitted format,
    // (i.e., this emitted format is opaque, and simply is provided as an
    // input to the map function below.)
    let parse = Toml.parse

    // A function that takes in the emitted parsed output (above), and returns
    // back a standard F# dictionary for consumption in other modules.
    let map = Toml.map
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
    let data (m: string) =
      let s = assembly.GetManifestResourceStream(m)
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

    // Try decoding a scheme manifest file
    // m: Manifest file name
    // returns: Option<(string, DecodedObject)>
    //          Success: tuple (<language name>, <parsed scheme object>)
    //          Failure: None
    let tryDecodeScheme (m: string) = async {
      let scheme = data m |> decodeScheme
      match scheme with
      | (Ok doc) -> return Some (lang m, doc)
      | (Error msg) ->
        Console.WriteLine($"Unable to parse {m}: {msg}")
        return None
    }

    // List of manifests (language scheme files) that contain the language schemes
    let manifests =
      assembly.GetManifestResourceNames()
      |> Array.filter (fun m -> m.StartsWith("Indic.Sanscript.Schemes.Toml"))

    // Map of (<name of the language>, <TOML Document>)
    let schemes =
      manifests
      |> Array.map tryDecodeScheme
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
  /// <param name="lang">Name of the language (Eg: "devanagari")</param>
  ///
  /// <returns>Language Scheme</returns>
  ///
  let scheme (lang: string) =
    let d = Internal.schemes.[lang]
    { Vowels = undefined
      VowelMarks = undefined
      Yogavaahas = undefined
      Virama = undefined
      Consonants = undefined
      Symbols = undefined
      Zwj = undefined
      Skip = undefined
      Accents = undefined
      AccentedVowelAlternates = undefined
      Candra = undefined
      Other = undefined
      ExtraConsonants = undefined
      Alternates = undefined }