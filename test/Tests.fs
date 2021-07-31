namespace FSharp.Indic.Sanscript.Tests
open System
open Xunit

module TestCases =
    [<Fact>]
    let ``Decode a valid language scheme`` () =
        let toml = """
[vowels]
"अ" = "अ"
"आ" = "आ"

[vowel_marks]
"ा" = "ा"
"ि" = "ि"

[yogavaahas]
"ं" = "ं"
"ः" = "ः"

[virama]
"्" = "्"

[consonants]
"क" = "क"
"ख" = "ख"

[symbols]
"०" = "०"
"१" = "१"

[zwj]
"\u200d" = "\u200d"

[zwnj]
"\u200c" = "\u200c"

[skip]
"" = ""

[accents]
"॑" = "॑"
"॒" = "॒"

[candra]
"ॅ" = "ॅ"

[extra_consonants]
"क़" = "क़"
"ख़" = "ख़"

[alternates]
"क़" = [ "क़",]
"ख़" = [ "ख़",]
"""
        Assert.True(Logic.isGoodScheme toml)

    [<Fact>]
    let ``Decode an invalid language scheme`` () =
        let toml = """
[vowels]
"अ" = "अ"
"आ" = "आ

[vowel_marks]
"ा" = "ा"
"ि" = "ि"

[yogavaahas]
"ं" = "ं"
"ः" = "ः"

[virama]
"्" = "्"

[consonants]
"क" = "क"
"ख" = "ख"

[symbols]
"०" = "०"
"१" = "१"

[zwj]
"\u200d" = "\u200d"

[zwnj]
"\u200c" = "\u200c"

[skip]
"" = ""

[accents]
"॑" = "॑"
"॒" = "॒"

[candra]
"ॅ" = "ॅ"

[extra_consonants]
"क़" = "क़"
"ख़" = "ख़"

[alternates]
"क़" = [ "क़",]
"ख़" = [ "ख़",]
"""
        Assert.False(Logic.isGoodScheme toml)        
