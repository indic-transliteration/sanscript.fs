namespace FSharp.Indic.Sanscript.Tests

open System.IO
open System.Reflection
open System.Text
open Xunit
open Xunit.Abstractions
type TestCases(helper: ITestOutputHelper) =
    let assembly = Assembly.GetExecutingAssembly()
    let scasm = Assembly.Load("sanscript")
    let log s =
        helper.WriteLine(s)

    let datamanifest (a: Assembly) m =
        let d = a.GetManifestResourceStream(m)
        use r = new StreamReader(d, Encoding.UTF8)
        r.ReadToEnd()

    let data s =
        let prefix = 
            let resfolder = "testdata"
            let resname = assembly.GetManifestResourceNames().[0] 
            let len =  resname.LastIndexOf(resfolder) + resfolder.Length + 1
            resname.Substring(0, len)

        let m = prefix + s
        let d = assembly.GetManifestResourceStream(m)
        use r = new StreamReader(d, Encoding.UTF8)
        r.ReadToEnd()

    [<Fact>]
    let ``Decode a valid language scheme`` () =
        let toml = data "valid.toml"
        Assert.True(Logic.isGoodScheme toml)

    [<Fact>]
    let ``Decode an invalid language scheme`` () =
        let toml = data "invalid.toml"
        Assert.False(Logic.isGoodScheme toml)

    [<Fact>]
    let ``Decode all language schemes`` () =
        scasm.GetManifestResourceNames() 
        |> Array.filter(fun s -> s.StartsWith("sanscript"))
        |> Array.map (datamanifest scasm >> Logic.isGoodScheme)
        |> Array.reduce (&&)
        |> Assert.True
