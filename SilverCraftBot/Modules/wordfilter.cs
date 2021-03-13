using System.Globalization;

namespace dotnetcorebot.Modules
{
    public class ProfanityFilter
    {
        // METHOD: containsProfanity
        public static bool ContainsProfanity(string checkStr)
        {
            bool badwordpresent = false;

            string[] inStrArray = checkStr.Split(new[] { ' ' });

            string[] words = ProfanityArray();

            // LOOP THROUGH WORDS IN MESSAGE
            for (int x = 0; x < inStrArray.Length; x++)
            {
                // LOOP THROUGH PROFANITY WORDS
                for (int i = 0; i < words.Length; i++)
                {
                    // IF WORD IS PROFANITY, SET FLAG AND BREAK OUT OF LOOP
                    //if (inStrArray[x].toString().toLowerCase().equals(words[i]))
                    if (inStrArray[x].ToLower(CultureInfo.InvariantCulture) == words[i].ToLower(CultureInfo.InvariantCulture))
                    {
                        badwordpresent = true;
                        break;
                    }
                }
                // IF FLAG IS SET, BREAK OUT OF OUTER LOOP
                if (badwordpresent == true)
                {
                    break;
                }
            }

            return badwordpresent;
        }

        // ************************************************************************

        // ************************************************************************
        // METHOD: profanityArray()
        // METHOD OF PROFANITY WORDS
        private static string[] ProfanityArray()
        {
            // THESE WERE UPDATED TO USE THE SAME BADWORDS FROM FACESOFMBCFBAPP
            string[] words = {
"david",
"fuck",
"anal",
"anus",
"arse",
"ass",
"ballsack",
"balls",
"bastard",
"bitch",
"biatch",
"bloody",
"blowjob",
"blow job",
"bollock",
"bollok",
"boner",
"boob",
"bugger",
"bum",
"butt",
"buttplug",
"clitoris",
"cock",
"coon",
"crap",
"cunt",
"damn",
"dick",
"dildo",
"dyke",
"fag",
"feck",
"fellate",
"fellatio",
"felching",
"fuck",
"f u c k",
"fudgepacker",
"fudge packer",
"fudge",
"frick",
"flange",
"Goddamn",
"God damn",
"hell",
"homo",
"jerk",
"jizz",
"knobend",
"knob end",
"labia",
"lmao",
"lmfao",
"muff",
"nigger",
"nigga",
"omg",
"penis",
"piss",
"poop",
"prick",
"pube",
"pussy",
"queer",
"scrotum",
"sex",
"shit",
"s hit",
"sh1t",
"slut",
"smegma",
"spunk",
"tit",
"fat",
"tosser",
"turd",
"twat",
"vagina",
"wank",
"whore",
"wtf",
};

            return words;
        }
    }
}