using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Agent {
    [DataContract]
    public class WordnikDef {
        [DataMember]
        public string sourceDictionary;
        [DataMember]
        public string[] exampleUses;
        [DataMember]
        public string[] relatedWords;
        [DataMember]
        public string[] labels;
        [DataMember]
        public string[] citations;
        [DataMember]
        public string word;
        [DataMember]
        public string text;
        [DataMember]
        public string partOfSpeech;

        public static WordnikDef[] Create(Stream stream) {
            DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(WordnikDef[]));

            return (WordnikDef[])des.ReadObject(stream);
        }
    }
}
