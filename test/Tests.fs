namespace FSharp.Indic.Sanscript.Tests

open System.IO
open System.Reflection
open System.Text
open Xunit
open Xunit.Abstractions
type TestCases(helper: ITestOutputHelper) =
    let tasm = Assembly.GetExecutingAssembly()
    let scasm = Assembly.Load("sanscript")
    let log s =
        helper.WriteLine(s)

    let testfname f =
        let resfolder = "testdata"
        let resnames = tasm.GetManifestResourceNames() 
                      |> Array.filter (fun m -> m.StartsWith("sanscript"))
        let len =  resnames.[0].LastIndexOf(resfolder) + resfolder.Length + 1
        let prefix = resnames.[0].Substring(0, len)
        prefix + f

    [<Fact>]
    let ``Decode a valid language scheme`` () =
        let m = testfname "valid.toml"
        Assert.True(Logic.isGoodScheme tasm m)

    [<Fact>]
    let ``Decode an invalid language scheme`` () =
        let m = testfname "invalid.toml"
        Assert.False(Logic.isGoodScheme tasm m)

    [<Fact>]
    let ``Decode all language schemes`` () =
        scasm.GetManifestResourceNames() 
        |> Array.filter(fun s -> s.StartsWith("sanscript"))
        |> Array.map (Logic.isGoodScheme scasm)
        |> Array.reduce (&&)
        |> Assert.True
