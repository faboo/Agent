using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Agent {
    public class Definitions {
        private static string wordnik = "http://api.wordnik.com//v4/word.json/{0}/definitions?sourceDictionaries=wordnet";
        private static string wordnikKey = "6c2306919367258f7beaf05ffdb04e1d61e7667c71908dcbf";

        public string Define(string word) {
            string result = null;

            try {
                WebRequest request = WebRequest.Create(String.Format(wordnik, Uri.EscapeUriString(word)));
                WebResponse response;
                WordnikDef[] def;

                request.Headers["api_key"] = wordnikKey;

                response = request.GetResponse();
                def = WordnikDef.Create(response.GetResponseStream());

                response.Close();

                if (def.Length > 0)
                    result = def[0].text;
                else
                    result = "no definition found";
            } catch (Exception ex) {
                result = ex.Message;
            }

            return result;
        }
    }
}
