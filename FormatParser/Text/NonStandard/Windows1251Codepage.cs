using System.Collections.Immutable;
using System.Text;

namespace FormatParser.Text;

internal static class Windows1251Codepage
{
    public static ImmutableArray<char> ConversionMap = BuildMap().ToImmutableArray();
    
    private static char[] BuildMap()
    {
        var map = new char[256];

        for (byte i = 1; i < 128; i++)
            map[i] = (char) i;
        
        map[128]='Ђ';
        map[129]='Ѓ';
        map[130]='‚';
        map[131]='ѓ';
        map[132]='„';
        map[133]='…';
        map[134]='†';
        map[135]='‡';
        map[136]='€';
        map[137]='‰';
        map[138]='Љ';
        map[139]='‹';
        map[140]='Њ';
        map[141]='Ќ';
        map[142]='Ћ';
        map[143]='Џ';
        map[144]='ђ';
        map[145]='‘';
        map[146]='’';
        map[147]='“';
        map[148]='”';
        map[149]='•';
        map[150]='–';
        map[151]='—';
        // 152 is unused
        map[153]='™';
        map[154]='љ';
        map[155]='›';
        map[156]='њ';
        map[157]='ќ';
        map[158]='ћ';
        map[159]='џ';
        map[160]='\u00A0'; // nbsp
        map[161]='Ў';
        map[162]='ў';
        map[163]='Ј';
        map[164]='¤';
        map[165]='Ґ';
        map[166]='¦';
        map[167]='§';
        map[168]='Ё';
        map[169]='©';
        map[170]='Є';
        map[171]='«';
        map[172]='¬';
        map[173]='\u00AD';
        map[174]='®';
        map[175]='Ї';
        map[176]='°';
        map[177]='±';
        map[178]='І';
        map[179]='і';
        map[180]='ґ';
        map[181]='µ';
        map[182]='¶';
        map[183]='·';
        map[184]='ё';
        map[185]='№';
        map[186]='є';
        map[187]='»';
        map[188]='ј';
        map[189]='Ѕ';
        map[190]='ѕ';
        map[191]='ї';
        map[192]='А';
        map[193]='Б';
        map[194]='В';
        map[195]='Г';
        map[196]='Д';
        map[197]='Е';
        map[198]='Ж';
        map[199]='З';
        map[200]='И';
        map[201]='Й';
        map[202]='К';
        map[203]='Л';
        map[204]='М';
        map[205]='Н';
        map[206]='О';
        map[207]='П';
        map[208]='Р';
        map[209]='С';
        map[210]='Т';
        map[211]='У';
        map[212]='Ф';
        map[213]='Х';
        map[214]='Ц';
        map[215]='Ч';
        map[216]='Ш';
        map[217]='Щ';
        map[218]='Ъ';
        map[219]='Ы';
        map[220]='Ь';
        map[221]='Э';
        map[222]='Ю';
        map[223]='Я';
        map[224]='а';
        map[225]='б';
        map[226]='в';
        map[227]='г';
        map[228]='д';
        map[229]='е';
        map[230]='ж';
        map[231]='з';
        map[232]='и';
        map[233]='й';
        map[234]='к';
        map[235]='л';
        map[236]='м';
        map[237]='н';
        map[238]='о';
        map[239]='п';
        map[240]='р';
        map[241]='с';
        map[242]='т';
        map[243]='у';
        map[244]='ф';
        map[245]='х';
        map[246]='ц';
        map[247]='ч';
        map[248]='ш';
        map[249]='щ';
        map[250]='ъ';
        map[251]='ы';
        map[252]='ь';
        map[253]='э';
        map[254]='ю';
        map[255]='я';

        return map;
    }
}