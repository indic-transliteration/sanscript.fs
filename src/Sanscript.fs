namespace FSharp.Indic.Sanscript

open System
open System.IO
open System.Reflection
open System.Text
open Tomlet

module Sanscript =

    let undefined<'T> : 'T = failwith "Not implemented yet"

    // Map of (<name of the language>, <TOML Document>)
    let private schemes =
        let assembly = Assembly.GetExecutingAssembly()

        // Decode toml data into a Tomlet Document
        // s: TOML data from a toml file as a string
        // returns: Result<TomlDocument, string>: 
        //          Ok -> TOML document if successful in parsing
        //          Error -> an error message.
        let decodeScheme (s: string) =
            try
                let p = TomlParser()
                let toml = p.Parse s
                Ok toml
            with
            | exn as ex -> Error ex.Message

        // Get data out of a toml file
        // m: Manifest name of the toml file (Eg: "sanscript.toml.brahmic.devanagari.toml")
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
            m.Substring(filei,exti)

        // Try decoding a scheme manifest file
        // m: Manifest file name
        // returns: Option<(string, TomlDocument)>
        //          Success: tuple (<language name>, <parsed TOML document>)
        //          Failure: None
        let tryDecodeScheme (m: string) = async {
            let toml = decodeScheme (data m)
            match toml with
            | (Ok tbl) -> return Some (lang m, tbl)
            | (Error msg) -> 
                Console.WriteLine($"Unable to parse {m}: {msg}")
                return None 
        }

        assembly.GetManifestResourceNames()
        |> Array.filter (fun m -> m.StartsWith("sanscript"))
        |> Array.map tryDecodeScheme
        |> Async.Parallel
        |> Async.RunSynchronously 
        |> Array.choose id
        |> Map.ofArray

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
        let scheme lang = schemes.[lang]

        undefined