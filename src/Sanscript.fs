namespace Indic.Sanscript

open System
open System.Globalization
open System.Reflection
open Indic.Sanscript.Schemes

module Sanscript =

  module private Internal =
    // Placeholder for unimplemented functions
    let undefined<'T> : 'T = failwith "Not implemented yet"

    // Create a map of all schemes
    let schememap = Schemes.schememap (Assembly.GetAssembly(typeof<Toml.TomlType>.DeclaringType))

    // Given a language, get its scheme
    let scheme (lang: string) = schememap.[lang]

    // For easily navigating inside an Option
    let (>>=) m f = Option.bind f m

    // Capitalise the given string (Eg: given "au", return back "Au")
    let capitalise (s: string) =
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s)

    // For every character in the codelist add alternative characters of that value
    // in the alternates map of the langscheme - these additional alternates are capitalised form of
    // the existing alternates plus the original value also being added in its capitalised form.
    let addCapitalAlternates (langscheme: Scheme) (codelist: string list) =
      let alts = langscheme.Alternates

      // Get all the alternates for a single character
      let altc ch =
        let alt = alts
                  // Pull out the list of alternates for the character
                  // If the said character does not have any alternates, just return None
                  >>= fun m -> if (m.ContainsKey ch) then Some m.[ch] else None
                  // Capitalise each of the given alternatives, and add both
                  // non-capitalised and capitalised as alternatives of a given character
                  >>= fun v -> Some (List.collect (fun (c: string) -> [c; capitalise c]) v)

        // Finally, add the capitalised version of the original character as an alternative
        // Note that this must be added regardless of whether the character had an alternate
        // list originally - this makes sure altc always returns a list of alternates.
        if alt.IsNone then [capitalise ch] else (List.concat [[capitalise ch];alt.Value])

      // Now, for each value in the codelist, create a new map of all the alternates
      // value with the new alternates that include the capital letters.
      let addtomap (m: Map<string, string list>) (c: string) =
        let getmap _ = (Some (altc c))
        m |> Map.change c getmap
      codelist |> List.fold addtomap alts.Value

    // Join all the alternates to create one single map of alternates.
    let join (m1: Map<string,string list>) (m2: Map<string,string list>) =
      Map.fold (fun (m: Map<string,string list>) k v -> Map.add k v m) m1 m2

    // Add captial alternates to the following schemes
    let iastsch = scheme "iast"
    let kolkatav2sch = scheme "kolkata_v2"
    let isosch = scheme "iso"

    let iastalts =
      [ addCapitalAlternates iastsch iastsch.VowelsList
        addCapitalAlternates iastsch iastsch.ConsonantList
        addCapitalAlternates iastsch iastsch.ExtraConsonantList
        addCapitalAlternates iastsch ["oṃ"] ] |> List.fold join Map.empty

    let kolkataV2alts =
      [ addCapitalAlternates kolkatav2sch kolkatav2sch.VowelsList
        addCapitalAlternates kolkatav2sch kolkatav2sch.ConsonantList
        addCapitalAlternates kolkatav2sch kolkatav2sch.ExtraConsonantList ] |> List.fold join Map.empty

    let isoalts =
      [ addCapitalAlternates isosch isosch.VowelsList
        addCapitalAlternates isosch isosch.ConsonantList
        addCapitalAlternates isosch isosch.ExtraConsonantList
        addCapitalAlternates isosch ["oṃ"] ] |> List.fold join Map.empty

    iastsch.Alternates <- Some iastalts
    kolkatav2sch.Alternates <- Some kolkataV2alts
    isosch.Alternates <- Some isoalts
    let a = 8

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
    let f = Internal.scheme fromlang
    let t = Internal.scheme tolang

    let m = [("a", [1;1;1]); ("b", [2;2;2]); ("c", [3;3;3])] |> Map.ofList

    ""