namespace Indic.Sanscript

open System
open System.Reflection
open Tomlet

module Sanscript =

    let undefined<'T> : 'T = failwith "Not implemented yet"

    // Assembly in which scheme files are embedded
    let assembly = Assembly.GetExecutingAssembly()

    // Inject Toml decoding function
    let tryTomlDecode =
        let tomlDecode =
            let p = TomlParser()
            p.Parse
        SansCore.tryDecodeScheme tomlDecode assembly 

    // Map of (<name of the language>, <TOML Document>)
    let private schemes =
        assembly.GetManifestResourceNames()
        |> Array.filter (fun m -> m.StartsWith("Indic.Sanscript"))
        |> Array.map tryTomlDecode
        |> Async.Parallel
        |> Async.RunSynchronously 
        |> Array.choose id
        |> Map.ofArray

    // // List of alternates for every vowel and consonant
    // let private alternates =
    //     let addCapitalAlternates t a =

    /// <summary>
    ///   The transliteration function.
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
    ///     let output = Sanscript.t("कर्मण्येवाधिकारस्ते मा फलेषु कदाचन । मा कर्मफलहेतुर्भूर्मा ते सङ्गोऽस्त्वकर्मणि ॥", "devanagari", "iast")
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
        // let scheme lng = schemes.[lng]
        // let table s lng = (scheme lng).GetSubTable
        // let charmap c lng = scheme >> 

        // let capitalise (s: string) =
        //     CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s)
        undefined