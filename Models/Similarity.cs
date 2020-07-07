using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApplication5.Models
{
    public class Similarity
    {

        /*1. Similar ads are fetched on the basis of location, color, category to narrow down the results.
          2. Description of ad undergoes cleaning by removal of punctuation and other unnecessary characters.
          3. Tokenize function splits the description sentence into List of keywords.
          4. The keywords are then normalized using a dictionary.
          5. Similar ad's description also go through (2-4).
          6. Occurence class stores id of matched description along with ratio of match.
          7. A new list of ads is generated from occurences.
          8. Duplicates from already present ad list are removed.
          9. similarAds merge with newSimilarAds.
          10.List is sorted in descending order based on ratio of match (Higher Ratio first).*/ 

        class Occurence
        {
            public int Id { get; set; }
            public double Ratio { get; set; }
        }

        DBConnect c = DBConnect.getFController();
        List<AdViewModel> SimilarAds;
        public AdViewModel currentAd;
        DataTable dictionary;
        List<Occurence> descMatch = new List<Occurence>();
        List<AdViewModel> newSimilarAds=new List<AdViewModel>();

        char[] trimChars = {'.', ',', '/', '-', '@', '#', '$', '&', '=' };


        public int getAllAds(string type)
        {
            SimilarAds=c.getAds(currentAd, type);
            if (SimilarAds == null)
                return -1;
            return 1;
        }


        //This function gets Matches
        public List<AdViewModel> getMatches()
        {
            dictionary = c.getDictionary().Tables["myTable"];
            string Sentence = clean(currentAd.Description);
            List<string> ArrayOfWords = tokenize(Sentence);
            List<string> NormalizedWords = normalize(ArrayOfWords);

            foreach(AdViewModel ad in SimilarAds)
            {
                string TestSentence = clean(ad.Description);
                List<string> ArrayOfTestWords = tokenize(TestSentence);
                List<string> normalizedTestWords = normalize(ArrayOfTestWords);

                int occurences = getOccurences(NormalizedWords, normalizedTestWords);
                if (occurences > 0)
                {
                    double ratio = occurences/(ArrayOfWords.Count + ArrayOfTestWords.Count);
                    descMatch.Add(new Occurence()
                    {
                        Id = ad.Id,
                        Ratio = ratio
                    });
                }
            }

            descMatch.OrderByDescending(occur => occur.Ratio);
            getSimilarAds();
            return newSimilarAds;
        }


        public string clean(string desc)
        {
            desc = desc.ToLower();
            desc = desc.Trim(trimChars);
            return desc;
        }


        public List<string> tokenize(string sentence)
        {
            List<string> arrayOfWords = sentence.Split(' ').ToList();
            return arrayOfWords;
        }

        public List<string> normalize(List<string> arrayOfWords)
        {
            List<string> Normalized = new List<string>();
            foreach (string s in arrayOfWords)
            {
                foreach (DataRow r in dictionary.Rows)
                {
                    if (r["w1"].ToString() == s || r["w2"].ToString() == s || r["w3"].ToString() == s || r["w4"].ToString() == s)
                    {
                        Normalized.Add(r["result"].ToString());
                        break;
                    }
                    Normalized.Add(s);
                }
            }
            return Normalized;
        }

        public int getOccurences(List<string> firstList, List<string> secondList)
        {
            int occurrences = 0;
            foreach(string first in firstList)
                foreach(string second in secondList)
                {
                    if (first == second)
                    {
                        occurrences++;
                    }
                }
            return occurrences;
        }


        public void getSimilarAds()
        {
            foreach(Occurence o in descMatch)
                newSimilarAds.Add(c.getAd(o.Id));
            removeDuplicates();
            newSimilarAds.AddRange(SimilarAds);
        }

        public void removeDuplicates()
        {
            for(int i=0; i<newSimilarAds.Count; i++)
            {
                for (int j=0; j<SimilarAds.Count; j++)
                {
                    if(newSimilarAds[i].Id==SimilarAds[j].Id)
                    {
                        SimilarAds.RemoveAt(j);
                    }
                }
            }
        }
    }
}