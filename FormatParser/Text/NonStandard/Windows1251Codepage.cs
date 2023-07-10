using System.Collections.Immutable;

namespace FormatParser.Text;

internal static class Windows1251Codepage
{
    public static IReadOnlyDictionary<byte, char> ConversionDictionary = BuildDictionary();
    
    private static Dictionary<byte, char> BuildDictionary()
    {
        var dict = new Dictionary<byte, char>();

        for (byte i = 1; i < 128; i++)
            dict[i] = (char) i;
        
        dict[128]='Ђ';
        dict[129]='Ѓ';
        dict[130]='‚';
        dict[131]='ѓ';
        dict[132]='„';
        dict[133]='…';
        dict[134]='†';
        dict[135]='‡';
        dict[136]='€';
        dict[137]='‰';
        dict[138]='Љ';
        dict[139]='‹';
        dict[140]='Њ';
        dict[141]='Ќ';
        dict[142]='Ћ';
        dict[143]='Џ';
        dict[144]='ђ';
        dict[145]='‘';
        dict[146]='’';
        dict[147]='“';
        dict[148]='”';
        dict[149]='•';
        dict[150]='–';
        dict[151]='—';
        // 152 unused
        dict[153]='™';
        dict[154]='љ';
        dict[155]='›';
        dict[156]='њ';
        dict[157]='ќ';
        dict[158]='ћ';
        dict[159]='џ';
        dict[160]='\u00A0'; // nbsp
        dict[161]='Ў';
        dict[162]='ў';
        dict[163]='Ј';
        dict[164]='¤';
        dict[165]='Ґ';
        dict[166]='¦';
        dict[167]='§';
        dict[168]='Ё';
        dict[169]='©';
        dict[170]='Є';
        dict[171]='«';
        dict[172]='¬';
        dict[173]='\u00AD';
        dict[174]='®';
        dict[175]='Ї';
        dict[176]='°';
        dict[177]='±';
        dict[178]='І';
        dict[179]='і';
        dict[180]='ґ';
        dict[181]='µ';
        dict[182]='¶';
        dict[183]='·';
        dict[184]='ё';
        dict[185]='№';
        dict[186]='є';
        dict[187]='»';
        dict[188]='ј';
        dict[189]='Ѕ';
        dict[190]='ѕ';
        dict[191]='ї';
        dict[192]='А';
        dict[193]='Б';
        dict[194]='В';
        dict[195]='Г';
        dict[196]='Д';
        dict[197]='Е';
        dict[198]='Ж';
        dict[199]='З';
        dict[200]='И';
        dict[201]='Й';
        dict[202]='К';
        dict[203]='Л';
        dict[204]='М';
        dict[205]='Н';
        dict[206]='О';
        dict[207]='П';
        dict[208]='Р';
        dict[209]='С';
        dict[210]='Т';
        dict[211]='У';
        dict[212]='Ф';
        dict[213]='Х';
        dict[214]='Ц';
        dict[215]='Ч';
        dict[216]='Ш';
        dict[217]='Щ';
        dict[218]='Ъ';
        dict[219]='Ы';
        dict[220]='Ь';
        dict[221]='Э';
        dict[222]='Ю';
        dict[223]='Я';
        dict[224]='а';
        dict[225]='б';
        dict[226]='в';
        dict[227]='г';
        dict[228]='д';
        dict[229]='е';
        dict[230]='ж';
        dict[231]='з';
        dict[232]='и';
        dict[233]='й';
        dict[234]='к';
        dict[235]='л';
        dict[236]='м';
        dict[237]='н';
        dict[238]='о';
        dict[239]='п';
        dict[240]='р';
        dict[241]='с';
        dict[242]='т';
        dict[243]='у';
        dict[244]='ф';
        dict[245]='х';
        dict[246]='ц';
        dict[247]='ч';
        dict[248]='ш';
        dict[249]='щ';
        dict[250]='ъ';
        dict[251]='ы';
        dict[252]='ь';
        dict[253]='э';
        dict[254]='ю';
        dict[255]='я';

        return dict;
    }
}