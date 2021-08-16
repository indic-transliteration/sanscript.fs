namespace Indic.Sanscript

open System
open System.Reflection
open System.Globalization
open Indic.Sanscript.Schemes
open Indic.Sanscript.Schemes
open Tomlet.Models

module Sanscript =

  let private undefined<'T> : 'T = failwith "Not implemented yet"

  // Internal private module that deals with loading language scheme files,
  // decoding them and creating an array of language schemes that can be used
  // across various other functions.
  module private Internal =
    // Assembly in which language scheme files are embedded
    let assembly = Assembly.GetAssembly(typeof<Toml.TomlType>.DeclaringType)

    // List of manifests (language scheme files) that contain the language schemes
    let manifests =
      Schemes.schemeFiles assembly "Indic.Sanscript.Schemes.Toml"

    // Inject Toml decoding function - we'll use this function for
    // decoding all the language schemes from the manifest files.
    let tryTomlDecode =
      Schemes.tryDecodeScheme Toml.parse assembly

    // Decode language schemes files from the manifest files
    let schemes = Schemes.schemes tryTomlDecode manifests

  /// <summary>
  ///   The transliteration function. The only public function in this module.
  ///
  /// <para>
  ///     This function transliterates data from fromlang to tolang. The transliteration
  ///     can be controlled via the `options` parameter.
  /// </para>
  /// </summary>
  ///
  /// <param name="data">The input data</param>
  /// <param name="fromlang">From language (Eg: devanagari, tamil, etc.)</param>
  /// <param name="tolang">To language (Eg: IAST, etc.)</param>
  /// <param name="options">Options that control the transliteration</param>
  ///
  /// <returns>Transliterated data</returns>
  ///
  /// <example>
  ///   <code>
  ///     let output = Sanscript.t "कर्मण्येवाधिकारस्ते मा फलेषु कदाचन । मा कर्मफलहेतुर्भूर्मा ते सङ्गोऽस्त्वकर्मणि ॥" "devanagari" "iast"
  ///     Console.WriteLine(output)
  ///   </code>
  ///   will print...
  ///   <code>
  ///     karmaṇyēvādhikārastē mā phalēṣu kadāchana । mā karmaphalahēturbhūrmā tē saṅgōstvakarmaṇi ||
  ///   </code>
  /// </example>
  ///
  /// <category>Foo</category>
  let t data fromlang tolang options =

    let scheme lng = Internal.schemes.[lng]
    let table lng t = (scheme lng).GetSubTable t
    let keys (t: TomlTable) = t.Keys

    let fromScheme = scheme fromlang
    let toScheme = scheme tolang

    let capitalise (s: string) =
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s)
    ""