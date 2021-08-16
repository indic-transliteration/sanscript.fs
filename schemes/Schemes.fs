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

  let undefined<'T> : 'T = failwith "Not implemented yet"

  // Decode scheme data into a decoded object
  // This decoded object depends on the format of
  // the scheme (JSON, TOML, etc.)
  // f: Function to parse and decode a scheme into a decodable object
  // s: Scheme data from a scheme file as a string
  // returns: Result<DecodedObject, string>:
  //          Ok -> Decoded object if successful in parsing
  //          Error -> an error message.
  let private decodeScheme f (s: string) =
    try
      let scheme = f s
      Ok scheme
    with
    | ex -> Error ex.Message

  // Get data out of a scheme file
  // a: Assembly in which the scheme files are embedded
  // m: Manifest name of the scheme file (Eg: "sanscript.toml.brahmic.devanagari.toml")
  let private data (a: Assembly) (m: string) =
    let s = a.GetManifestResourceStream(m)
    use r = new StreamReader(s, Encoding.UTF8)
    r.ReadToEnd()

  // Get the name of the language from its scheme file name
  // m: Scheme file name (manifest name)
  let private lang (m: string) =
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
  // f: Function for decoding a scheme file
  // a: Assembly in which the scheme files are embedded
  // m: Manifest file name
  // returns: Option<(string, DecodedObject)>
  //          Success: tuple (<language name>, <parsed scheme object>)
  //          Failure: None
  let tryDecodeScheme f a (m: string) = async {
    let scheme = data a m |> decodeScheme f
    match scheme with
    | (Ok tbl) -> return Some (lang m, tbl)
    | (Error msg) ->
      Console.WriteLine($"Unable to parse {m}: {msg}")
      return None
  }

  // Get the language scheme files from a assembly
  // a: Assembly from which the languages schemes files are to be extracted
  // n: Namespace of the module in the assembly that contains the scheme files
  let schemeFiles (a: Assembly) (n: string) =
    a.GetManifestResourceNames()
    |> Array.filter (fun m -> m.StartsWith(n))

  // Map of (<name of the language>, <TOML Document>)
  // f: Function that extracts / parses the scheme out of a manifest
  // ms: Array of manifests that contain the language schemes
  let schemes f ms =
    ms
    |> Array.map f
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.choose id
    |> Map.ofArray