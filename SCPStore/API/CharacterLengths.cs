﻿namespace RueI.Parsing;

using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Linq;

/// <summary>
/// Provides lengths for characters in hints.
/// </summary>
/// <remarks>This class is mosty designed for internal use within RueI. However, it can still be useful for external use.</remarks>
internal static class CharacterLengths
{
    /// <summary>
    /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> of character sizes.
    /// </summary>
    public static IReadOnlyDictionary<char, float> Lengths { get; } = GetDictionary();

    private static IReadOnlyDictionary<char, float> GetDictionary()
    {
#pragma warning disable SA1123 // Do not place regions within elements
        Dictionary<char, float> charSizes = new()
#region character sizes
        {
            { '', 0f },
            { ' ', 8.437592455621301f },
            { '!', 7.828032544378698f },
            { '"', 9.963096338757397f },
            { '#', 20.17964127218935f },
            { '$', 19.230011094674555f },
            { '%', 25.635201964497043f },
            { '&', 21.349033325443788f },
            { '\'', 5.8966900887573965f },
            { '(', 11.063512389053255f },
            { ')', 11.300919933431953f },
            { '*', 14.724080579881656f },
            { '+', 19.586122411242606f },
            { ',', 6.642598927514793f },
            { '-', 9.929410133136095f },
            { '.', 8.285202477810651f },
            { '/', 13.774450402366863f },
            { '0', 19.230011094674555f },
            { '1', 19.230011094674555f },
            { '2', 19.230011094674555f },
            { '3', 19.230011094674555f },
            { '4', 19.230011094674555f },
            { '5', 19.230011094674555f },
            { '6', 19.230011094674555f },
            { '7', 19.230011094674555f },
            { '8', 19.230011094674555f },
            { '9', 19.230011094674555f },
            { ':', 7.285845044378698f },
            { ';', 6.759698594674556f },
            { '<', 17.739798035502957f },
            { '=', 19.196325402366863f },
            { '>', 17.977205579881655f },
            { '?', 15.75712421893491f },
            { '@', 31.6842825443787f },
            { 'A', 21.66985433136095f },
            { 'B', 21.264016272189348f },
            { 'C', 22.534467455621304f },
            { 'D', 22.720543639053254f },
            { 'E', 19.738511875739647f },
            { 'F', 19.51875f },
            { 'G', 23.721505177514793f },
            { 'H', 24.550826964497045f },
            { 'I', 9.234832655325445f },
            { 'J', 19.09526627218935f },
            { 'K', 21.891221852071006f },
            { 'L', 18.28198450887574f },
            { 'M', 30.02403332544379f },
            { 'N', 24.635845044378698f },
            { 'O', 23.484097633136095f },
            { 'P', 21.365075402366863f },
            { 'Q', 23.484097633136095f },
            { 'R', 22.025966674556212f },
            { 'S', 20.551793639053255f },
            { 'T', 20.72182877218935f },
            { 'U', 22.805560692307694f },
            { 'V', 21.39876109467456f },
            { 'W', 31.10840832544379f },
            { 'X', 21.230330579881656f },
            { 'Y', 20.773160133136095f },
            { 'Z', 20.75551549112426f },
            { '[', 8.318888683431952f },
            { '\\', 13.673391272189349f },
            { ']', 8.318888683431952f },
            { '^', 14.435341674556213f },
            { '_', 14.977528147928997f },
            { '`', 9.911764977810652f },
            { 'a', 18.58676549112426f },
            { 'b', 19.230011094674555f },
            { 'c', 17.874542857988164f },
            { 'd', 19.281342455621303f },
            { 'e', 17.92587421893491f },
            { 'f', 11.486996116863905f },
            { 'g', 19.247656763313607f },
            { 'h', 19.043934911242605f },
            { 'i', 7.776701183431953f },
            { 'j', 7.913050110946746f },
            { 'k', 16.993888683431955f },
            { 'l', 7.776701183431953f },
            { 'm', 30.752298035502957f },
            { 'n', 19.061580579881657f },
            { 'o', 19.43373294674556f },
            { 'p', 19.230011094674555f },
            { 'q', 19.348714866863904f },
            { 'r', 11.674676405325444f },
            { 's', 17.569761875739648f },
            { 't', 11.148529955621303f },
            { 'u', 19.061580579881657f },
            { 'v', 16.689108727810652f },
            { 'w', 26.161348414201186f },
            { 'x', 16.875184911242602f },
            { 'y', 16.485386875739646f },
            { 'z', 16.875184911242602f },
            { '{', 11.453309911242604f },
            { '|', 7.6579974112426035f },
            { '}', 11.453309911242604f },
            { '~', 23.75519086982249f },
            { '¡', 7.6579974112426035f },
            { '¢', 18.891544420118343f },
            { '£', 19.823529955621304f },
            { '¤', 25.09301549112426f },
            { '¥', 17.892187500000002f },
            { '¦', 7.523252588757397f },
            { '§', 20.992923035502958f },
            { '¨', 14.927801405325445f },
            { '©', 27.736580579881657f },
            { 'ª', 15.351285133136095f },
            { '«', 15.808455579881656f },
            { '¬', 18.925231139053256f },
            { '®', 27.821597633136097f },
            { '¯', 14.809097633136094f },
            { '°', 13.113558616863905f },
            { '±', 18.383043639053255f },
            { '²', 12.537684911242604f },
            { '³', 12.537684911242604f },
            { '´', 9.759375f },
            { 'µ', 19.315028147928995f },
            { '¶', 16.485386875739646f },
            { '·', 8.52261002218935f },
            { '¸', 8.437592455621301f },
            { '¹', 12.537684911242604f },
            { 'º', 15.622379396449704f },
            { '»', 15.723437500000001f },
            { '¼', 25.144346852071006f },
            { '½', 26.228721852071004f },
            { '¾', 26.939339866863907f },
            { '¿', 15.927158325443788f },
            { 'Æ', 31.59926549112426f },
            { '×', 18.16328176331361f },
            { 'Ø', 23.484097633136095f },
            { 'Þ', 20.569439307692306f },
            { 'ß', 20.33203176331361f },
            { 'æ', 29.345497411242604f },
            { 'ð', 20.17964127218935f },
            { '÷', 19.722471852071006f },
            { 'ø', 19.315028147928995f },
            { 'þ', 19.48506430769231f },
            { 'đ', 19.857216674556213f },
            { 'Ħ', 23.907581360946747f },
            { 'ı', 7.726973927514793f },
            { 'ĸ', 18.82417303550296f },
            { 'Ł', 18.33331586982249f },
            { 'ł', 8.963738905325444f },
            { 'Ŋ', 24.297379396449703f },
            { 'ŋ', 19.315028147928995f },
            { 'Œ', 32.41254622781065f },
            { 'œ', 32.056434911242604f },
            { 'ſ', 7.879363905325444f },
            { 'Ə', 24.27973372781065f },
            { 'ƒ', 11.352251294378698f },
            { 'Ơ', 24.19471667455621f },
            { 'ơ', 19.417690869822486f },
            { 'Ư', 23.6188424556213f },
            { 'ư', 20.33203176331361f },
            { 'ȷ', 7.929091161242604f },
            { 'ə', 17.874542857988164f },
            { 'ˆ', 14.910156763313609f },
            { 'ˇ', 14.164247411242604f },
            { 'ˉ', 14.825138683431954f },
            { '˘', 14.656707142011836f },
            { '˙', 7.776701183431953f },
            { '˚', 10.758732433431954f },
            { '˛', 8.675f },
            { '˜', 15.09623294674556f },
            { '˝', 13.097517566568047f },
            { '˳', 10.538970044378699f },
            { '̀', 0f },
            { '́', 0f },
            { '̃', 0f },
            { '̉', 0f },
            { '̏', 0f },
            { '̣', 0f },
            { '΄', 8.79370377218935f },
            { '΅', 16.621736316568047f },
            { '·', 8.52261002218935f },
            { 'Γ', 19.146597633136096f },
            { 'Δ', 24.178674597633137f },
            { 'Θ', 23.34774921893491f },
            { 'Λ', 22.483136094674556f },
            { 'Ξ', 19.400046227810652f },
            { 'Π', 24.550826964497045f },
            { 'Σ', 19.400046227810652f },
            { 'Φ', 24.805880177514794f },
            { 'Ψ', 24.093657544378697f },
            { 'Ω', 22.720543639053254f },
            { 'α', 19.536395668639052f },
            { 'β', 20.551793639053255f },
            { 'γ', 16.977847633136093f },
            { 'δ', 19.315028147928995f },
            { 'ε', 18.85785872781065f },
            { 'ζ', 17.061261094674556f },
            { 'η', 19.315028147928995f },
            { 'θ', 19.874861316568047f },
            { 'ι', 10.402621116863905f },
            { 'λ', 19.09526627218935f },
            { 'ξ', 17.027575402366864f },
            { 'π', 20.654456360946746f },
            { 'ρ', 19.315028147928995f },
            { 'ς', 18.553078772189348f },
            { 'σ', 19.281342455621303f },
            { 'τ', 17.739798035502957f },
            { 'υ', 18.553078772189348f },
            { 'φ', 23.788877588757398f },
            { 'ψ', 23.721505177514793f },
            { 'ω', 28.601192677514792f },
            { 'ϑ', 19.908548035502957f },
            { 'ϒ', 18.604410133136096f },
            { 'ϖ', 27.279410133136096f },
            { ' ', 8.437592455621301f },
            { '­', 9.929410133136095f },
            { 'Đ', 23.229044420118345f },
            { 'Ð', 23.229044420118345f },
            { 'ħ', 19.552435692307693f },
            { 'Ŧ', 20.72182877218935f },
            { 'ŧ', 11.148529955621303f },
            { 'À', 21.66985433136095f },
            { 'Á', 21.66985433136095f },
            { 'Â', 21.66985433136095f },
            { 'Ã', 21.66985433136095f },
            { 'Ä', 21.66985433136095f },
            { 'Å', 21.66985433136095f },
            { 'Ǻ', 21.66985433136095f },
            { 'Ç', 22.534467455621304f },
            { 'È', 19.738511875739647f },
            { 'É', 19.738511875739647f },
            { 'Ê', 19.738511875739647f },
            { 'Ë', 19.738511875739647f },
            { 'Ì', 9.234832655325445f },
            { 'Í', 9.234832655325445f },
            { 'Î', 9.234832655325445f },
            { 'Ï', 9.234832655325445f },
            { 'Ñ', 24.635845044378698f },
            { 'Ò', 23.484097633136095f },
            { 'Ó', 23.484097633136095f },
            { 'Ô', 23.484097633136095f },
            { 'Õ', 23.484097633136095f },
            { 'Ö', 23.484097633136095f },
            { 'Ù', 22.805560692307694f },
            { 'Ú', 22.805560692307694f },
            { 'Û', 22.805560692307694f },
            { 'Ü', 22.805560692307694f },
            { 'Ý', 20.773160133136095f },
            { 'à', 18.58676549112426f },
            { 'á', 18.58676549112426f },
            { 'â', 18.58676549112426f },
            { 'ã', 18.58676549112426f },
            { 'ä', 18.58676549112426f },
            { 'å', 18.58676549112426f },
            { 'ǻ', 18.58676549112426f },
            { 'ç', 17.874542857988164f },
            { 'è', 17.92587421893491f },
            { 'é', 17.92587421893491f },
            { 'ê', 17.92587421893491f },
            { 'ë', 17.92587421893491f },
            { 'ì', 7.726973927514793f },
            { 'í', 7.726973927514793f },
            { 'î', 7.726973927514793f },
            { 'ï', 7.726973927514793f },
            { 'ñ', 19.061580579881657f },
            { 'ò', 19.43373294674556f },
            { 'ó', 19.43373294674556f },
            { 'ô', 19.43373294674556f },
            { 'õ', 19.43373294674556f },
            { 'ö', 19.43373294674556f },
            { 'ù', 19.061580579881657f },
            { 'ú', 19.061580579881657f },
            { 'û', 19.061580579881657f },
            { 'ü', 19.061580579881657f },
            { 'ý', 16.485386875739646f },
            { 'ÿ', 16.485386875739646f },
            { 'Ā', 21.66985433136095f },
            { 'ā', 18.58676549112426f },
            { 'Ă', 21.66985433136095f },
            { 'ă', 18.58676549112426f },
            { 'Ą', 21.66985433136095f },
            { 'ą', 18.58676549112426f },
            { 'Ć', 22.534467455621304f },
            { 'ć', 17.874542857988164f },
            { 'Ĉ', 22.534467455621304f },
            { 'ĉ', 17.874542857988164f },
            { 'Ċ', 22.534467455621304f },
            { 'ċ', 17.874542857988164f },
            { 'Č', 22.534467455621304f },
            { 'č', 17.874542857988164f },
            { 'Ď', 22.720543639053254f },
            { 'ď', 21.823848414201183f },
            { 'Ē', 19.738511875739647f },
            { 'ē', 17.92587421893491f },
            { 'Ĕ', 19.738511875739647f },
            { 'ĕ', 17.92587421893491f },
            { 'Ė', 19.738511875739647f },
            { 'ė', 17.92587421893491f },
            { 'Ę', 19.738511875739647f },
            { 'ę', 17.92587421893491f },
            { 'Ě', 19.738511875739647f },
            { 'ě', 17.92587421893491f },
            { 'Ĝ', 23.721505177514793f },
            { 'ĝ', 19.247656763313607f },
            { 'Ğ', 23.721505177514793f },
            { 'ğ', 19.247656763313607f },
            { 'Ġ', 23.721505177514793f },
            { 'ġ', 19.247656763313607f },
            { 'Ģ', 23.721505177514793f },
            { 'ģ', 19.247656763313607f },
            { 'Ĥ', 24.550826964497045f },
            { 'ĥ', 19.043934911242605f },
            { 'Ĩ', 9.234832655325445f },
            { 'ĩ', 7.726973927514793f },
            { 'Ī', 9.234832655325445f },
            { 'ī', 7.726973927514793f },
            { 'Ĭ', 9.234832655325445f },
            { 'ĭ', 7.726973927514793f },
            { 'Į', 9.234832655325445f },
            { 'į', 7.776701183431953f },
            { 'İ', 9.234832655325445f },
            { 'Ĳ', 28.330098414201185f },
            { 'ĳ', 15.68975078106509f },
            { 'Ĵ', 19.09526627218935f },
            { 'ĵ', 7.929091161242604f },
            { 'Ķ', 21.891221852071006f },
            { 'ķ', 16.993888683431955f },
            { 'Ĺ', 18.28198450887574f },
            { 'ĺ', 7.776701183431953f },
            { 'Ļ', 18.28198450887574f },
            { 'ļ', 7.776701183431953f },
            { 'Ľ', 18.28198450887574f },
            { 'ľ', 10.319207655325444f },
            { 'Ŀ', 18.28198450887574f },
            { 'ŀ', 11.50464127218935f },
            { 'Ń', 24.635845044378698f },
            { 'ń', 19.061580579881657f },
            { 'Ņ', 24.635845044378698f },
            { 'ņ', 19.061580579881657f },
            { 'Ň', 24.635845044378698f },
            { 'ň', 19.061580579881657f },
            { 'ŉ', 19.061580579881657f },
            { 'Ō', 23.484097633136095f },
            { 'ō', 19.43373294674556f },
            { 'Ŏ', 23.484097633136095f },
            { 'ŏ', 19.43373294674556f },
            { 'Ő', 23.484097633136095f },
            { 'ő', 19.43373294674556f },
            { 'Ŕ', 22.025966674556212f },
            { 'ŕ', 11.674676405325444f },
            { 'Ŗ', 22.025966674556212f },
            { 'ŗ', 11.674676405325444f },
            { 'Ř', 22.025966674556212f },
            { 'ř', 11.674676405325444f },
            { 'Ś', 20.551793639053255f },
            { 'ś', 17.569761875739648f },
            { 'Ŝ', 20.551793639053255f },
            { 'ŝ', 17.569761875739648f },
            { 'Ş', 20.551793639053255f },
            { 'ş', 17.569761875739648f },
            { 'Ș', 20.551793639053255f },
            { 'ș', 17.569761875739648f },
            { 'Š', 20.551793639053255f },
            { 'š', 17.569761875739648f },
            { 'Ț', 20.72182877218935f },
            { 'ț', 11.148529955621303f },
            { 'Ţ', 20.72182877218935f },
            { 'ţ', 11.148529955621303f },
            { 'Ť', 20.72182877218935f },
            { 'ť', 11.827066383136096f },
            { 'Ũ', 22.805560692307694f },
            { 'ũ', 19.061580579881657f },
            { 'Ū', 22.805560692307694f },
            { 'ū', 19.061580579881657f },
            { 'Ŭ', 22.805560692307694f },
            { 'ŭ', 19.061580579881657f },
            { 'Ů', 22.805560692307694f },
            { 'ů', 19.061580579881657f },
            { 'Ű', 22.805560692307694f },
            { 'ű', 19.061580579881657f },
            { 'Ų', 22.805560692307694f },
            { 'ų', 19.061580579881657f },
            { 'Ŵ', 31.10840832544379f },
            { 'ŵ', 26.161348414201186f },
            { 'Ŷ', 20.773160133136095f },
            { 'ŷ', 16.485386875739646f },
            { 'Ÿ', 20.773160133136095f },
            { 'Ź', 20.75551549112426f },
            { 'ź', 16.875184911242602f },
            { 'Ż', 20.75551549112426f },
            { 'ż', 16.875184911242602f },
            { 'Ž', 20.75551549112426f },
            { 'ž', 16.875184911242602f },
            { 'Ǽ', 31.59926549112426f },
            { 'ǽ', 29.345497411242604f },
            { 'Ǿ', 23.484097633136095f },
            { 'ǿ', 19.315028147928995f },
            { 'Ά', 21.66985433136095f },
            { 'Έ', 19.738511875739647f },
            { 'Ή', 24.550826964497045f },
            { 'Ί', 9.234832655325445f },
            { 'Ό', 23.822564307692307f },
            { 'Ύ', 22.4670950443787f },
            { 'Ώ', 23.060613905325443f },
            { 'ΐ', 10.402621116863905f },
            { 'Α', 21.66985433136095f },
            { 'Β', 21.264016272189348f },
            { 'Ε', 19.738511875739647f },
            { 'Ζ', 20.75551549112426f },
            { 'Η', 24.550826964497045f },
            { 'Ι', 9.234832655325445f },
            { 'Κ', 21.891221852071006f },
            { 'Μ', 30.02403332544379f },
            { 'Ν', 24.635845044378698f },
            { 'Ο', 23.484097633136095f },
            { 'Ρ', 21.365075402366863f },
            { 'Τ', 20.72182877218935f },
            { 'Υ', 20.773160133136095f },
            { 'Χ', 21.230330579881656f },
            { 'Ϊ', 9.234832655325445f },
            { 'Ϋ', 20.773160133136095f },
            { 'ά', 19.536395668639052f },
            { 'έ', 18.85785872781065f },
            { 'ή', 19.315028147928995f },
            { 'ί', 10.402621116863905f },
            { 'ΰ', 18.553078772189348f },
            { 'κ', 18.82417303550296f },
            { 'ο', 19.43373294674556f },
            { 'μ', 19.315028147928995f },
            { 'ν', 16.689108727810652f },
            { 'χ', 16.875184911242602f },
            { 'ϊ', 10.402621116863905f },
            { 'ϋ', 18.553078772189348f },
            { 'ό', 19.43373294674556f },
            { 'ύ', 18.553078772189348f },
            { 'ώ', 28.601192677514792f },
            { 'ǰ', 7.929091161242604f },
            { 'ʼ', 6.167783838757397f },
        };
        #endregion
#pragma warning restore SA1123 // Do not place regions within elements
        try
        {
            return LoadFrozenDictionary(charSizes);
        }
        catch (Exception)
        {
            return new ReadOnlyDictionary<char, float>(charSizes);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static IReadOnlyDictionary<char, float> LoadFrozenDictionary(Dictionary<char, float> sizes) => sizes;

    public static float StringSize(string str) => str.Sum(x => Lengths[x]);
}