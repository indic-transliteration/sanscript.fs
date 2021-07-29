namespace FSharp.Indic

open System
open System.IO
open System.Reflection
open System.Collections.Generic
open System.Text
open System.Text.Json
open System.Text.Json.Serialization

type Scheme = 
  { [<JsonPropertyName("vowels")>] Vowels: string array
    [<JsonPropertyName("vowel_marks")>] VowelMarks: string array
    [<JsonPropertyName("yogavaahas")>] Yogavaahas: string array
    [<JsonPropertyName("virama")>] Virama: string array
    [<JsonPropertyName("consonants")>] Consonants: string array
    [<JsonPropertyName("symbols")>] Symbols: string array
    [<JsonPropertyName("zwj")>] Zwj: string array
    [<JsonPropertyName("skip")>] Skip: string array
    [<JsonPropertyName("accents")>] Accents: string array 
    [<JsonPropertyName("accented_vowel_alternates")>] AccentedVowelAlternates: IDictionary<string, string array>
    [<JsonPropertyName("candra")>] Candra: string array
    [<JsonPropertyName("other")>] Other: string array
    [<JsonPropertyName("extra_consonants")>] ExtraConsonants: string array
    [<JsonPropertyName("alternates")>] Alternates: IDictionary<string, string array> }

module Sanscript =
    let t data fromlang tolang options =
        let assembly = Assembly.GetExecutingAssembly()

        let tryDecodeScheme n = async {
            let s = assembly.GetManifestResourceStream(n)
            use r = new StreamReader(s, Encoding.UTF8)
            let! jstring = (r.ReadToEndAsync() |> Async.AwaitTask)

            try
                let res = JsonSerializer.Deserialize<Scheme>(jstring)
                return (Some res)
            with
            | exn as ex -> 
                Console.WriteLine($"Unable to decode {n}: {ex.Message}")
                return None
        }

        let schemes = 
            assembly.GetManifestResourceNames()
            |> Array.filter (fun n -> n.StartsWith("stothramala"))
            |> Array.map tryDecodeScheme
            |> Async.Parallel
            |> Async.RunSynchronously 
            |> Array.choose id

        printfn "%A" schemes
