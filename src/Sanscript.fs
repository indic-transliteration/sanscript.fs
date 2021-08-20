namespace Indic.Sanscript

open System
open System.Globalization
open System.Reflection
open Indic.Sanscript.Schemes

module Sanscript =

  // Placeholder for unimplemented functions
  let private undefined<'T> : 'T = failwith "Not implemented yet"

  // In which assembly are schemes present?
  let private scheme = Schemes.scheme (Assembly.GetAssembly(typeof<Toml.TomlType>.DeclaringType))

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
    let f = scheme fromlang
    let t = scheme tolang

    let capitalise (s: string) =
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s)
    ""