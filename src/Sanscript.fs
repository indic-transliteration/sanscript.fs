namespace FSharp.Indic.Sanscript

open System
open System.IO
open System.Reflection
open System.Text
open Tomlet

module Sanscript =

    let undefined<'T> : 'T = failwith "Not implemented yet"

    let assembly = Assembly.GetExecutingAssembly()

    let stream (m) = 
        assembly.GetManifestResourceStream(m)

    let decodeScheme s =
        try
            let p = TomlParser()
            let toml = p.Parse s
            Ok toml
        with
        | exn as ex -> Error ex.Message

    let tryDecodeScheme manifest = async {
        use r = new StreamReader(stream manifest, Encoding.UTF8)
        let toml = decodeScheme (r.ReadToEnd())
        match toml with
        | (Ok tbl) -> return Some tbl
        | (Error msg) -> 
            Console.WriteLine($"Unable to parse {manifest}: {msg}")
            return None 
    }

    let schemes = 
        assembly.GetManifestResourceNames()
        |> Array.filter (fun m -> m.StartsWith("sanscript"))
        |> Array.map tryDecodeScheme
        |> Async.Parallel
        |> Async.RunSynchronously 
        |> Array.choose id


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
    let t data fromlang tolang options = undefined